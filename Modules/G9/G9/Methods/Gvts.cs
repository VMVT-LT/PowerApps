using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Any;
using Npgsql;
using PowerApps.Modules.G9.Models;
using PowerApps.Shared;
using System.Runtime.InteropServices.Marshalling;
using Vmvt.Npgsql;
using Vmvt.RouteAPI;

namespace PowerApps.Modules.G9.Methods;


/// <summary>Geriamojo vandens tiekimo sistemos</summary>
public static class Gvts {


	private static async Task<List<T>> DBList<T>(string sql, (string, object?)[] prm) where T : new() {
		using var db3 = new DBRead(sql, Conn.G9, prm);
		return await db3.GetList<T>();
	}

	private static readonly string SqlList1 = "SELECT * FROM g9.v_api_gvts WHERE {0};";
	private static readonly string SqlList2 = "SELECT * FROM g9.v_api_ja j WHERE EXISTS (SELECT 1 FROM g9.gvts g WHERE j.id = g.vkl_ja {0});";
	private static readonly string SqlList3 = "SELECT * FROM g9.v_api_users u WHERE EXISTS (SELECT 1 FROM g9.gvts g LEFT JOIN g9.gvts_insp i ON (g.id=i.vkl_id) WHERE u.id = i.vkl_insp {0});";

	/// <summary>GVTS sąrašas</summary>
	/// <param name="ctx"></param><returns></returns>
	public static async Task List(HttpContext ctx) {
		if (ctx.CheckApi()) {
			try {
				var sav = ctx.ParamLongN("sav");
				var apg = ctx.ParamLongN("apg");
				var inact = ctx.ParamTrue("inact");
				var prm = new (string, object?)[] { ("@sav", sav), ("@apg", apg), ("@inact", inact) };
				var ret = new GvtsList() {
					GVTS = await DBList<GvtsListItem>(string.Format(SqlList1, $" (active <> @inact) {(sav > 0 ? " AND sav=@sav" : "")}{(apg > 0 ? " AND apg=@apg" : "")}"), prm)
				};
				if (ret.GVTS.Count > 0) {
					var whr = $" AND (vkl_active <> @inact) {(sav > 0 ? " AND vkl_adr_sav=@sav" : "")}{(apg > 0 ? " AND vkl_adr_apg=@apg" : "")}";
					ret.Subjektai = await DBList<JARDetails>(string.Format(SqlList2, whr), prm);
					ret.Inspektoriai = await DBList<User>(string.Format(SqlList3, whr), prm);
				}
				await ctx.Response.WriteAsJsonAsync(ret);
			}
			catch (Exception ex) {
				await ctx.Response.E500("Nenumatyta klaida", [ex.Message]);
			}
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
				var gvts = await db.GetObject<GvtsListItem>();

				if (gvts is not null) {
					var ret = new GvtsItem() {
						ID = gvts.ID, Active=gvts.Active, Adresas=gvts.Adresas, GVTOT=gvts.GVTOT, Pavad=gvts.Pavad, Pakeista=gvts.Pakeista,
						JA = await Jar.JarInfo(gvts.JA ?? 0)
					};
					var db2 = new DBRead("SELECT * FROM g9.v_api_users u WHERE EXISTS (SELECT * FROM g9.gvts g LEFT JOIN g9.gvts_insp i ON (g.id=i.vkl_id) WHERE g.vkl_id=@id and i.vkl_insp=u.id)", Conn.G9, ("@id", id));
					ret.Inspektoriai = await db2.GetList<User>();
					await ctx.Response.WriteAsJsonAsync(ret);
				}
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
				if (dt.JA < 100000) await ctx.Response.E400("Nenurodytas juridinis asmuo");
				else if (dt.Adresas < 10000) await ctx.Response.E400("Nenurodytas adresas");
				else if ((dt.Pavad?.Length ?? 0) < 3) await ctx.Response.E400("Nenurodytas pavadinimas");
				else if ((dt.GVTOT?.Length ?? 0) < 10) await ctx.Response.E400("Nenurodytas teritorijos kodas");
				else if (!dt.GVTOT!.StartsWith("LT0")) await ctx.Response.E400("Neteisingas teritorijos kodas");
				else {
					var adr = await Adr.GetAdr(dt.Adresas);

					if (adr is null || adr.Adm is null || adr.Sav is null) await ctx.Response.E400("Neteisingas adresas");
					else {
						var pref = $"LT0{(adr.Adm.ID == 10 ? "11" : "2" + adr.Adm.ID)}{adr.Sav.ID}";

						if (!dt.GVTOT.StartsWith(pref)) await ctx.Response.E400("Neteisingas GVTOT kodas");
						else {
							var ja = await Jar.GetJar(dt.JA);
							if (ja is null) await ctx.Response.E400("Juridinis asmuo nerastas");
							else if (ja.StatusKodas == 10) await ctx.Response.E400("Juridinis asmuo išregistruotas");
							else {

								using var db = new DBRead("SELECT * FROM g9.v_api_gvts WHERE id=@id;", Conn.G9, ("@id", id));
								var ret = await db.GetObject<GvtsListItem>();

								var param = new (string key, object? val)[] { ("@id", id), ("@title", dt.Pavad), ("@ja", dt.JA), ("@gvtot", dt.GVTOT), ("@aob", dt.Adresas), ("@apg", adr.Apg?.ID), ("@sav", adr.Sav.ID), ("@saviv", adr.Sav.Vardas), ("@adr", $"{adr.Pavad}, {adr.Vietove}"), ("@act", dt.Active) };

								var sql = ret is null ?
									"INSERT INTO g9.gvts(vkl_id,vkl_ja,vkl_title,vkl_saviv,vkl_adresas,vkl_gvtot,vkl_active,vkl_adr_aob,vkl_adr_sav,vkl_date,vkl_adr_apg) VALUES (@id, @ja, @title, @saviv, @adr, @gvtot, @act, @aob, @sav, timezone('utc',now()), @apg);" :
									"UPDATE g9.gvts SET vkl_ja=@ja, vkl_title=@title, vkl_saviv=@saviv, vkl_adresas=@adr, vkl_gvtot=@gvtot, vkl_active=@act, vkl_adr_aob=@aob, vkl_adr_sav=@sav, vkl_date=timezone('utc',now()), vkl_adr_apg=@apg WHERE vkl_id=@id;";

								await Conn.G9!.Execute(sql + " SELECT g9.sync_ja_detales();", param);
								if(dt.Active) await Jar.UpdateJar(dt.JA);
								await Info(ctx);
							}
						}
					}
				}
			}
			else await ctx.Response.E400();
		}
		else await ctx.Response.E401();
	}


	/// <summary>Pridėti inspektorių</summary>
	/// <param name="ctx"></param><returns></returns>
	public static async Task UserAdd(HttpContext ctx) {
		if (ctx.CheckApi()) {
			var gvts = ctx.ParamLong("gvts");
			var usr = ctx.ParamStringN("usr");
			if (gvts > 0) {
				if (Guid.TryParse(usr, out var id)) {
					using var db = new DBRead("SELECT id FROM g9.gvts WHERE vkl_id=@id", Conn.G9, ("@id", gvts));
					var gvid = await db.GetScalar<int>();
					if (gvid > 0) {
						try {
							await Conn.G9!.Execute("INSERT INTO g9.gvts_insp (vkl_id,vkl_insp) VALUES (@gvid,@id);", ("@id", id), ("@gvid", gvid));
							await ctx.Response.Ok();
						}
						catch (PostgresException ex) {
							switch (ex.SqlState) {
								case "23505": await ctx.Response.Ok("Vartotojas jau buvo pridėtas"); break;
								case "23503": await ctx.Response.E400("Vartotojas nerastas"); break;
								default: await ctx.Response.E500("Klaida pridedant vartotoją"); break;
							}
						}
						catch (Exception) { await ctx.Response.E500("Nenumatyta klaida pridedant vartotoją"); }
					}
					else await ctx.Response.E404("Nerastas GVTS");
				}
				else await ctx.Response.E400("Neteisingas vartotojo identifikatorius");
			}
			else await ctx.Response.E400("Nenurodytas GVTS");
		}
		else await ctx.Response.E401();
	}

	/// <summary>Pašalinti inspektorių</summary>
	/// <param name="ctx"></param><returns></returns>
	public static async Task UserRem(HttpContext ctx) {
		if (ctx.CheckApi()) {
			var gvts = ctx.ParamLong("gvts");
			var usr = ctx.ParamStringN("usr");
			if (gvts > 0) {
				if (Guid.TryParse(usr, out var id)) {
					using var db = new DBRead("SELECT id FROM g9.gvts WHERE vkl_id=@id", Conn.G9, ("@id", gvts));
					var gvid = await db.GetScalar<int>();
					if (gvid > 0) {
						try {
							await Conn.G9!.Execute("DELETE FROM g9.gvts_insp WHERE vkl_id=@gvid and vkl_insp=@id;", ("@id", id), ("@gvid", gvid));
							await ctx.Response.Ok();
						}
						catch (Exception) { await ctx.Response.E500("Nenumatyta klaida pridedant vartotoją"); }
					}
					else await ctx.Response.E404("Nerastas GVTS");
				}
				else await ctx.Response.E400("Neteisingas vartotojo identifikatorius");
			}
			else await ctx.Response.E400("Nenurodytas GVTS");
		}
		else await ctx.Response.E401();
	}
}