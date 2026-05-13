using Microsoft.AspNetCore.Http;
using Vmvt.RouteAPI;

namespace PowerApps.Modules.Export;

/// <summary>Export pagrindiniai metodai</summary>
public static partial class Export {



	/// <summary>Gauti užklausos teises</summary>
	/// <param name="ctx"></param>
	/// <returns></returns>
	public static bool CheckApi(this HttpContext ctx) =>
		ctx.Request.Headers.TryGetValue("X-API-Key", out var k) && !string.IsNullOrWhiteSpace(k) && ExportAPI.ApiKeys.Contains(k.ToString());


	private static readonly char[] MkSrhExclude = ['-'];
	/// <summary>Paieškos teksto generavimas</summary>
	/// <param name="q"></param>
	/// <returns></returns>
	public static string? MkSerach(this string? q) => q?.RemoveAccents().RemoveNonAlphanumeric(MkSrhExclude).ToLower();

}

