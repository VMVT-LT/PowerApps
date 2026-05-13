using System.Text.Json.Serialization;
using Vmvt.Npgsql;

namespace PowerApps.Modules.Export.Models;


/// <summary>Sertifikatų sąrašo rezultatas</summary>
public class List_Cert : DBPagingResponse<List_Cert_Item> { }

/// <summary>Sertifikatų sąrašo įrašo modelis</summary>
public class List_Cert_Item {
	/// <summary>Sertifikato ID</summary>
	/// <example>100</example>
	public int Cert_id { get; set; }
	/// <summary>Sertifikato numeris</summary>
	/// <example>EXP25000000</example>
	public string? Cert_nr { get; set; }
	/// <summary>Produkto KPN kodai 8 skaičiai</summary>
	/// <example>["29012300","29012400"]</example>
	public List<string>? Prod_kpn { get; set; }
	/// <summary>Produktų susijusių su konkrečiu sertfikatu skaičius</summary>  
	/// <example>4</example>
	public int Prod_count { get; set; }
	/// <summary>Exportuotojo pavadinimas</summary>
	/// <example>UAB Eksportuotojas</example>
	public string? Cert_export { get; set; }
	/// <summary>Sertifikato statusas</summary>
	/// <example>Pildoma</example>
	public string? Cert_status { get; set; }
	/// <summary>Blanko numeris</summary>
	/// <example>LT1234565</example>
	public string? Cert_blankas { get; set; }
	/// <summary>Export-in numeris</summary>
	/// <example>1</example>
	public int? Cert_request { get; set; }
	/// <summary>Importuojanti šalis</summary>
	/// <example>Ukraina</example>
	public string? Cert_imp_salis { get; set; }
	/// <summary>Sertifikatą išdavusio inspektoriaus vardas pavardė</summary>
	/// <example>Vardenis Pavardenis</example>
	public string? Cert_isdave_name { get; set; }
	/// <summary>Sertifikato sukūrimo data</summary>
	/// <example>2025-12-01</example>
	public DateOnly? Cert_date_created { get; set; }
	/// <summary>Sertifikato išdavimo data</summary>
	/// <example>2025-12-01</example>
	public DateOnly? Cert_date_isdavimo { get; set; }
	/// <summary>Rizikos balas</summary>  
	/// <example>289.1</example>
	public float? Cert_rizikos_balas { get; set; }
	/// <summary>Sertifikatą sukūrusio departamento pavadinimas</summary>  
	/// <example>Didžiosios apygardos paslaugų skyrius</example>
	public string? Cert_created_user_dep { get; set; }
	/// <summary>Paskutinio komentaro data</summary>  
	/// <example>2025-01-01</example>
	public string? Log_date { get; set; }
	/// <summary>Komentaras</summary>  
	/// <example>null</example>
	public string? Log_comment { get; set; }
}


/// <summary>Sąrašo filtras</summary>
public class ListFilter_Sertifikatai {
	/// <summary>Vartotojo ID</summary>
	/// <example>null</example>
	[JsonPropertyName("user_id")] public Guid? User { get; set; }
	/// <summary>Vartotojo apygarda</summary>
	/// <example>null</example>
	[JsonPropertyName("cert_created_user_dep")] public string? Apygarda { get; set; }
	/// <summary>Sertifikato statusas</summary>
	/// <example>Išduotas</example>
	[JsonPropertyName("cert_status")] public string? Status { get; set; }
	/// <summary>Sertifikato tipas</summary>
	/// <example>1</example>
	[JsonPropertyName("cert_type")] public int? Type { get; set; }
	/// <summary>Sertifikatą išdavęs asmuo</summary>
	/// <example>null</example>
	[JsonPropertyName("cert_isdave")] public string? IsdaveUsr { get; set; }
	/// <summary>Sertifikatą išdavęs darbuotojas</summary>
	/// <example>null</example>
	[JsonPropertyName("cert_isdave_dep")] public string? IsdaveDep { get; set; }
	/// <summary></summary>
	/// <example>null</example>
	[JsonPropertyName("cert_imp_salis")] public string? Salis { get; set; }
	/// <summary></summary>
	/// <example>null</example>
	[JsonPropertyName("cert_export")] public int? Exportuotojas { get; set; }
	/// <summary></summary>
	/// <example>null</example>
	[JsonPropertyName("cert_postas")] public int? Postas { get; set; }
	/// <summary></summary>
	/// <example>null</example>
	[JsonPropertyName("cert_post_issued")] public int? PostIssue { get; set; }
	/// <summary></summary>
	/// <example>null</example>
	[JsonPropertyName("cert_warehouse")] public int? Warehouse { get; set; }
	/// <summary></summary>
	/// <example>false</example>
	[JsonPropertyName("cert_pakeistas")] public bool? Pakeistas { get; set; }
	/// <summary>Turi dokumentų</summary>
	/// <example>null</example>
	[JsonPropertyName("cert_file_count")] public bool? Files { get; set; }

	/// <summary>Sukūrimo data nuo</summary>
	/// <example>2025-01-01</example>
	[JsonPropertyName("date_created_nuo")] public DateOnly? DateSukurNuo { get; set; }
	/// <summary>Sukūrimo data iki</summary>
	/// <example>null</example>
	[JsonPropertyName("date_created_iki")] public DateOnly? DateSukurIki { get; set; }
	/// <summary>Išdavimo data nuo</summary>
	/// <example></example>
	[JsonPropertyName("date_isdavimo_nuo")] public DateOnly? DateIsdavNuo { get; set; }
	/// <summary>Išdabimo data iki</summary>
	/// <example>2027-01-01</example>
	[JsonPropertyName("date_isdavimo_iki")] public DateOnly? DateIsdavIki { get; set; }


	/// <summary>KPN kodas</summary>
	/// <example>null</example>
	[JsonPropertyName("prod_kpn")] public string? KPN { get; set; }

	/// <summary>Gamintojo ID</summary>
	/// <example>null</example>
	[JsonPropertyName("prod_gamintojas")] public int? Gamintojas { get; set; }


	/// <summary>Sertifikatas vėluoja</summary>
	/// <example>null</example>
	[JsonPropertyName("cert_veluoja")] public bool? Veluoja { get; set; }

	/// <summary>Tekstinė paieška</summary>
	/// <example>null</example>
	[JsonPropertyName("cert_search")] public string? Search { get; set; }
}