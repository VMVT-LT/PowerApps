

namespace PowerApps.Modules.G9.Models;



public class GvtsDetails {
	public long? ID { get; set; }
	public long? JA { get; set; }
	public string? Pavad { get; set; }
	public string? GVTOT { get; set; }
	public GvtsAdr? Adresas { get; set; }
	public DateTime? Pakeista { get; set; }
}

public class GvtsAdr {
	public long? Aob { get; set; }
	public string? Pavad { get; set; }
	public long? Sav { get; set; }
}