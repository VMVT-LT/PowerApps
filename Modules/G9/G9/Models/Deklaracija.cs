
namespace PowerApps.Modules.G9.Models;

/// <summary>Vandens deklaracijos įrašo duomenys.</summary>
public class Deklaracija {
	/// <summary>Unikalus deklaracijos ID.</summary>
	public int ID { get; set; }
	/// <summary>Vandenvietės ar GVTS kodas.</summary>
	public int GVTS { get; set; }
	/// <summary>Deklaracijos metai.</summary>
	public int Metai { get; set; }
	/// <summary>Deklaracijos statusas.</summary>
	public string? Statusas { get; set; }
	/// <summary>Stebėsenos tipas.</summary>
	public string? Stebesena { get; set; }
	/// <summary>Vandens kiekis (m³).</summary>
	public double VandensKiekis { get; set; }
	/// <summary>Vartotojų skaičius.</summary>
	public int? Vartotojai { get; set; }
	/// <summary>Vandens ruošimo informacija.</summary>
	public Ruosimas? Ruosimas { get; set; }
	/// <summary>Suvestos rodiklių reikšmės</summary>
	public Kontaktas? Kontaktas { get; set; }
	/// <summary>Paskutinio keitimo data.</summary>
	public DateTime Keitimas { get; set; }
	/// <summary>Paskutinis keitęs naudotojas.</summary>
	public string? Keite { get; set; }
	/// <summary>Pateikimo data.</summary>
	public DateTime? Pateiktas { get; set; }
	/// <summary>Pateikęs naudotojas.</summary>
	public string? Pateike { get; set; }
}

/// <summary>Vandens ruošimo medžiagos ir būdai.</summary>
public class Ruosimas {
	/// <summary>Naudojamų medžiagų sąrašas.</summary>
	public List<string>? Medziagos { get; set; }
	/// <summary>Ruošimo būdų sąrašas.</summary>
	public List<string>? Budai { get; set; }
}

/// <summary>Kontaktinio asmens informacija.</summary>
public class Kontaktas {
	/// <summary>Vardas.</summary>
	public string? Vardas { get; set; }
	/// <summary>Pavardė.</summary>
	public string? Pavarde { get; set; }
	/// <summary>El. paštas.</summary>
	public string? Email { get; set; }
	/// <summary>Telefonas.</summary>
	public string? Phone { get; set; }
}

/// <summary>Suvestos reiksmės informacija</summary>
public class DklRodiklis {
	/// <summary>Rodiklio ID</summary>
	public int ID { get; set; }
	/// <summary>Rodiklio kodas</summary>
	public string? Kodas { get; set; }
	/// <summary>Grupė</summary>
	public string? Grupe { get; set; }
	/// <summary>Pavadinimas</summary>
	public string? Rodiklis { get; set; }
	/// <summary>Viršyjimas</summary>
	public int Virsija { get; set; }
	/// <summary>Reikalaujamas suvedimų skaičius</summary>
	public int Reikia { get; set; }
	/// <summary>Suvestos reikšmės</summary>
	public int Suvesta { get; set; }
}

/// <summary>Suvestos rodiklio reikšmės</summary>
public class DklReiksmes {
	/// <summary>Reikšmės ID</summary>
	public long ID { get; set; }
	/// <summary>Mėginio imimo data</summary>
	public DateOnly? Data { get; set; }
	/// <summary>Reikšmė</summary>
	public double Reiksme { get; set; }
	/// <summary>Suvedimo ID</summary>
	public long? Suvedimas { get; set; }
	/// <summary>Mažiau arba neaptikta</summary>
	public bool Maziau { get; set; }
	/// <summary>Protokolo numeris</summary>
	public string? Protokolas { get; set; }
}