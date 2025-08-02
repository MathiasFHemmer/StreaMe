using api.Setup;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration.Get<Configuration>()?
    .BindRuntimeValues() ?? throw new Exception("Missing configuration file!");
    
builder.Services.AddSingleton(config);

builder.SetupLogging(config)
    .SetupHangfireService(config)
    .SetupOtelServices(config)
    .SetupApiServices();
    ;

var app = builder.Build()
    .SetupHangfireDashboard()
    .SetupApiApp();

app.Run();