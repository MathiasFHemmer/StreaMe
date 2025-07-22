using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using api.Modules.Admin.Services;
using api.Modules.Admin.Requests;
using api.Modules.Admin;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration.Get<Configuration>()?
    .BindRuntimeValues() ?? throw new Exception("Missing configuration file!"); 

builder.SetupLogging(config)
    .SetupHangfireService()
    .SetupOtelServices(config)
    ;
    
builder.Services
    .AddTransient<VideoEncodingJob>()
    .AddTransient<JobStateTracker>()
    .AddTransient<VideoEncoderService>();

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
    [FromBody] EnqueueEncodeVideoRequest request,
    [FromServices] VideoEncoderService service) =>
{
    var result = service.EnqueueEncodeVideo(request);
    return result switch
    {
        { IsSuccess: true } => Results.Ok($"Job {result.Value} enqueued!"),
        { Error.Code: Errors.Code error } when error == Errors.Code.PathNotFound => Results.BadRequest(result.Error.Value.Formatted),
        { Error.Code: Errors.Code error } when error == Errors.Code.VideoPathEmpty => Results.NotFound(result.Error.Value.Formatted),
        { Error: var error } => Results.InternalServerError(error?.Formatted)
    };
});

app.MapPost("/remove/{jobId}", (
    string jobId,
    [FromServices] VideoEncoderService service) =>
{
    var result = service.RemoveEnqueuedVideo(jobId);
    return result switch
    {
        { IsSuccess: true } => Results.Ok($"Job {jobId} removed!"),
        { Error.Code: Errors.Code error} when error == Errors.Code.JobIdNotFound => Results.NotFound(result.Error.Value.Formatted),
        { Error: var error } => Results.InternalServerError(error?.Formatted)
    };
});

app.Run();