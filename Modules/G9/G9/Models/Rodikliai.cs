
namespace PowerApps.Modules.G9.Models;


/// <summary>Rodiklio informacija.</summary>
public class Rodiklis {
	/// <summary>Unikalus rodiklio identifikatorius.</summary>
	public int ID { get; set; }
	/// <summary>Rodiklio grupė.</summary>
	public int Grupe { get; set; }
	/// <summary>Rodiklio kodas.</summary>
	public string? Kodas { get; set; }
	/// <summary>Rodiklio pavadinimas.</summary>
	public string? Pavad { get; set; }
	/// <summary>Tyrimų dažnumas.</summary>
	public string? Daznumas { get; set; }
	/// <summary>Minimali rodiklio reikšmė.</summary>
	public double Min { get; set; }
	/// <summary>Maksimali rodiklio reikšmė.</summary>
	public double Max { get; set; }
	/// <summary>Reikšmės keitimo žingsnis.</summary>
	public double Step { get; set; }
	/// <summary>Skaitmenų skaičius po kablelio formatavimui.</summary>
	public int Decim { get; set; }
	/// <summary>Matavimo vienetai.</summary>
	public string? Vnt { get; set; }
	/// <summary>Rodiklio aprašas.</summary>
	public string? Apras { get; set; }
	/// <summary>Rodiklio aktyvumo būsena.</summary>
	public bool Active { get; set; } = true;
}


/// <summary>Rodiklio informacijos keitimas</summary>
public class RodDtlSet {
	/// <summary>Rodiklio grupė.</summary>
	public int Grupe { get; set; }
	/// <summary>Rodiklio pavadinimas.</summary>
	public string? Pavad { get; set; }
	/// <summary>Tyrimų dažnumas.</summary>
	public string? Daznumas { get; set; }
	/// <summary>Minimali rodiklio reikšmė.</summary>
	public double Min { get; set; }
	/// <summary>Maksimali rodiklio reikšmė.</summary>
	public double Max { get; set; }
	/// <summary>Reikšmės keitimo žingsnis.</summary>
	public double Step { get; set; }
	/// <summary>Skaitmenų skaičius po kablelio formatavimui.</summary>
	public int Decim { get; set; }
	/// <summary>Matavimo vienetai.</summary>
	public string? Vnt { get; set; }
	/// <summary>Rodiklio aprašas.</summary>
	public string? Apras { get; set; }
	/// <summary>Rodiklio aktyvumo būsena.</summary>
	public bool Active { get; set; } = true;
}