using Vmvt.RouteAPI;


namespace PowerApps.Shared;

public static partial class Extensions {

	private static readonly char[] MkSrhExclude = ['-'];
	/// <summary>Paieškos teksto generavimas</summary>
	/// <param name="q"></param>
	/// <returns></returns>
	public static string? MkSerach(this string? q) => q?.RemoveAccents().RemoveNonAlphanumeric(MkSrhExclude).ToLower();

}
