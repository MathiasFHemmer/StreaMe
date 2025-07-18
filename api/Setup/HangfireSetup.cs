using Hangfire;
using OpenTelemetry.Trace;

public static class HangfireSetup
{
    const string HangfireUrl = "/hangfire";

    public static WebApplicationBuilder SetupHangfireService(this WebApplicationBuilder builder)
    {
        builder.Services.AddHangfire(config =>
        {
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseInMemoryStorage();
        });

        builder.Services.AddHangfireServer();
        builder.Services.Configure<HangfireInstrumentationOptions>(options =>
        {
            options.RecordException = true;
        });

        return builder;
    }

    public static WebApplication SetupHangfireDashboard(this WebApplication app)
    {
        app.UseHangfireDashboard(HangfireUrl, new DashboardOptions
        {
            Authorization = [new HangfireAuthFilter()]
        });

        return app;
    }
}