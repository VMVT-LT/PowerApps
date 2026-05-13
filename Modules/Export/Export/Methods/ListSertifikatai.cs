using Microsoft.AspNetCore.Http;
using PowerApps.Modules.Export.Models;
using PowerApps.Shared;
using System.Collections.Concurrent;
using System.Text;
using Vmvt.Npgsql;
using Vmvt.RouteAPI;

namespace PowerApps.Modules.Export.Methods;

/// <summary>Sertifikato sąrašo metodai</summary>
public static partial class ListSertifikatai {
	/// <summary>Duomenų pasiimimo lentelė</summary>
	public static readonly string FilterView = "export.v_api_cert_list";
	/// <summary>Maksimalus rezultatų skaičius puslapyje</summary>
	public static readonly int FilterMaxPage = 1000;
	private static readonly List<string> FilterSortFields = ["cert_id", "cert_nr", "cert_imp_salis", "cert_status", "cert_type", "cert_isdave", "cert_isdave_dep", "cert_export"];


	/// <summary>Sertifikatų sąrašas</summary>
	/// <param name="ctx"></param>
	/// <param name="qry">Filtro užklausa</param>
	/// <returns></returns>
	public static async Task Filter(HttpContext ctx, ListQuery<ListFilter_Sertifikatai> qry) {
		if (ctx.CheckApi()) {
			var prm = qry.Filter?.MakeSearch() ?? new();
			await prm.GetCount();

			if (prm.Total > 0) {
				if (qry.Page > FilterMaxPage) qry.Page = FilterMaxPage;
				if (qry.Page < 1) qry.Page = 1;
				if (qry.Limit < 1) qry.Limit = 10;
				var srt = new List<string>();
				if (qry.Sort?.Count > 0) {
					foreach (var i in qry.Sort) {
						var sp = i.ToLower().Split(' ');
						if (FilterSortFields.Contains(sp[0].ToLower()))
							srt.Add((sp.Length > 1 && sp[1] == "desc") ? $"{sp[0]} {sp[1]}" : sp[0]);
					}
				}

				var sql = $"SELECT data FROM {FilterView} t " + (prm.Where.Count > 0 ? " WHERE " + string.Join(" AND ", prm.Where) : "") +
					(srt.Count > 0 ? " ORDER By " + string.Join(", ", srt) : "") +
					$" LIMIT {qry.Limit} OFFSET {(qry.Page - 1) * qry.Limit};";


				using var db = new DBRead(sql, prm.Params, Conn.Export);
				await using var rdr = await db.GetReader();
				ctx.Response.ContentType = "application/json; charset=utf-8";
				await ctx.Response.WriteAsync($"{{\"page\":{qry.Page},\"total\":{prm.Total},\"data\":[");
				var comma = false; var incr = 0;
				while (await rdr.ReadAsync()) {
					if (comma) await ctx.Response.WriteAsync(",");
					else comma = true; incr++;
					await ctx.Response.Body.WriteAsync(await rdr.GetFieldValueAsync<byte[]>(0));
				}
				await ctx.Response.WriteAsync($"],\"items\":{incr}}}");
			}
			else {
				ctx.Response.ContentType = "application/json; charset=utf-8";
				await ctx.Response.WriteAsync($"{{\"page\":{qry.Page},\"total\":0,\"data\":[],\"items\":0}}");
			}
		}
		else await ctx.Response.E401();
	}


	private static FilterData MakeSearch(this ListFilter_Sertifikatai qry) {
		var ret = new FilterData();

		if (qry.User is not null) ret.Add(
			$"EXISTS (SELECT 1 FROM export.cert_users q WHERE t.cert_id=user_cert AND user_id=@user)", ("@user", qry.User));
		if (qry.Apygarda is not null) ret.Add("cert_created_user_dep", qry.Apygarda);
		if (qry.IsdaveDep is not null) ret.Add("cert_isdave_dep", qry.IsdaveDep);
		if (qry.Exportuotojas is not null) ret.Add("cert_export", qry.Exportuotojas);
		if (qry.Postas is not null) ret.Add("cert_postas", qry.Postas);
		if (qry.Status is not null) ret.Add("cert_status", qry.Status);
		if (qry.Type is not null) ret.Add("cert_type", qry.Type);
		if (qry.IsdaveUsr is not null) ret.Add("cert_isdave", qry.IsdaveUsr);
		if (qry.Salis is not null) ret.Add("cert_imp_salis", qry.Salis);
		if (qry.PostIssue is not null) ret.Add("cert_post_issued", qry.PostIssue);
		if (qry.Warehouse is not null) ret.Add("cert_warehouse", qry.Warehouse);
		if (qry.Pakeistas is not null) ret.Add((qry.Pakeistas.Value ? "" : " not ") + "cert_pakeistas", ("@pkst", qry.Pakeistas));
		if (qry.Files is not null) ret.Add($"cert_file_count {(qry.Files.Value ? ">" : "=")} 0", ("@file", qry.Files));
		if (qry.DateSukurNuo is not null) ret.Add($"cert_date_created>=@dtsuknuo::date", ("@dtsuknuo", qry.DateSukurNuo));
		if (qry.DateSukurIki is not null) ret.Add($"cert_date_created<=@dtsukiki::date", ("@dtsukiki", qry.DateSukurIki));
		if (qry.DateIsdavNuo is not null) ret.Add($"cert_date_isdavimo>=@dtisdnuo::date", ("@dtisdnuo", qry.DateIsdavNuo));
		if (qry.DateIsdavIki is not null) ret.Add($"cert_date_isdavimo<=@dtisdiki::date", ("@dtisdiki", qry.DateIsdavIki));

		if (qry.Gamintojas is not null) ret.Add(
			$"EXISTS (SELECT 1 FROM export.cert_produktai WHERE t.cert_id=prod_cert_id and prod_gamintojas=@gamin)", ("@gamin", qry.Gamintojas));
		if (qry.KPN is not null) ret.Add(
			$"EXISTS (SELECT 1 FROM export.cert_produktai WHERE t.cert_id=prod_cert_id and prod_kpn=@kpn)", ("@kpn", qry.KPN));
		if (qry.Veluoja == true) ret.Add("cert_status='Pildoma' AND cert_date_created < (CURRENT_DATE - " +
			"CASE WHEN EXTRACT(dow FROM CURRENT_DATE) = ANY(ARRAY[1,2,3]) THEN '5 days' ELSE '3 days' END::interval)", ("@vel", "uoja"));

		if (!string.IsNullOrWhiteSpace(qry.Search)) ret.Add($"cert_search like '%'||@srh||'%'", ("@srh", qry.Search.MkSerach()));

		return ret;
	}





	private class FilterData {
		private static readonly ConcurrentDictionary<string, (int count, DateTime reload)> Cache = [];
		public List<string> Where { get; set; } = [];
		public Dictionary<string, object?> Params { get; set; } = [];
		public int Total { get; set; }
		public void Add(string field, object? data) { Where.Add($"{field}=@{field}"); Params[$"@{field}"] = data; }
		public void Add(string where, params (string key, object? val)[] param) { Where.Add(where); foreach (var (key, val) in param) Params[key] = val; }

		private string GetHash() {
			var ret = new StringBuilder("q:");
			var keys = Params.Keys.OrderBy(k => k);
			foreach (var i in keys) {
				var val = Params[i];
				ret.Append(i); ret.Append('=');
				ret.Append(val?.ToString() ?? "#");
				ret.Append('|');
			}
			return ret.ToString();
		}

		public async Task GetCount() {
			var hs = GetHash();
			var now = DateTime.UtcNow;
			if (!Cache.TryGetValue(hs, out var cnt)) cnt = (0, now);
			if (cnt.reload <= now) {
				Cache[hs] = (cnt.count, DateTime.UtcNow.AddMinutes(1));
				using var db = new DBRead($"SELECT COUNT(*) FROM {FilterView} t " + (Where.Count > 0 ? " WHERE " + string.Join(" AND ", Where) : ""), Params, Conn.Export);
				Cache[hs] = (Total = (int)await db.GetScalar<long>(), DateTime.UtcNow.AddMinutes(5));
			}
			else Total = cnt.count;
		}
	}

}