using PowerApps.App;
using PowerApps.Modules;
using PowerApps.Shared;


#if DEBUG
Console.Clear();
#endif

var cfg = new Configuration().Data;

Conn.Export = new(cfg.ConnExport) { Debug = cfg.Debug };
Conn.G9 = new(cfg.ConnG9) { Debug = cfg.Debug };

Startup.Routes();

ExportAPI.ApiKeys = cfg.ExportKeys;
G9API.ApiKeys = cfg.G9Keys;
G9API.Cfg = cfg.Registrai;

var app = Startup.Build(args);
app.UseForwardedHeaders();

app.Run();
