using Microsoft.AspNetCore.Http;
using PowerApps.Modules.G9.Models;
using PowerApps.Shared;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Vmvt.Npgsql;
using Vmvt.RouteAPI;

namespace PowerApps.Modules.G9.Methods;

/// <summary>Deklaracijos duomenų metodai</summary>
public static class Jar {
	private static HttpClient HClient { get; } = new() { BaseAddress = new(G9API.Cfg.Base) };

	/// <summary>G9 Juridiniai asmenys</summary>
	/// <param name="ctx"></param><returns></returns>
	public static async Task List(HttpContext ctx) {
		if (ctx.CheckApi()) {
			var sql = $"SELECT * FROM g9.v_api_ja;";
			using var db = new DBRead(sql, Conn.G9);
			var ret = await db.GetList<JARDetails>();
			if (ret is not null) await ctx.Response.WriteAsJsonAsync(ret);
			else await ctx.Response.E404();
		}
		else await ctx.Response.E401();
	}


	/// <summary>G9 Juridinio asmens detalės</summary>
	/// <param name="ctx"></param><returns></returns>
	public static async Task Info(HttpContext ctx) {
		if (ctx.CheckApi()) {
			var id = ctx.ParamLongN("id");
			if (id is not null) {
				var sql = $"SELECT * FROM g9.v_api_ja WHERE id=@id;";
				using var db = new DBRead(sql, Conn.G9, ("@id", id));
				var ret = await db.GetObject<JARDetails>();
				if (ret is not null) await ctx.Response.WriteAsJsonAsync(ret);
				else await ctx.Response.E404();
			}
			else await ctx.Response.E400("Nenurodytas įrašo ID");
		}
		else await ctx.Response.E401();
	}

	/// <summary>G9 Juridinio asmens detalių keitimas</summary>
	/// <param name="ctx"></param><returns></returns>
	public static async Task InfoSet(HttpContext ctx, JARDtlSet dt) {
		if (ctx.CheckApi()) {
			var id = ctx.ParamLongN("id");
			var usr = ctx.ParamStringN("u");
			//TODO: validate user
			if (usr is not null) {
				if (id is not null) {
					if (dt is not null) {
						var ja = await GetJar(id.Value);
						if (ja is null) { await ctx.Response.E400("Nerastas juridinis asmuo"); return; }
						if (ja.Statusas == "Teisinis statusas neįregistruotas") ja.Statusas = null;
						var aob = dt.Aob ?? ja.AobKodas ?? 0;
						if (aob < 0) { await ctx.Response.E400("Adresas privalomas"); return; }
						var adr = await Adr.GetAdr(aob);
						if (adr is null) { await ctx.Response.E400("Nerastas juridinio asmens adresas"); return; }
						var adpav = $"{adr.Pavad}, {adr.Vietove}";
						var sql = "UPDATE g9.ja_detales SET ja_pavadinimas=@pavad, ja_tipas=@tipas, ja_statusas=@status, ja_aob=@jaob, jad_kontaktas_vardas=@vardas, jad_kontaktas_pavarde=@pavard, " +
							"jad_kontaktas_email=@email, jad_kontaktas_phone=@phone, jad_aob=@aob, jad_adresas=@adr, jad_adr_sav=@sav, jad_adr_apg=@apg," +
							"jad_date=timezone('utc',now()), jad_update=timezone('utc',now()) WHERE ja_id=@id;";
					
						await Conn.G9!.Execute(sql, ("@id", id), ("@pavad", ja.Pavad), ("@tipas", ja.Forma), ("@status", ja.Statusas), ("@jaob", ja.AobKodas), ("@vardas", dt.Kontaktas?.Vardas), ("@pavard", dt.Kontaktas?.Pavarde),
							("@email", dt.Kontaktas?.Email), ("@phone", dt.Kontaktas?.Phone), ("@aob", dt.Aob), ("@adr", adpav), ("@apg", adr.Apg?.ID), ("@sav", adr.Sav?.ID));

						using var db = new DBRead("SELECT * FROM g9.v_api_ja WHERE id=@id;", Conn.G9, ("@id", id));
						var ret = await db.GetObject<JARDetails>();
						if (ret is not null) await ctx.Response.WriteAsJsonAsync(ret);
						else await ctx.Response.E404();
					}
					else await ctx.Response.E400("Netinkama užklausa");
				}
				else await ctx.Response.E400("Nenurodytas įrašo ID");
			}
			else await ctx.Response.E400("Neatpažintas vartotojas");
		}
		else await ctx.Response.E401();
	}


	/// <summary>Juridinio asmens paieška</summary>
	/// <param name="ctx"></param><returns></returns>
	public static async Task Find(HttpContext ctx) {
		var q = ctx.ParamString("q");
		var top = ctx.ParamInt("top", 10);
		using var response = await HClient.GetAsync($"{G9API.Cfg.JarFind}?top={top}&details=true&active=true&q={Uri.EscapeDataString(q)}", HttpCompletionOption.ResponseHeadersRead);
		ctx.Response.ContentType = "application/json; charset=utf-8";
		if (response.IsSuccessStatusCode) {
			using var stream = await response.Content.ReadAsStreamAsync();
			await stream.CopyToAsync(ctx.Response.Body);
		}
		else await ctx.Response.E400("Klaida gaunant juridinius asmenis");
	}

	/// <summary>Juridinio asmens informacija</summary>
	/// <param name="ctx"></param>
	public static async Task Details(HttpContext ctx) {
		var id = ctx.ParamLong("id");
		using var response = await HClient.GetAsync($"{G9API.Cfg.JarDetails}?id={id}", HttpCompletionOption.ResponseHeadersRead);
		ctx.Response.ContentType = "application/json; charset=utf-8";
		if (response.IsSuccessStatusCode) {
			using var stream = await response.Content.ReadAsStreamAsync();
			await stream.CopyToAsync(ctx.Response.Body);
		}
		else await ctx.Response.E400("Klaida gaunant juridinį asmenį");
	}

	/// <summary>Gauti juridinio asmens detales</summary>
	/// <param name="id">JAR kodas</param><returns></returns>
	public static async Task<JARItem?> GetJar(long id) {
		using var response = await HClient.GetAsync($"{G9API.Cfg.JarDetails}?id={id}");
		if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<JARItem>();
		return null;
	}

	/// <summary>Juridinių asmenų informacijos atnaujinimas</summary>
	/// <param name="ctx"></param><returns></returns>
	public static async Task AdmUpdateJar(HttpContext ctx) {
		await UpdateJar();
		await ctx.Response.WriteAsync("Ok");
	}

	/// <summary>Juridinių asmenų informacijos atnaujinimas</summary>
	public static async Task UpdateJar() {
		using var dbr = new DBRead("SELECT ja_id FROM g9.ja_detales WHERE jad_active;", Conn.G9);
		using var rdr = await dbr.GetReader();
		while (await rdr.ReadAsync()) await UpdateJar(rdr.GetInt64(0));
	}

	/// <summary>Juridinio asmens informacijos atnaujinimas</summary>
	/// <param name="ja">Juridinio asmens kodas</param>
	public static async Task UpdateJar(long ja) {
		using var db = new DBRead("SELECT ja_id, ja_aob, jad_aob, jad_adresas, jad_update FROM g9.ja_detales WHERE ja_id=@id;", Conn.G9, ("@id", ja));
		using var rdr = await db.GetReader();
		if (await rdr.ReadAsync()) {
			var jar = await GetJar(ja);
			if (jar is null) await Conn.G9!.Execute("UPDATE g9.ja_detales SET ja_statusas='Neegzistuojantis' WHERE ja_id=@id", ("@id", ja));
			else {
				long? caob = null;
				string? adr = rdr.GetStringN(3);
				if (jar.AobKodas > 0) {
					caob = rdr.GetLongN(2);
					if (caob > 0 && rdr.GetDateOnlyN(4) < jar.AobData) caob = null;

				}
				if (jar.Statusas == "Teisinis statusas neįregistruotas") jar.Statusas = null;

				var aob = caob ?? jar.AobKodas;
				var ar = aob is null ? null : await Adr.GetAdr(aob.Value);
				if (ar is not null) adr = $"{ar.Pavad}, {ar.Vietove}";

				adr ??= jar.Adresas;

				var sql = "UPDATE g9.ja_detales SET ja_pavadinimas=@pavad, ja_tipas=@tipas, ja_statusas=@status, ja_aob=@jaob, jad_aob=@aob, jad_adresas=@adr, jad_adr_sav=@sav, jad_adr_apg=@apg, jad_date=timezone('utc',now()), jad_update=timezone('utc',now()) WHERE ja_id=@id;";
				await Conn.G9!.Execute(sql, ("@id", ja), ("@pavad", jar.Pavad), ("@tipas", jar.Forma), ("@status", jar.Statusas), ("@jaob", jar.AobKodas), ("@aob", caob), ("@adr", adr), ("@apg", ar?.Apg?.ID), ("@sav", ar?.Sav?.ID));

			}
		}
	}
}


