

using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Any;
using PowerApps.Modules.G9.Models;
using PowerApps.Shared;
using System.Runtime.InteropServices.Marshalling;
using Vmvt.Npgsql;
using Vmvt.RouteAPI;

namespace PowerApps.Modules.G9.Methods;


/// <summary>Geriamojo vandens tiekimo sistemos</summary>
public static class Gvts {

	/// <summary>GVTS sąrašas</summary>
	/// <param name="ctx"></param><returns></returns>
	public static async Task List(HttpContext ctx) {
		if (ctx.CheckApi()) {
			var inact = ctx.ParamTrue("inact");
			var sql = $"SELECT * FROM g9.v_api_gvts WHERE (@sav::int IS NULL OR sav=@sav) AND (@apg::int IS NULL OR apg=@apg) AND (active <> @inact);";
			using var db = new DBRead(sql, Conn.G9, ("@sav", ctx.ParamLongN("sav")), ("@apg", ctx.ParamLongN("apg")), ("@inact", inact));
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

						if(!dt.GVTOT.StartsWith(pref)) await ctx.Response.E400("Neteisingas GVTOT kodas");
						else {
							var ja = await Jar.GetJar(dt.JA);
							if (ja is null) await ctx.Response.E400("Juridinis asmuo nerastas");
							else if (ja.StatusKodas == 10) await ctx.Response.E400("Juridinis asmuo išregistruotas");
							else {

								using var db = new DBRead("SELECT * FROM g9.v_api_gvts WHERE id=@id;", Conn.G9, ("@id", id));
								var ret = await db.GetObject<GvtsDetails>();

								//TODO: UPDATE JAR

								var param = new (string key, object? val)[] { ("@id", id), ("@title", dt.Pavad), ("@ja", dt.JA), ("@gvtot", dt.GVTOT), ("@aob", dt.Adresas), ("@apg", adr.Apg?.ID), ("@sav", adr.Sav.ID), ("@saviv", adr.Sav.Vardas), ("@adr", $"{adr.Pavad}, {adr.Vietove}"), ("@act", dt.Active) };

								if (ret is not null) {
									await Conn.G9!.Execute("UPDATE g9.gvts SET vkl_ja=@ja, vkl_title=@title, vkl_saviv=@saviv, vkl_adresas=@adr, vkl_gvtot=@gvtot, vkl_active=@act, vkl_adr_aob=@aob, vkl_adr_sav=@sav, vkl_date=timezone('utc',now()), vkl_adr_apg=@apg WHERE vkl_id=@id;", param);
								}
								else {
									await Conn.G9!.Execute("INSERT INTO g9.gvts(vkl_id,vkl_ja,vkl_title,vkl_saviv,vkl_adresas,vkl_gvtot,vkl_active,vkl_adr_aob,vkl_adr_sav,vkl_date,vkl_adr_apg) VALUES (@id, @ja, @title, @saviv, @adr, @gvtot, @act, @aob, @sav, timezone('utc',now()), @apg);", param);
								}
								await ctx.Response.WriteAsync("Ok");
							}

						}

					}

				}
			}
			else await ctx.Response.E400();
		}
		else await ctx.Response.E401();
	}

}