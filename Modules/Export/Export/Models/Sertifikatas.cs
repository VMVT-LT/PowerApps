
namespace PowerApps.Modules.Export.Models;

/// <summary>Pagrindinė vartotojo informacija</summary>
public class Export_UserBase {
	/// <summary>Vartotojo kodas (EntraID)</summary>
	public Guid ID { get; set; }
	/// <summary>Pilnas vardas</summary>
	public string? Vardas { get; set; }
	/// <summary>El. Paštas</summary>
	public string? Email { get; set; }
}

/// <summary>Vartotojas</summary>
public class Export_User {
	/// <summary>Apydardos pavadinimas</summary>
	public string? Dept { get; set; }
}


/// <summary>Sertifikato vartotojai</summary>
public class Cert_user : Export_UserBase {
	/// <summary>Vartotojas yra sertifikato autorius</summary>
	public bool Autorius { get; set; }
}