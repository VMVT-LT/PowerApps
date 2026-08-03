
using System.Text.Json.Serialization;

namespace PowerApps.Modules.G9.Models;


/// <summary>Ardesų registro sąrašas</summary>
public class ARList {
	/// <summary>Visos apygardos</summary>
	public List<ARApygarda>? Apygardos { get; set; }
	/// <summary>Visos Apskritys</summary>
	public List<ARApskritis>? Apskritys { get; set; }
	/// <summary>Visos savivaldybės</summary>
	public List<ARSavivaldybe>? Savivaldybes { get; set; }
}

/// <summary>Adresų registro paieškos rezultatas</summary>
public class ARFind {
	/// <summary>Adresų registro kodas</summary>
	public int? ID { get; set; }
	/// <summary>Pavadinimas</summary>
	public string? Pavad { get; set; }
	/// <summary>Vietovės pavadinimas</summary>
	public string? Vietove { get; set; }
	/// <summary>Įrašo tipas</summary>
	public string? Tipas { get; set; }
}

/// <summary>Bazinė AR klasė</summary>
public class ARBase {
	/// <summary>Įrašo ID</summary>
	public int ID { get; set; }
	/// <summary>Įrašo pavadinimas</summary>
	public string? Vardas { get; set; }
}


/// <summary>Apygardos informacija</summary>
public class ARApygarda : ARBase { }
/// <summary>Apygardos informacija</summary>
public class ARApskritis : ARBase { }


/// <summary>Savivaldybės informacija</summary>
public class ARSavivaldybe : ARBase {
	/// <summary>Apskrities ID</summary>
	public int Adm { get; set; }
	/// <summary>Apygardos ID</summary>
	public int Apg { get; set; }
}


public class ARDetales : ARBase {
	public string? Pavad { get; set; }
	public string? Vietove { get; set; }
	public string? Tipas { get; set; }
	public string? Trump { get; set; }
	public string? Nr { get; set; }
	public string? Pat { get; set; }
	public string? Post { get; set; }
	public ARDetales? Adm { get; set; }
	public ARDetales? Sav { get; set; }
	public ARDetales? Gyv { get; set; }
	public ARDetales? Gat { get; set; }
	public ARDetales? Aob { get; set; }
	public ARDetales? Apg { get; set; }
}


/// <summary>Registrų savivaldybės užklausos atsakas</summary>
public class RegData<T> {
	/// <summary>Savivaldybių duomenys</summary>
	public List<T>? Data { get; set; }
}