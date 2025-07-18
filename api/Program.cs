using Hangfire;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Diagnostics;

var serviceName = Assembly.GetEntryAssembly()?.GetName().Name ?? "unknown";
var serviceVersion = "1.0.0";

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration.Get<Configuration>()?
    .BindRuntimeValues() ?? throw new Exception("Missing configuration file!"); 

builder.SetupLogging(config)
    .SetupHangfireService()
    .SetupOtelServices(config)
    ;
    
builder.Services
    .AddTransient<VideoEncodingJob>()
    .AddTransient<JobStateTracker>();

var app = builder.Build()
    .SetupHangfireDashboard();

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