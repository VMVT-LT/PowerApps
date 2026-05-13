using PowerApps.App;
using PowerApps.Modules;
using PowerApps.Shared;


#if DEBUG
Console.Clear();
#endif

var cfg = new Configuration().Data;

Conn.Export = new(cfg.ConnExport) { Debug=cfg.Debug };

Startup.Routes();

ExportAPI.ApiKeys = cfg.ExportKeys;

var app = Startup.Build(args);
app.UseForwardedHeaders();

app.Run();
