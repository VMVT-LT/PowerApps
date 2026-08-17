using Microsoft.AspNetCore.Http;
using PowerApps.Modules.G9.Models;
using PowerApps.Shared;
using System;
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
			if (id > 0) {
				using var db = new DBRead("SELECT * FROM g9.v_api_rodikliai WHERE id=@id;", Conn.G9, ("@id", id));
				var ret = await db.GetObject<Rodiklis>();
				if (ret is not null) await ctx.Response.WriteAsJsonAsync(ret);
				else await ctx.Response.E404();
			}
		}
		else await ctx.Response.E401();
	}

	private static readonly string RodInsSql = "INSERT INTO g9.rodikliai(rod_grupe, rod_kodas, rod_rodiklis, rod_daznumas, rod_min, rod_max, rod_step, rod_decim, rod_vnt, rod_aprasymas, rod_delete) VALUES (@grupe,@kodas,@pavad,@dazn,@min,@max,@step,@decim,@vnt,@apras,NOT @active) RETURNING rod_id;";
	private static readonly string RodUpdSql = "UPDATE g9.rodikliai SET rod_grupe=@grupe, rod_kodas=@kodas, rod_rodiklis=@pavad, rod_daznumas=@dazn, rod_min=@min, rod_max=@max, rod_step=@step, rod_decim=@decim, rod_vnt=@vnt, rod_aprasymas=@apras, rod_delete=NOT @active WHERE rod_id=@id RETURNING rod_id;";

	/// <summary>Rodiklio informacijos keitimas</summary>
	/// <param name="id">Rodiklio ID</param><param name="dt">Nauja rodiklio informacija</param><param name="ctx"></param><returns></returns>
	public static async Task SetInfo(HttpContext ctx, int id, RodDtlSet dt) {
		if (ctx.CheckApi()) {
			if (id >= 0) {
				if (dt.Kodas?.Length < 9 && id == 0) await ctx.Response.E400("Neteisingas rodiklio kodas");
				else if (dt.Pavad is null || dt.Pavad.Length < 3) await ctx.Response.E400("Neteisingas rodiklio pavadinimas");
				else if (dt.Vnt is null || dt.Vnt.Length < 1) await ctx.Response.E400("Neteisingi rodiklio vienetai");
				else if (dt.Min == dt.Max && dt.Min != 1) await ctx.Response.E400("Minimali ir maksimali reikšmės negali būti vienodos");
				else if (dt.Step > 1 || dt.Step <= 0) await ctx.Response.E400("Žingsnis negali būti didesnis už 1 ir mažesnis ar lygus 0");
				else if (dt.Decim < 0 || dt.Decim > 10) await ctx.Response.E400("Po kablelio rodomų skaičių gali būti 0-10");
				else {
					var chk = "SELECT EXISTS (SELECT 1 FROM g9.lookup WHERE lkp_group='RodikliuGrupe' AND lkp_num=@grp), EXISTS (SELECT 1 FROM g9.lookup WHERE lkp_group='Daznumas' AND lkp_key=@dzn);";
					using var dbv = new DBRead(chk, Conn.G9, ("@grp", dt.Grupe), ("@dzn", dt.Daznumas));
					using var rdr = await dbv.GetReader();
					if (await rdr.ReadAsync()) {
						if (!rdr.GetBoolean(0)) await ctx.Response.E400("Neteisinga rodiklių grupė");
						else if (!rdr.GetBoolean(1)) await ctx.Response.E400("Neteisingas rodiklio dažnumas");
						else {
							var prm = new (string, object?)[] { ("@id",id), ("@kodas",dt.Kodas), ("@grupe",dt.Grupe), ("@pavad",dt.Pavad), ("@dazn",dt.Daznumas),
									("@min",dt.Min), ("@max",dt.Max), ("@step",dt.Step), ("@decim",dt.Decim), ("@vnt",dt.Vnt), ("@apras",dt.Apras), ("@active",dt.Active) };

							var dbi = new DBRead(id == 0 ? RodInsSql : RodUpdSql, Conn.G9, prm);
							var ret = await dbi.GetScalar<int>();
							await Info(ctx, ret);
						}
					}
					else await ctx.Response.E400("Nenumatyta klaida");
				}
			}
			else await ctx.Response.E400("Neteisingas rodiklio ID");
		}
		else await ctx.Response.E401();
	}

}