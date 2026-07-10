
using Microsoft.AspNetCore.Http;
using Vmvt.RouteAPI;
namespace PowerApps.Modules.G9.Methods;


/// <summary>Deklaracijos duomenų metodai</summary>
public static class Deklar {
	/// <summary>Gauti deklaracijos duomenis</summary>
	/// <param name="ctx"></param><returns></returns>
	public static async Task Get(HttpContext ctx) {
		if (ctx.CheckApi()) {
			await ctx.Response.E404();
		}
		else await ctx.Response.E401();
	}
}