using Microsoft.AspNetCore.Http;
using PowerApps.Modules.G9.Methods;
using PowerApps.Modules.G9.Models;
using Vmvt.RouteAPI;

namespace PowerApps.Modules;

/// <summary>Registro inicijavimas</summary>
public class G9API {
	/// <summary>API prieigos raktai</summary>
	public static List<string> ApiKeys { get; set; } = [];

	/// <summary>Export maršrutų priskyrimas</summary>
	/// <returns></returns>
	public static RouteDefinition Route() => new("G9") {
		Description = "G9-app duomenų integracija",
		Tag = "g9", Version = "v1",
		Routes = [
			new RouteGroup("Deklaracijos duomenų gavimas")
				.Map(new("/api/g9/deklaracija", Deklar.Get) {
					Description = "Deklaracijos duomenų gavimas pagal identifikacinį numerį", Response=typeof(Deklaracija), Code = "GetDeklar",
					Params = [
						new("id"){ Description="Įrašo identifikacinis numeris", Type=RouteParamType.Integer, Required=true },
						new("X-API-Key") { Description = "Prieigos raktas", Type=RouteParamType.String, Location=RouteParamLoc.Header, Required=true }
						],
				})
			],
	};
}

