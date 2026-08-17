using Microsoft.AspNetCore.Http;
using PowerApps.Modules.G9.Models;
using PowerApps.Shared;
using System.Diagnostics.Eventing.Reader;
using Vmvt.Npgsql;
using Vmvt.RouteAPI;

namespace PowerApps.Modules.G9.Methods;


/// <summary>Rodiklių metodai</summary>
public static class Rodikliai {

	/// <summary>Rodiklių sąrašas</summary>
	/// <param name="ctx"></param><returns></returns>
	public static async Task List(HttpContext ctx) {
		if (ctx.CheckApi()) {
			var grp = ctx.ParamIntN("g");
			var inact = ctx.ParamTrue("inact");
			var sql = $"SELECT * FROM g9.v_api_rodikliai WHERE 1=1 {(inact ? "" : " AND active")}{(grp > 0 ? " AND grupe=@g" : "")};";
			using var db = new DBRead(sql, Conn.G9, ("@g", grp), ("@inact", inact));
			var ret = await db.GetList<Rodiklis>();
			if (ret is not null) await ctx.Response.WriteAsJsonAsync(ret);
			else await ctx.Response.E404();
		}
		else await ctx.Response.E401();
	}

	/// <summary>Rodiklio informacija</summary>
	/// <param name="id">Rodiklio ID</param><param name="ctx"></param><returns></returns>
	public static async Task Info(HttpContext ctx, int id) {
		if (ctx.CheckApi()) {
			using var db = new DBRead("SELECT * FROM g9.v_api_rodikliai WHERE id=@id;", Conn.G9, ("@id", id));
			var ret = await db.GetObject<Rodiklis>();
			if (ret is not null) await ctx.Response.WriteAsJsonAsync(ret);
			else await ctx.Response.E404();
		}
		else await ctx.Response.E401();
	}

	/// <summary>Rodiklio informacijos keitimas</summary>
	/// <param name="id">Rodiklio ID</param><param name="dt">Nauja rodiklio informacija</param><param name="ctx"></param><returns></returns>
	public static async Task SetInfo(HttpContext ctx, int id, RodDtlSet dt) {
		if (ctx.CheckApi()) {
			var chk = "SELECT EXISTS (SELECT 1 FROM g9.lookup WHERE lkp_group='RodikliuGrupe' AND lkp_num=@grp), EXISTS (SELECT 1 FROM g9.lookup WHERE lkp_group='Daznumas' AND lkp_key=@dzn);";
			using var dbv = new DBRead(chk, Conn.G9, ("@grp", dt.Grupe), ("@dzn", dt.Daznumas));
			using var rdr = await dbv.GetReader();
			if (await rdr.ReadAsync()) {
				if (!rdr.GetBoolean(0)) await ctx.Response.E400("Neteisinga rodiklių grupė");
				else if (!rdr.GetBoolean(1)) await ctx.Response.E400("Neteisingas rodiklio dažnumas");
				else {

					await Info(ctx, id);

				}
			}
			else await ctx.Response.E400("Nenumatyta klaida");
		}
		else await ctx.Response.E401();
	}

}