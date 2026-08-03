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

/// <summary>Juridinio asmens detalės</summary>
public class JARItem {
	/// <summary>Juridinio asmens registro kodas</summary>
	public int? ID { get; set; }
	/// <summary>Pavadinimas</summary>
	public string? Pavad { get; set; }
	/// <summary>Adresas</summary>
	public string? Adresas { get; set; }
	/// <summary>Adresų registro kodas (AOB)</summary>
	public int? AobKodas { get; set; }
	/// <summary>Juridinio asmens forma</summary>
	public string? Forma { get; set; }
	/// <summary>Juridinio asmens statusas</summary>
	public string? Statusas { get; set; }
	/// <summary>Registracijos data</summary>
	public DateOnly? RegData { get; set; }
	/// <summary>Išregistravimo data</summary>
	public DateOnly? IsregData { get; set; }
}