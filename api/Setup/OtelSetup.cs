using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

public static class OtelSetup
{
    public static WebApplicationBuilder SetupOtelServices(this WebApplicationBuilder builder, Configuration config)
    {        
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(config.ServiceName, serviceVersion: config.ServiceVersion))
            .WithTracing(tracing => tracing
                .AddSource(config.ServiceName)
                .AddAspNetCoreInstrumentation()
                .AddHangfireInstrumentation()
                .AddOtlpExporter(otlpOptions => otlpOptions.Endpoint = new Uri(config.AlloyUri))
                .AddConsoleExporter());
        // .WithMetrics(metrics => metrics
        //     .AddMeter(serviceName)
        //     .AddAspNetCoreInstrumentation()
        //     .AddPrometheusExporter()
        //     .AddConsoleExporter());

        return builder;
    }
}