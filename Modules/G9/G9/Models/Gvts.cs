

using System.Text.Json.Serialization;

namespace PowerApps.Modules.G9.Models;




public class GvtsItemBase {
	public long? ID { get; set; }
	public string? Pavad { get; set; }
	public string? GVTOT { get; set; }
	public GvtsAdr? Adresas { get; set; }
	/// <summary>GVTS statusas</summary>
	public bool Active { get; set; } = true;
}

/// <summary>Gvts informacija</summary>
public class GvtsItem : GvtsItemBase {
	/// <summary>Juridinio asmens detalės</summary>
	public JARDetails? JA { get; set; }
	/// <summary>Paskutinio keitimo data</summary>
	public DateTime? Pakeista { get; set; }
	/// <summary>Inspektoriai</summary>
	public List<User>? Inspektoriai { get; set; }
}



public class GvtsList {
	public List<JARDetails>? Subjektai { get; set; }
	public List<GvtsListItem>? GVTS { get; set; }
	public List<User>? Inspektoriai { get; set; }
	public int Count => GVTS?.Count ?? 0;
}

/// <summary>Gvts informacija sąrašams</summary>
public class GvtsListItem : GvtsItemBase {
	/// <summary>Juridinio asmens kodas</summary>
	public long? JA { get; set; }
	/// <summary>Inspektoriai</summary>
	public List<Guid>? Inspektoriai { get; set; }
	/// <summary>Paskutinio keitimo data</summary>
	[JsonIgnore] public DateTime? Pakeista { get; set; }
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