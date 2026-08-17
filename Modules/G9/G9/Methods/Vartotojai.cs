using Microsoft.AspNetCore.Http;
using PowerApps.Modules.G9.Models;
using PowerApps.Shared;
using Vmvt.Npgsql;
using Vmvt.RouteAPI;

namespace PowerApps.Modules.G9.Methods;

/// <summary>Vartotojų duomenų metodai</summary>
public static class Users {
	private static HttpClient HClient { get; } = new() { BaseAddress = new(G9API.Cfg.Base) };

	/// <summary>G9 Vartotojai</summary>
	/// <param name="ctx"></param><returns></returns>
	public static async Task List(HttpContext ctx) {
		if (ctx.CheckApi()) {
			var apg = ctx.ParamIntN("apg");
			var inact = ctx.ParamTrue("inact");
			var sql = $"SELECT * FROM g9.v_api_users WHERE 1=1 {(inact ? "" : " AND active")}{(apg > 0 ? " AND apygarda=@apg" : "")};";
			using var db = new DBRead(sql, Conn.G9, ("@apg", apg), ("@inact", inact));
			var ret = await db.GetList<User>();
			if (ret is not null) await ctx.Response.WriteAsJsonAsync(ret);
			else await ctx.Response.E404();
		}
		else await ctx.Response.E401();
	}




}