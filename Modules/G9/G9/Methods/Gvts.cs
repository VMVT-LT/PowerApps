

using Microsoft.AspNetCore.Http;
using PowerApps.Modules.G9.Models;
using PowerApps.Shared;
using Vmvt.Npgsql;
using Vmvt.RouteAPI;

namespace PowerApps.Modules.G9.Methods;


/// <summary>Geriamojo vandens tiekimo sistemos</summary>
public static class Gvts {

	/// <summary>GVTS sąrašas</summary>
	/// <param name="ctx"></param><returns></returns>
	public static async Task List(HttpContext ctx) {
		if (ctx.CheckApi()) {
			var sql = $"SELECT * FROM g9.v_api_gvts WHERE (@sav::int IS NULL OR sav=@sav) AND (@apg::int IS NULL OR apg=@apg);";
			using var db = new DBRead(sql, Conn.G9, ("@sav", ctx.ParamLongN("sav")), ("@apg", ctx.ParamLongN("apg")));
			var ret = await db.GetList<GvtsDetails>();
			if (ret is not null) await ctx.Response.WriteAsJsonAsync(ret);
			else await ctx.Response.E404();
		}
		else await ctx.Response.E401();
	}


	/// <summary>GVTS informacija</summary>
	/// <param name="ctx"></param><returns></returns>
	public static async Task Info(HttpContext ctx) {
		if (ctx.CheckApi()) {
			var id = ctx.ParamLongN("id");
			if (id is not null) {
				using var db = new DBRead("SELECT * FROM g9.v_api_gvts WHERE id=@id;", Conn.G9, ("@id", id));
				var ret = await db.GetObject<GvtsDetails>();
				if (ret is not null) await ctx.Response.WriteAsJsonAsync(ret);
				else await ctx.Response.E404();
			}
			else await ctx.Response.E400();
		}
		else await ctx.Response.E401();
	}


	/// <summary>GVTS informacijos keitimas</summary>
	/// <param name="ctx"></param><param name="dt"></param><returns></returns>
	public static async Task InfoSet(HttpContext ctx, GvtsDtlSet dt) {
		if (ctx.CheckApi()) {
			var id = ctx.ParamLongN("id");
			if (id is not null) {
				using var db = new DBRead("SELECT * FROM g9.v_api_gvts WHERE id=@id;", Conn.G9, ("@id", id));
				var ret = await db.GetObject<GvtsDetails>();
				if (ret is not null) await ctx.Response.WriteAsJsonAsync(ret);
				else await ctx.Response.E404();
			}
			else await ctx.Response.E400();
		}
		else await ctx.Response.E401();
	}

}