
using Microsoft.AspNetCore.Http;
using PowerApps.Modules.G9.Models;
using PowerApps.Shared;
using Vmvt.Npgsql;
using Vmvt.RouteAPI;
namespace PowerApps.Modules.G9.Methods;


/// <summary>Deklaracijos duomenų metodai</summary>
public static class Deklar {
	/// <summary>Gauti deklaracijos duomenis</summary>
	/// <param name="ctx"></param><returns></returns>
	public static async Task Get(HttpContext ctx) {
		if (ctx.CheckApi()) {
			var id = ctx.ParamIntN("dkl");
			if (id is not null) {
				var sql = $"SELECT * FROM g9.v_api_deklar WHERE \"ID\"=@id;";
				using var db = new DBRead(sql, Conn.G9, ("@id", id));
				var ret = await db.GetObject<Deklaracija>();
				if (ret is not null) await ctx.Response.WriteAsJsonAsync(ret);
				else await ctx.Response.E404();
			}
			else await ctx.Response.E400("Nenurodytas įrašo ID");
		}
		else await ctx.Response.E401();
	}


	/// <summary>Gauti deklaracijos rodiklius</summary>
	/// <param name="ctx"></param><returns></returns>
	public static async Task GetRod(HttpContext ctx) {
		if (ctx.CheckApi()) {
			var id = ctx.ParamIntN("dkl");
			if (id is not null) {
				var sql = $"SELECT rod_id \"ID\", rod_kodas \"Kodas\", rod_grupe \"Grupe\", rod_rodiklis \"Rodiklis\", rod_virsija \"Virsija\", rod_reikia \"Reikia\", rod_suvesta \"Suvesta\" FROM g9.valid_suvesti_detales(@id);";
				using var db = new DBRead(sql, Conn.G9, ("@id", id));
				var ret = await db.GetList<DklRodiklis>();
				if (ret is not null) await ctx.Response.WriteAsJsonAsync(ret);
				else await ctx.Response.E404();
			}
			else await ctx.Response.E400("Nenurodytas įrašo ID");
		}
		else await ctx.Response.E401();
	}

	/// <summary>Gauti rodiklio reikšmes</summary>
	/// <param name="ctx"></param><returns></returns>
	public static async Task GetReiksm(HttpContext ctx) {
		if (ctx.CheckApi()) {
			var dkl = ctx.ParamIntN("dkl");
			var rod = ctx.ParamIntN("rod");
			if (dkl is not null && rod is not null) {
				var sql = $"SELECT * FROM g9.v_api_deklar_reiksmes WHERE \"Deklar\"=@dkl and \"Rodiklis\"=@rod;";
				using var db = new DBRead(sql, Conn.G9, ("@dkl", dkl), ("@rod", rod));
				var ret = await db.GetList<DklReiksmes>();
				if (ret is not null) await ctx.Response.WriteAsJsonAsync(ret);
				else await ctx.Response.E404();
			}
			else await ctx.Response.E400("Nenurodytas įrašo ID");
		}
		else await ctx.Response.E401();
	}
}