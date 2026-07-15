using PowerApps.Modules.Export.Methods;
using PowerApps.Modules.Export.Models;
using Vmvt.RouteAPI;

namespace PowerApps.Modules;

/// <summary>Registro inicijavimas</summary>
public class ExportAPI {
	/// <summary>API prieigos raktai</summary>
	public static List<string> ApiKeys { get; set; } = [];


	/// <summary>Export maršrutų priskyrimas</summary>
	/// <returns></returns>
	public static RouteDefinition Route() => new("Export") {
		Description = "Export-app duomenų integracija",
		Tag = "export", Version = "v1",
		Routes = [
			new RouteGroup("Duomenų sąrašai \"galerijoms\"")
				.Map(new("/api/export/lists/sertifikatai", ListSertifikatai.Filter) {  Code = "ListCerts",
					Description = "Filtruoto sertifikatų sąrašo gavimas puslapiais", Response=typeof(List_Cert), Method=Method.Post,
					Params = [new("X-API-Key") { Description = "Prieigos raktas", Type=RouteParamType.String, Location=RouteParamLoc.Header, Required=true }],
				}),
			new RouteGroup("Sertifikato duomenų gavimas")
				.Map(new("/api/export/list/{view}", GetData.List) {
					Description = "Duomenų gavimas pagal identifikacinį numerį", Response=typeof(List_Data), Code = "GetList",
					Params = [
						new("id"){ Description="Įrašo identifikacinis numeris", Type=RouteParamType.Integer, Required=true },
						new("page"){ Description="Puslapis", Type=RouteParamType.Integer, Required=false },
						new("limit"){ Description="Įrašų skaičius", Type=RouteParamType.Integer, Required=false },
						new("desc"){ Description="Įrašų rikiavimas mažėjančia tvarka (true:desc,false:asc)", Type=RouteParamType.Boolean, Required=false },
						new("force"){ Description="Priverstinai atnaujinti įrašų skaičių", Type=RouteParamType.Boolean, Required=false },
						new("X-API-Key") { Description = "Prieigos raktas", Type=RouteParamType.String, Location=RouteParamLoc.Header, Required=true }
						],
				})
				.Map(new("/api/export/item/{view}", GetData.Item) {
					Description = "Duomenų gavimas pagal identifikacinį numerį", Response=typeof(object), Code="GetItem",
					Params = [
						new("id"){ Description="Įrašo identifikacinis numeris", Type=RouteParamType.Integer, Required=true },
						new("X-API-Key") { Description = "Prieigos raktas", Type=RouteParamType.String, Location=RouteParamLoc.Header, Required=true }
						],
				})
				.Map(new("/api/export/sertifikatas", GetData.Sertifikatas) { 
					Description = "Sertifikato informacija", Response=typeof(object), Code="GetCert",
					Params = [
						new("id"){ Description="Įrašo identifikacinis numeris", Type=RouteParamType.Integer },
						new("nr"){ Description="Sertifikato numeris (EXP)", Type=RouteParamType.Integer },
						new("X-API-Key") { Description = "Prieigos raktas", Type=RouteParamType.String, Location=RouteParamLoc.Header, Required=true }
						],
				}),
			],
	};
}
