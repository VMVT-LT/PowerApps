using Microsoft.AspNetCore.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization;
using Vmvt.RouteAPI;

namespace PowerApps.App;

/// <summary>Application initial startup class</summary>
public static class Startup {
	/// <summary></summary>
	public static string ConnStr { get; set; } = "";

	private static List<RouteDefinition> Endpoints { get; } = [];
	/// <summary></summary>
	/// <param name="route"></param>
	public static void Routes(params RouteDefinition[] route) { Endpoints.AddRange(route); }

	private const string RouteNamespace = "PowerApps.Modules";
	private const string RouteMethod = "Route";
	/// <summary>Add available routes</summary>
	public static void Routes() {
		foreach (string file in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, $"{RouteNamespace}.*.dll"))
			try { Assembly.LoadFrom(file); } catch (Exception ex) {
				Console.WriteLine($"Error loading assembly {Path.GetFileName(file)}: {ex.Message}");
			}


		var routes = new List<RouteDefinition>();
		foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			if (assembly.FullName is not null && assembly.FullName.StartsWith(RouteNamespace))
				foreach (var i in assembly.GetTypes())
					if (i.Namespace == RouteNamespace && i.IsClass && !i.IsInterface && !i.IsAbstract) {
						var mtd = i.GetMethod(RouteMethod, BindingFlags.Static | BindingFlags.Public, null, Type.EmptyTypes, null);
						if (mtd is not null && mtd.ReturnType == typeof(RouteDefinition) && mtd.Invoke(null, null) is RouteDefinition definition)
							routes.Add(definition);
					}
		Endpoints.AddRange(routes.OrderBy(x => x.Tag));

	}

	/// <summary>Build minimal API app</summary>
	/// <param name="args">primary execution arguments</param>
	/// <returns>WebApplication</returns>
	public static WebApplication Build(string[] args) {
		var builder = WebApplication.CreateBuilder(args);
		builder.WebHost.UseKestrel(option => option.AddServerHeader = false);

		ConnStr = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";

		Endpoints.Insert(0, new("PowerApps") {
			Description = "Bendri VMVT PowerApps API", Version = "v1",
			Routes = [new RouteGroup("PowerApps").Map(new("/info", () => Endpoints) { Description = "Bendra informacija apie registrą" })]
		});

		builder.Services.Configure<ForwardedHeadersOptions>(options => {
			options.ForwardedHeaders =
				Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
				Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
			//TODO: Range from apppsettings.json
			options.KnownNetworks.Add(new(System.Net.IPAddress.Parse("172.16.0.0"), 12));
		});

		builder.Services.AddSwagger(Endpoints);

		builder.Services.ConfigureHttpJsonOptions(a => {
			a.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
			//a.SerializerOptions.PropertyNamingPolicy=null; 
			a.SerializerOptions.WriteIndented = false;
			a.SerializerOptions.Converters.Add(new CustomDateTimeConverter());
			a.SerializerOptions.Converters.Add(new CustomDateOnlyConverter());
			a.SerializerOptions.Converters.Add(new CustomIntStringTupleConverter());
			a.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
		});

		var app = builder.Build();
		app.UseExceptionHandler(exh => exh.Run(HandleError));
		app.UseRouteEndpoints(Endpoints);

		return app;
	}


	private static async Task HandleError(HttpContext ctx){
		var rsp = ctx.Response;

		var ex = ctx.Features.Get<IExceptionHandlerFeature>();

		if(ex is not null && ex.Error is not null){
			await rsp.WriteAsync("Error...");
		}
	}

}
