

namespace PowerApps.Modules.G9.Models;



public class GvtsDetails {
	public long? ID { get; set; }
	public long? JA { get; set; }
	public string? Pavad { get; set; }
	public string? GVTOT { get; set; }
	public GvtsAdr? Adresas { get; set; }
	public DateTime? Pakeista { get; set; }
	/// <summary>GVTS statusas</summary>
	public bool Active { get; set; } = true;
}

public class GvtsAdr {
	public long? Aob { get; set; }
	public string? Pavad { get; set; }
	public long? Sav { get; set; }
	public long? Apg { get; set; }
}


public class GvtsDtlSet {
	/// <summary>Juridinio asmens kodas</summary>
	public long JA { get; set; }
	/// <summary>Pavadinimas</summary>
	public string? Pavad { get; set; }
	/// <summary>Teritorijos kodas</summary>
	public string? GVTOT { get; set; }
	/// <summary>Adreso kodas/summary>
	public long Adresas { get; set; }
	/// <summary>GVTS statusas</summary>
	public bool Active { get; set; } = true;
}