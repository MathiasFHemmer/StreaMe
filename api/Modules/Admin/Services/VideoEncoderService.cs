using api.Core.Result;
using api.Core.UnitOfWork;
using api.Modules.Admin.Requests;
using api.Modules.Movies.Repository;
using Hangfire;

namespace api.Modules.Admin.Services;

public class VideoEncoderService
{
    private readonly IUnitOfWorkFactory unitOfWorkFactory;
    private readonly IMovieRepository movieRepository;
    private readonly IBackgroundJobClient backgroundJobClient;
    private readonly VideoEncodingJob videoEncodingJob;
    private readonly ILogger<VideoEncoderService> logger;

    public VideoEncoderService(
        IBackgroundJobClient backgroundJobClient,
        VideoEncodingJob videoEncodingJob,
        ILogger<VideoEncoderService> logger,
        IUnitOfWorkFactory unitOfWorkFactory,
        IMovieRepository movieRepository)
    {
        this.backgroundJobClient = backgroundJobClient;
        this.videoEncodingJob = videoEncodingJob;
        this.logger = logger;
        this.unitOfWorkFactory = unitOfWorkFactory;
        this.movieRepository = movieRepository;
    }

    public async Task<Result<string>> EnqueueEncodeVideo(EnqueueEncodeVideoRequest request)
    {
        var valResult = request.IsValid();
        if (valResult.IsFailure && false)
        {
            logger.LogError(valResult.Error!.Value.Formatted);
            return valResult.ToFailedResultOf<string>();
        }

        try
        {
            using (unitOfWorkFactory.Create())
            {
                var movie = await movieRepository.InsertNew("A New Movie", default);                
            }
            //var jobId = backgroundJobClient.Enqueue(() => videoEncodingJob.Run(request.VideoPath));
            return Result<string>.Success("1");
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