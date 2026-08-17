using Microsoft.AspNetCore.Http;
using PowerApps.Shared;
using Vmvt.Npgsql;
using Vmvt.RouteAPI;

namespace PowerApps.Modules.G9.Methods;

/// <summary>Vartotojų duomenų metodai</summary>
public static class Lookup {
	/// <summary>Klasifikatorių reikšmės</summary>
	/// <param name="ctx"></param><returns></returns>
	public static async Task List(HttpContext ctx) {
		using var db = new DBRead("SELECT lookup_data FROM g9.v_lookup;", Conn.G9);
		var rsp = await db.GetScalar<string>();
		if (rsp != null) {
			ctx.Response.ContentType = "application/json; charset=utf-8";
			await ctx.Response.WriteAsync(rsp);
		}
		else await ctx.Response.E404();
	}
}



