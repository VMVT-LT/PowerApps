namespace PowerApps.Modules.G9.Models;


using System.Text.Json.Serialization;

/// <summary>Klasifikatorių sąrašas</summary>
public class LookupResponse {
	/// <summary>Daznumo reikšmių sąrašas.</summary>
	public List<LookupStrItem>? Daznumas { get; set; }
	/// <summary>Daznumo laiko reikšmių sąrašas.</summary>
	public List<LkpDaznLaikItem>? DaznumoLaikas { get; set; }
	/// <summary>Rodiklių grupės reikšmių sąrašas.</summary>
	public List<LkpRodGrpItem>? RodikliuGrupe { get; set; }
	/// <summary>Ruosimo būdų reikšmių sąrašas.</summary>
	public List<LookupIntItem>? RuosimoBudai { get; set; }
	/// <summary>Ruosimo dažnumo reikšmių sąrašas.</summary>
	public List<LkpRuosDaznItem>? RuosimoDaznumas { get; set; }
	/// <summary>Ruosimo medžiagų reikšmių sąrašas.</summary>
	public List<LookupIntItem>? RuosimoMedziagos { get; set; }
	/// <summary>Statuso reikšmių sąrašas.</summary>
	public List<LookupIntItem>? Statusas { get; set; }
	/// <summary>Stebėjimo statuso reikšmių sąrašas.</summary>
	public List<LookupIntItem>? StebejimoStatusas { get; set; }
	/// <summary>Stebėsenos reikšmių sąrašas.</summary>
	public List<LookupIntItem>? Stebesenos { get; set; }
	/// <summary>Suvedimo tipo reikšmių sąrašas.</summary>
	public List<LookupIntItem>? SuvedimoTipas { get; set; }
	/// <summary>Vietos tipo reikšmių sąrašas.</summary>
	public List<LookupIntItem>? VietosTipas { get; set; }
	/// <summary>Viršijimo priežasčių reikšmių sąrašas.</summary>
	public List<LookupIntItem>? VirsPriezastis { get; set; }
	/// <summary>Viršijimo taisomųjų veiksmų reikšmių sąrašas.</summary>
	public List<LookupStrItem>? VirsTaisomasisVeiksmas { get; set; }
}

/// <summary>Tekstinio klasifikatoriaus reikšmė.</summary>
public class LookupStrItem {
	/// <summary>Unikalus raktas.</summary>
	public string? Key { get; set; }
	/// <summary>Reikšmė.</summary>
	public string? Val { get; set; }
}
/// <summary>Klasifikatoriaus reikšmė.</summary>
public class LookupIntItem {
	/// <summary>Unikalus raktas.</summary>
	public int? Key { get; set; }
	/// <summary>Reikšmė.</summary>
	public string? Val { get; set; }
}

/// <summary>Daznumo laiko reikšmė.</summary>
public class LkpDaznLaikItem : LookupIntItem {
	/// <summary>Papildomas skaitmeninis laukas (num).</summary>
	public int? Num { get; set; }
}


/// <summary>Rodiklių grupės reikšmė.</summary>
public class LkpRodGrpItem : LookupIntItem {
	/// <summary>Reikalingos papildomos viršyjimo detalės.</summary>
	public bool? Pas { get; set; }
}

/// <summary>Ruosimo dažnumo reikšmė.</summary>
public class LkpRuosDaznItem {
	/// <summary>Rodiklio numeris.</summary>
	public int Rod { get; set; }
	/// <summary>Būdo tekstinė reikšmė.</summary>
	public string? Bud { get; set; }
	/// <summary>Paruošimo skaitmeninė reikšmė.</summary>
	public int? Ruos { get; set; }
}