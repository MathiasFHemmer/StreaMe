using System.Diagnostics;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

public static class LoggingSetup
{
    public static WebApplicationBuilder SetupLogging(this WebApplicationBuilder builder, Configuration config)
    {
        builder.Services.AddSingleton(new ActivitySource(config.ServiceName));

        builder.Logging.AddOpenTelemetry(options => options
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(config.ServiceName, serviceVersion: config.ServiceVersion))
            .AddOtlpExporter(otlpOptions => otlpOptions.Endpoint = new Uri(config.AlloyUri))
            .AddConsoleExporter());

        return builder;
    }
}