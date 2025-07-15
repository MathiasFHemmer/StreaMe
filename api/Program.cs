using Hangfire;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Logs;
using OpenTelemetry.Trace;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHangfire(c =>
{
    c.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseInMemoryStorage();
});

builder.Services.AddHangfireServer();

var serviceName = Assembly.GetEntryAssembly()?.GetName().Name ?? "unknown";
var serviceVersion = "1.0.0";

builder.Services.AddSingleton(new ActivitySource(serviceName));

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(
        serviceName: serviceName,
        serviceVersion: serviceVersion))
    .WithTracing(tracing => tracing
        .AddSource(serviceName)
        .AddAspNetCoreInstrumentation(          )
        .AddOtlpExporter(otlpOptions =>
        {
            otlpOptions.Endpoint = new Uri("http://alloy:4317");
        })
        .AddConsoleExporter());
    // .WithMetrics(metrics => metrics
    //     .AddMeter(serviceName)
    //     .AddAspNetCoreInstrumentation()
    //     .AddPrometheusExporter()
    //     .AddConsoleExporter());

builder.Logging.AddOpenTelemetry(options => options
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(
        serviceName: serviceName,
        serviceVersion: serviceVersion))
    .AddOtlpExporter(otlpOptions =>
    {
        otlpOptions.Endpoint = new Uri("http://alloy:4317");
    })
    .AddConsoleExporter());

builder.Services.AddTransient<VideoEncodingJob>();
builder.Services.AddTransient<JobStateTracker>();

var app = builder.Build();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = [new HangfireAuthFilter()]
});

app.MapGet("/", (
    [FromServices] ActivitySource activitySource,
    [FromServices] ILogger<Program> logger) =>
{
    using var activity = activitySource.StartActivity("EncodeVideo");
    logger.LogInformation("Testing");
    return "Hello World!";
});
app.MapPost("/encode", (
    [FromBody] EncodeRequest request, 
    [FromServices] VideoEncodingJob job, 
    [FromServices] IBackgroundJobClient jobClient,
    [FromServices] ILogger<Program> logger) =>
{
    logger.LogInformation("Enqueueing encoding job for path: {Path}", request.Path);
    
    var jobId = jobClient.Enqueue(() => job.Run(request.Path));
    
    logger.LogInformation("Job {JobId} enqueued", jobId);
    
    return Results.Ok($"Job {jobId} enqueued!");
});

app.Run();
