using api.Core.Result;
using api.Modules.Admin.Requests;
using Hangfire;

namespace api.Modules.Admin.Services;

public class VideoEncoderService
{
    private readonly IBackgroundJobClient backgroundJobClient;
    private readonly VideoEncodingJob videoEncodingJob;
    private readonly ILogger<VideoEncoderService> logger;

    public VideoEncoderService(IBackgroundJobClient backgroundJobClient, VideoEncodingJob videoEncodingJob, ILogger<VideoEncoderService> logger)
    {
        this.backgroundJobClient = backgroundJobClient;
        this.videoEncodingJob = videoEncodingJob;
        this.logger = logger;
    }

    public Result<string> EnqueueEncodeVideo(EnqueueEncodeVideoRequest request)
    {
        var valResult = request.IsValid();
        if (valResult.IsFailure)
        {
            logger.LogError(valResult.Error!.Value.Formatted);
            return valResult.ToFailedResultOf<string>();
        }

        try
        {
            var jobId = backgroundJobClient.Enqueue(() => videoEncodingJob.Run(request.VideoPath));
            return Result<string>.Success(jobId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unable to add video for encoding!");
            return Result<string>.Failure(Errors.Generic(request));
        }
    }

    public Result RemoveEnqueuedVideo(string jobId)
    {
        try
        {
            var result = backgroundJobClient.Delete(jobId);
            if (result == false)
            {
                logger.LogError("Unable to remove job {id}", jobId);
                return Result.Failure(Errors.JobIdNotFound(jobId));
            }
            logger.LogError("Job {id} removed!", jobId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unable to add video for encoding!");
            return Result.Failure(Errors.Generic(jobId));
        }
    }
}