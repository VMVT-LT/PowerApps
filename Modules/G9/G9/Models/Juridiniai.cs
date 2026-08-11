

namespace PowerApps.Modules.G9.Models;

/// <summary>Juridinių asmenų paieškos įrašo informacijos modelis</summary>
public class JARFind {
	/// <summary>Juridinio asmens registro kodas</summary>
	public int? ID { get; set; }
	/// <summary>Pavadinimas</summary>
	public string? Pavad { get; set; }
	/// <summary>Adresas</summary>
	public string? Adresas { get; set; }
	/// <summary>Juridinio asmens statusas</summary>
	public string? Statusas { get; set; }
	/// <summary>Juridinio asmens forma</summary>
	public string? Forma { get; set; }
}

/// <summary>Juridinio asmens detalės (RC)</summary>
public class JARItem {
	/// <summary>Juridinio asmens registro kodas</summary>
	public int? ID { get; set; }
	/// <summary>Pavadinimas</summary>
	public string? Pavad { get; set; }
	/// <summary>Adresas</summary>
	public string? Adresas { get; set; }
	/// <summary>Adresų registro kodas (AOB)</summary>
	public long? AobKodas { get; set; }
	/// <summary>Juridinio asmens forma</summary>
	public string? Forma { get; set; }
	/// <summary>Juridinio asmens statusas</summary>
	public string? Statusas { get; set; }
	/// <summary>Juridinio asmens statusas</summary>
	public int? StatusKodas { get; set; }
	/// <summary>Registracijos data</summary>
	public DateOnly? RegData { get; set; }
	/// <summary>Išregistravimo data</summary>
	public DateOnly? IsregData { get; set; }
	/// <summary>Keitimo data</summary>
	public DateOnly? ModData { get; set; }
	/// <summary>Keitimo data</summary>
	public DateOnly? AobData { get; set; }
}


/// <summary>Juridinio asmens detalės</summary>
public class JARDetails {
	/// <summary>JAR identifikatorius</summary>
	public long ID { get; set; }
	/// <summary>Pavadinimas</summary>
	public string? Pavad { get; set; }
	/// <summary>Forma</summary>
	public string? Tipas { get; set; }
	/// <summary>Statusas</summary>
	public string? Statusas { get; set; }
	/// <summary>Adreso detalės</summary>
	public JARDtlAdr? Adresas { get; set; }
	/// <summary>Kontaktinis asmuo</summary>
	public JARDtlKont? Kontaktas { get; set; }
	/// <summary>Paskutinio keitimo data</summary>
	public DateTime? Pakeista { get; set; }
}

/// <summary>Juridinio asmens adreso detalės</summary>
public class JARDtlAdr {
	/// <summary>JAR adreso kodas</summary>
	public long? Aob { get; set; }
	/// <summary>Adreso pavadinimas</summary>
	public string? Pavad { get; set; }
	/// <summary>Pakeistas adreso kodas</summary>
	public long? Kita { get; set; }
	/// <summary>Savivaldybė</summary>
	public long? Sav { get; set; }
	/// <summary>Apygarda</summary>
	public long? Apg { get; set; }
}

/// <summary>Juridinio asmens kontakai</summary>
public class JARDtlKont {
	/// <summary>Vardas</summary>
	public string? Vardas { get; set; }
	/// <summary>Pavardė</summary>
	public string? Pavarde { get; set; }
	/// <summary>Telefono numeris</summary>
	public string? Phone { get; set; }
	/// <summary>El. pašto adresas </summary>
	public string? Email { get; set; }
}


/// <summary>Juridinio asmens detalių keitimas</summary>
public class JARDtlSet {
	/// <summary>Pakeistas adresas</summary>
	public long? Aob { get; set; }
	/// <summary>Juridinio asmens kontakto detalės</summary>
	public JARDtlKont? Kontaktas { get; set; }
}