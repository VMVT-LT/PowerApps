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
				.Map(new("/api/g9/deklaracija", Deklar.Get) { Deprecated=true,
					Description = "Deklaracijos duomenų gavimas pagal identifikacinį numerį", Response=typeof(Deklaracija), Code = "Dkl_Get",
					Params = [
						new("dkl"){ Description="Deklaracijos ID", Type=RouteParamType.Integer, Required=true },
						new("X-API-Key") { Description = "Prieigos raktas", Type=RouteParamType.String, Location=RouteParamLoc.Header, Required=true }
						],
				})
				.Map(new("/api/g9/deklaracija/rodikliai", Deklar.GetRod) { Deprecated=true,
					Description = "Deklaracijos rodiklių sąrašas", Response = typeof(List<DklRodiklis>), Code = "Dkl_GetRod",
					Params = [
						new("dkl"){ Description="Deklaracijos ID", Type=RouteParamType.Integer, Required=true },
						new("X-API-Key") { Description = "Prieigos raktas", Type=RouteParamType.String, Location=RouteParamLoc.Header, Required=true }
						],
				})
				.Map(new("/api/g9/deklaracija/reiksmes", Deklar.GetReiksm) { Deprecated=true,
					Description = "Deklaracijos reikšmių sąrašas", Response = typeof(List<DklRodiklis>), Code = "Dkl_GetReiksm",
					Params = [
						new("dkl"){ Description="Deklaracijos ID", Type=RouteParamType.Integer, Required=true },
						new("rod"){ Description="Rodiklio ID", Type=RouteParamType.Integer, Required=true },
						new("X-API-Key") { Description = "Prieigos raktas", Type=RouteParamType.String, Location=RouteParamLoc.Header, Required=true }
						],
				}),
			new RouteGroup("Rodikliai")
				.Map(new("/api/g9/rodikliai/list",Rodikliai.List){
					Description = "Visų rodiklių sąrašas", Response=typeof(List<Rodiklis>), Code = "Rod_List",
					Params = [
						new ("g") { Description="Rodiklių grupės id", Type=RouteParamType.Integer },
						new ("inact") { Description="Rodyti neaktyvius", Type=RouteParamType.Boolean },
						new ("X-API-Key") { Description = "Prieigos raktas", Type=RouteParamType.String, Location=RouteParamLoc.Header, Required=true },
					], Errors=[404]
				})
				.Map(new("/api/g9/rodikliai/{id}",Rodikliai.Info){
					Description = "Rodiklio informacija", Response=typeof(Rodiklis), Code = "Rod_GetInfo",
					Params = [
						new ("X-API-Key") { Description = "Prieigos raktas", Type=RouteParamType.String, Location=RouteParamLoc.Header, Required=true },
					], Errors=[400]
				})
				.Map(new("/api/g9/rodikliai/{id}",Rodikliai.SetInfo, Method.Post){
					Description = "Keisti rodiklio informaciją<br>Kurti naują: id=0", Response=typeof(Rodiklis), Code = "Rod_SetInfo",
					Params = [
						new ("X-API-Key") { Description = "Prieigos raktas", Type=RouteParamType.String, Location=RouteParamLoc.Header, Required=true },
					], Errors=[400]
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
			new RouteGroup("GVTS")
				.Map(new("/api/g9/gvts/{id}",Gvts.Info){
					Description = "Geriamojo vandens tiekimo sistemos informacija", Response = typeof(GvtsItem), Code = "Gvts_GetInfo",
					Params = [
						new("X-API-Key") { Description = "Prieigos raktas", Type=RouteParamType.String, Location=RouteParamLoc.Header, Required=true },
					], Errors=[404]
				})
				.Map(new("/api/g9/gvts/{id}",Gvts.InfoSet,Method.Post){
					Description = "Geriamojo vandens tiekimo sistemos informacijos keitimas", Response = typeof(GvtsItem), Code = "Gvts_SetInfo",
					Params = [
						new("X-API-Key") { Description = "Prieigos raktas", Type=RouteParamType.String, Location=RouteParamLoc.Header, Required=true },
					], Errors=[400]
				})
				.Map(new("/api/g9/gvts/list",Gvts.List){
					Description = "Geriamojo vandens tiekimo sistemų sąrašas", Response = typeof(GvtsList), Code = "Gvts_GetList",
					Params = [
						new("apg") { Description="Apygardos id", Type=RouteParamType.Integer },
						new("sav") { Description="Savivaldybės id", Type=RouteParamType.Integer },
						new("inact") { Description="Rodyti neaktyvius GVTS", Type=RouteParamType.Boolean, Default="false" },
						new("X-API-Key") { Description = "Prieigos raktas", Type=RouteParamType.String, Location=RouteParamLoc.Header, Required=true },
					], Errors=[401]
				})
				.Map(new("/api/g9/gvts/{id}/user",Gvts.UserAdd,Method.Post){
					Description = "Inspektoriaus priskyrimas geriamojo vandens tiekimo sistemai", Response = typeof(DefaultResponse), Code = "Gvts_AddUser",
					Params = [
						new("usr") { Description="Vartotojo id", Type=RouteParamType.String, Required=true },
						new("X-API-Key") { Description = "Prieigos raktas", Type=RouteParamType.String, Location=RouteParamLoc.Header, Required=true },
					], Errors = [400]
				})
				.Map(new("/api/g9/gvts/{id}/user",Gvts.UserRem,Method.Delete){
					Description = "Inspektoriaus pašalinimas geriamojo vandens tiekimo sistemai", Response = typeof(DefaultResponse), Code = "Gvts_RemUser",
					Params = [
						new("usr") { Description="Vartotojo id", Type=RouteParamType.String, Required=true },
						new("X-API-Key") { Description = "Prieigos raktas", Type=RouteParamType.String, Location=RouteParamLoc.Header, Required=true },
					], Errors = [400]
				}),
			new RouteGroup("Juridiniai asmenys")
				.Map(new("/api/g9/jar/list",Jar.List){
					Description = "G9 juridinių asmenų sąrašas", Response = typeof(List<JARDetails>), Code = "JAR_List",
					Params = [
						new("X-API-Key") { Description = "Prieigos raktas", Type=RouteParamType.String, Location=RouteParamLoc.Header, Required=true },
					]
				})
				.Map(new("/api/g9/jar/{id}",Jar.Info){
					Description = "G9 juridinio asmens detalės", Response = typeof(List<JARDetails>), Code = "JAR_GetInfo",
					Params = [
						new("X-API-Key") { Description = "Prieigos raktas", Type=RouteParamType.String, Location=RouteParamLoc.Header, Required=true },
					]
				})
				.Map(new("/api/g9/jar/{id}",Jar.InfoSet, Method.Post){
					Description = "G9 juridinio asmens detalių keitimas", Response = typeof(JARDtlSet), Code = "JAR_SetInfo",
					Params = [
						new("X-API-Key") { Description = "Prieigos raktas", Type=RouteParamType.String, Location=RouteParamLoc.Header, Required=true },
					]
				})
				.Map(new("/api/g9/jar/find",Jar.Find){
					Description = "Juridinio asmens paieška (RC)", Response = typeof(List<JARFind>), Code = "JAR_Find",
					Params = [
						new("q") { Description="Paieškos tekstas", Type=RouteParamType.String, Required=true },
						new("top") { Description="Grąžinamų įrašų skaičius", Type=RouteParamType.Integer, Default="10" }
					]
				})
				.Map(new("/api/g9/jar/details",Jar.Details){
					Description = "Juridinio asmens informacija (RC)", Response = typeof(JARItem), Code = "JAR_Details",
					Params = [
						new("id") { Description="Įrašo id", Type=RouteParamType.Integer, Required=true }
					]
				}),
			new RouteGroup("Klasifikatoriai")
				.Map(new("/api/g9/lookup",Lookup.List){
					Description = "Pilnas G9 klasifikatorių sąrašas", Response = typeof(LookupResponse), Code = "Klas_List"
				}),
			new RouteGroup("Vartotojai")
				.Map(new("/api/g9/users",Users.List){
					Description = "Vartotojų sąrašas", Response = typeof(List<User>), Code = "Usr_List",
					Params = [
						new ("apg") { Description="Apygardos id", Type=RouteParamType.Integer },
						new ("inact") { Description="Rodyti neaktyvius", Type=RouteParamType.Boolean },
						new ("X-API-Key") { Description = "Prieigos raktas", Type=RouteParamType.String, Location=RouteParamLoc.Header, Required=true },
					]
				}),
			new RouteGroup("Administravimas")
				.Map(new ("/api/g9/admin/jar",Jar.AdmUpdateJar,Method.Post){
					Description = "Juridinių asmenų informacijos atnaujinimas iš registrų centro", Response = typeof(DefaultResponse), Code = "Adm_JARUpdate",
					Params = [
						new ("X-API-Key") { Description = "Prieigos raktas", Type=RouteParamType.String, Location=RouteParamLoc.Header, Required=true },
					]
				})
			]
	};
}

