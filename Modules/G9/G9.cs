using Microsoft.AspNetCore.Http;
using PowerApps.Modules.G9.Methods;
using PowerApps.Modules.G9.Models;
using PowerApps.Shared;
using Vmvt.RouteAPI;

namespace PowerApps.Modules;

/// <summary>Registro inicijavimas</summary>
public class G9API {
	/// <summary>API prieigos raktai</summary>
	public static List<string> ApiKeys { get; set; } = [];
	/// <summary>API prieigos raktai</summary>
	public static CfgRegistrai Cfg { get; set; } = new();

	/// <summary>Export maršrutų priskyrimas</summary>
	/// <returns></returns>
	public static RouteDefinition Route() => new("G9") {
		Description = "G9-app duomenų integracija",
		Tag = "g9", Version = "v1",
		Routes = [
			new RouteGroup("Deklaracijos duomenų gavimas")
				.Map(new("/api/g9/deklaracija", Deklar.Get) {
					Description = "Deklaracijos duomenų gavimas pagal identifikacinį numerį", Response=typeof(Deklaracija), Code = "Dkl_Get",
					Params = [
						new("dkl"){ Description="Deklaracijos ID", Type=RouteParamType.Integer, Required=true },
						new("X-API-Key") { Description = "Prieigos raktas", Type=RouteParamType.String, Location=RouteParamLoc.Header, Required=true }
						],
				})
				.Map(new("/api/g9/deklaracija/rodikliai", Deklar.GetRod) {
					Description = "Deklaracijos rodiklių sąrašas", Response = typeof(List<DklRodiklis>), Code = "Dkl_GetRod",
					Params = [
						new("dkl"){ Description="Deklaracijos ID", Type=RouteParamType.Integer, Required=true },
						new("X-API-Key") { Description = "Prieigos raktas", Type=RouteParamType.String, Location=RouteParamLoc.Header, Required=true }
						],
				})
				.Map(new("/api/g9/deklaracija/reiksmes", Deklar.GetReiksm) {
					Description = "Deklaracijos reikšmių sąrašas", Response = typeof(List<DklRodiklis>), Code = "Dkl_GetReiksm",
					Params = [
						new("dkl"){ Description="Deklaracijos ID", Type=RouteParamType.Integer, Required=true },
						new("rod"){ Description="Rodiklio ID", Type=RouteParamType.Integer, Required=true },
						new("X-API-Key") { Description = "Prieigos raktas", Type=RouteParamType.String, Location=RouteParamLoc.Header, Required=true }
						],
				}),
			new RouteGroup("Adresai")
				.Map(new("/api/g9/ar/list",Adr.List){
					Description = "Savivaldybių sąrašas (Apg/Adm/Sav)", Response = typeof(List<ARList>), Code = "AR_GetList"
				})
				.Map(new("/api/g9/ar/details",Adr.Details){
					Description = "Adreso informacija", Response = typeof(List<ARDetales>), Code = "AR_GetDetails",
					Params = [
						new("id") { Description="Įrašo id", Type=RouteParamType.Integer, Required=true }
					]
				})
				.Map(new("/api/g9/ar/find/gyv",Adr.FindGyv){
					Description = "Gyvenvietės paieška", Response = typeof(List<ARFind>), Code = "AR_FindGyv",
					Params = [
						new("q") { Description="Paieškos tekstas", Type=RouteParamType.String, Required=true },
						new("top") { Description="Grąžinamų įrašų skaičius", Type=RouteParamType.Integer, Default="10" }
					]
				})
				.Map(new("/api/g9/ar/find/adr",Adr.FindAdr){
					Description = "Adreso paieška", Response = typeof(List<ARFind>), Code = "AR_FindAdr",
					Params = [
						new("q") { Description="Paieškos tekstas", Type=RouteParamType.String, Required=true },
						new("gyv") { Description="Gyvenvietės id", Type=RouteParamType.String, Required=true },
						new("top") { Description="Grąžinamų įrašų skaičius", Type=RouteParamType.Integer, Default="10"  }
					]
				}),
			new RouteGroup("Juridiniai asmenys")
				.Map(new("/api/g9/jar/find",Jar.Find){
					Description = "Juridinio asmens paieška", Response = typeof(List<JARFind>), Code = "JAR_Find",
					Params = [
						new("q") { Description="Paieškos tekstas", Type=RouteParamType.String, Required=true },
						new("top") { Description="Grąžinamų įrašų skaičius", Type=RouteParamType.Integer, Default="10" }
					]
				})
				.Map(new("/api/g9/jar/details",Jar.Details){
					Description = "Juridinio asmens informacija", Response = typeof(JARItem), Code = "JAR_Details",
					Params = [
						new("id") { Description="Įrašo id", Type=RouteParamType.Integer, Required=true }
					]
				})
			]
	};
}

