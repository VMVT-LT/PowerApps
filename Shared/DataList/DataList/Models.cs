

namespace PowerApps.Shared.DataList;

/// <summary>Sąrašo užklausa</summary>
public class ListQuery<T> {
	/// <summary>Puslapis</summary>
	/// <example>1</example>
	public int Page { get; set; } = 1;
	/// <summary>Įrašų skaičius</summary>
	/// <example>20</example>
	public int Limit { get; set; } = 100;
	/// <summary>Filtras</summary>
	public T? Filter { get; set; }
	/// <summary>Įrašų rikiavimas</summary>
	/// <example>["cert_isdave","cert_id desc"]</example>
	public List<string>? Sort { get; set; }
}



