using Microsoft.AspNetCore.Http;
using PowerApps.Modules.G9.Models;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Vmvt.RouteAPI;

namespace PowerApps.Modules.G9.Methods;

/// <summary>Deklaracijos duomenų metodai</summary>
public static class Adr {
	private static HttpClient HClient { get; } = new() { BaseAddress = new(G9API.Cfg.Base) };

	/// <summary>Gauti savivaldybių duomenis</summary>
	/// <param name="ctx"></param><returns></returns>
	public static async Task List(HttpContext ctx) {
		await ctx.Response.WriteAsJsonAsync(new ARList() {
			Savivaldybes = (await HClient.GetFromJsonAsync<RegData<ARSavivaldybe>>(G9API.Cfg.ARSav))?.Data,
			Apygardos = (await HClient.GetFromJsonAsync<RegData<ARApygarda>>(G9API.Cfg.ARApg))?.Data,
			Apskritys = (await HClient.GetFromJsonAsync<RegData<ARApskritis>>(G9API.Cfg.ARAdm))?.Data
		});
	}
	/// <summary>Gauti adreso detales</summary>
	/// <param name="ctx"></param><returns></returns>
	public static async Task Details(HttpContext ctx) {
		var id = ctx.ParamInt("id");
		using var response = await HClient.GetAsync($"{G9API.Cfg.ARDetails}?id={id}&details=true");

		if (response.IsSuccessStatusCode) {
			var dt = await response.Content.ReadFromJsonAsync<ARDetales>();
			if (dt is not null) await ctx.Response.WriteAsJsonAsync(dt);
			else await ctx.Response.E400("Klaida gaunant adresus");
		}
		else if (response.StatusCode == HttpStatusCode.NotFound) 
			await ctx.Response.E404("Adresas nerastas");
		else await ctx.Response.E400("Klaida gaunant adresus");
	}

	/// <summary>Gyvenvietės paieška</summary>
	/// <param name="ctx"></param><returns></returns>
	public static async Task FindGyv(HttpContext ctx) {
		var q = ctx.ParamString("q");
		var top = ctx.ParamInt("top", 10);
		using var response = await HClient.GetAsync($"{G9API.Cfg.ARFindGyv}?top={top}&q={Uri.EscapeDataString(q)}", HttpCompletionOption.ResponseHeadersRead);
		ctx.Response.ContentType = "application/json; charset=utf-8";
		if (response.IsSuccessStatusCode) {
			using var stream = await response.Content.ReadAsStreamAsync();
			await stream.CopyToAsync(ctx.Response.Body);
		}
		else await ctx.Response.E400("Klaida gaunant adresus");
	}

	/// <summary>Adreso paieška</summary>
	/// <param name="ctx"></param>
	public static async Task FindAdr(HttpContext ctx) {
		var q = ctx.ParamString("q");
		var gyv = ctx.ParamLong("gyv");
		var top = ctx.ParamInt("top",10);
		using var response = await HClient.GetAsync($"{G9API.Cfg.ARFindAdr}?gyv={gyv}&top={top}&q={Uri.EscapeDataString(q)}", HttpCompletionOption.ResponseHeadersRead);
		ctx.Response.ContentType = "application/json; charset=utf-8";
		if (response.IsSuccessStatusCode) {
			using var stream = await response.Content.ReadAsStreamAsync();
			await stream.CopyToAsync(ctx.Response.Body);
		}
		else await ctx.Response.E400("Klaida gaunant adresus");
	}

	/// <summary>Gauti adreso detales</summary>
	/// <param name="id">Adreso AOB kodas</param><returns></returns>
	public static async Task<ARDetales?> GetAdr(long id) {
		using var response = await HClient.GetAsync($"{G9API.Cfg.ARDetails}?id={id}&details=true");
		if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<ARDetales>();
		return null;
	}
}