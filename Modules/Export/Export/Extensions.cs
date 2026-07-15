using Microsoft.AspNetCore.Http;

namespace PowerApps.Modules.Export;



/// <summary>Plėtiniai</summary>
public static class Extensions {


	/// <summary>Gauti užklausos teises</summary>
	/// <param name="ctx"></param>
	/// <returns></returns>
	public static bool CheckApi(this HttpContext ctx) =>
		ctx.Request.Headers.TryGetValue("X-API-Key", out var k) && !string.IsNullOrWhiteSpace(k) && ExportAPI.ApiKeys.Contains(k.ToString());

}

