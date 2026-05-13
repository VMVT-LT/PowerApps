using Microsoft.AspNetCore.Http;
using PowerApps.Shared;
using System.Collections.Concurrent;
using Vmvt.Npgsql;
using Vmvt.RouteAPI;

namespace PowerApps.Modules.Export.Methods;

/// <summary>Sertifikato duomenų gavimas</summary>
public static partial class GetData {
	private static readonly string Schema = "export";
	/// <summary>Duomenų pasiimimo lentelės prefiksas</summary>
	private static readonly string ListView = "v_api_list_";
	/// <summary>Duomenų pasiimimo lentelės prefiksas</summary>
	private static readonly string ItemView = "v_api_item_";

	/// <summary>Duomenų sąrašas</summary>
	/// <param name="view">Duomenų pasiimimo šaltinis</param>
	/// <param name="ctx"></param>
	/// <returns></returns>
	public static async Task List(HttpContext ctx, string view) {
		if (ctx.CheckApi()) {
			if (await CheckListView(view)) {
				var id = ctx.ParamIntN("id");
				var page = ctx.ParamIntN("page") ?? 1;
				var limit = ctx.ParamIntN("limit") ?? 100;

				if (id is not null) {
					var cnt = await GetListCount(view, id.Value, ctx.ParamTrue("force"));

					ctx.Response.ContentType = "application/json; charset=utf-8";
					if (cnt > 0) {
						var sql = $"SELECT data FROM \"{Schema}\".\"{ListView}{view}\" WHERE id=@id" +
							(ctx.ParamNull("desc") ? "" : (" ORDER By sort " + (ctx.ParamTrue("desc") ? "desc" : "asc"))) +
							$" LIMIT {limit} OFFSET {(page - 1) * limit};";

						using var db = new DBRead(sql, Conn.Export, ("@id", id));
						await using var rdr = await db.GetReader();

						await ctx.Response.WriteAsync($"{{\"page\":{page},\"total\":{cnt},\"data\":[");
						var comma = false; var incr = 0;
						while (await rdr.ReadAsync()) {
							if (comma) await ctx.Response.WriteAsync(",");
							else comma = true; incr++;
							await ctx.Response.Body.WriteAsync(await rdr.GetFieldValueAsync<byte[]>(0));
						}
						await ctx.Response.WriteAsync($"],\"items\":{incr}}}");
					}
					else {
						await ctx.Response.WriteAsync($"{{\"page\":{page},\"total\":0,\"data\":[],\"items\":0}}");
					}
				}
				else await ctx.Response.E400("Nenurodytas įrašo ID");
			}
			else await ctx.Response.E404();
		}
		else await ctx.Response.E401();
	}


	/// <summary>Įrašo duomenys</summary>
	/// <param name="view">Įrašo duomenų šaltinis</param>
	/// <param name="ctx"></param>
	/// <returns></returns>
	public static async Task Item(HttpContext ctx, string view) {
		if (ctx.CheckApi()) {
			if (await CheckItemView(view)) {
				var id = ctx.ParamIntN("id");

				if (id is not null) {
					var sql = $"SELECT data FROM \"{Schema}\".\"{ItemView}{view}\" WHERE id=@id;";

					using var db = new DBRead(sql, Conn.Export, ("@id", id));
					await using var rdr = await db.GetReader();

					if (await rdr.ReadAsync()) {
						ctx.Response.ContentType = "application/json; charset=utf-8";
						await ctx.Response.Body.WriteAsync(await rdr.GetFieldValueAsync<byte[]>(0));
					}
					else await ctx.Response.E404();
				}
				else await ctx.Response.E400("Nenurodytas įrašo ID");
			}
			else await ctx.Response.E404();
		}
		else await ctx.Response.E401();
	}

	/// <summary>Sertifikato duomenys</summary>
	/// <param name="ctx"></param>
	/// <returns></returns>
	public static async Task Sertifikatas(HttpContext ctx) {
		if (ctx.CheckApi()) {
			var id = ctx.ParamInt("id");
			var nr = ctx.ParamString("nr")?.ToUpper() ?? "";

			if (id > 0 || nr.StartsWith("EXP")) {
				var sql = $"SELECT data FROM export.v_api_cert_item WHERE " +
					(id > 0 ? "id=@id;" : "nr=@nr;");

				using var db = new DBRead(sql, Conn.Export, ("@id", id), ("@nr", nr));
				await using var rdr = await db.GetReader();

				if (await rdr.ReadAsync()) {
					ctx.Response.ContentType = "application/json; charset=utf-8";
					await ctx.Response.Body.WriteAsync(await rdr.GetFieldValueAsync<byte[]>(0));
				}
				else await ctx.Response.E404();
			}
			else await ctx.Response.E400("Nenurodytas įrašo ID");
		}
		else await ctx.Response.E401();
	}



	private static readonly ConcurrentDictionary<string, (int count, DateTime reload)> ListCountCache = [];
	private static async Task<int> GetListCount(string view, int id, bool force = false) {
		var hs = $"{view}|{id}";
		var now = DateTime.UtcNow;
		if (force || !ListCountCache.TryGetValue(hs, out var cnt) || cnt.reload <= now) {
			using var db = new DBRead($"SELECT COUNT(*) FROM \"{Schema}\".\"{ListView}{view}\" WHERE id=@id;", Conn.Export, ("@id", id));
			var ret = (int)await db.GetScalar<long>();
			ListCountCache[hs] = (ret, DateTime.UtcNow.AddMinutes(1));
			return ret;
		}
		return cnt.count;
	}

	private static List<string> ListViews { get; set; } = [];
	private static async Task<bool> CheckListView(string view) =>
		ListViews.Contains($"{ListView}{view}") || (ListViews = await GetTables(ListView)).Contains($"{ListView}{view}");

	private static List<string> ItemViews { get; set; } = [];
	private static async Task<bool> CheckItemView(string view) =>
		ItemViews.Contains($"{ItemView}{view}") || (ItemViews = await GetTables(ItemView)).Contains($"{ItemView}{view}");


	private static async Task<List<string>> GetTables(string prefix) {
		using var db = new DBRead($"SELECT table_name FROM information_schema.views WHERE table_schema='{Schema}' AND table_name LIKE @pref||'%';", Conn.Export, ("@pref", prefix));
		using var rdr = await db.GetReader();
		var ret = new List<string>();
		while (await rdr.ReadAsync()) ret.Add(rdr.GetString(0));
		return ret;
	}

}