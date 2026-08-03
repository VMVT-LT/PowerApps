using Microsoft.AspNetCore.Http;
using PowerApps.Modules.G9.Models;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Vmvt.RouteAPI;

namespace PowerApps.Modules.G9.Methods;

/// <summary>Deklaracijos duomenų metodai</summary>
public static class Jar {
	private static HttpClient HClient { get; } = new() { BaseAddress = new(G9API.Cfg.Base) };

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
}