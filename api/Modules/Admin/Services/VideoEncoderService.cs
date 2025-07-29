using api.Core.Result;
using api.Core.UnitOfWork;
using api.Modules.Admin.Requests;
using api.Modules.Movies.Repository;
using Hangfire;

namespace api.Modules.Admin.Services;

public class VideoEncoderService
{
    private readonly ILogger<VideoEncoderService> logger;
    private readonly IConfiguration configuration;
    private readonly IUnitOfWorkFactory unitOfWorkFactory;
    private readonly IMovieRepository movieRepository;
    private readonly IBackgroundJobClient backgroundJobClient;
    private readonly VideoEncodingJob videoEncodingJob;

    public VideoEncoderService(
        IBackgroundJobClient backgroundJobClient,
        VideoEncodingJob videoEncodingJob,
        ILogger<VideoEncoderService> logger,
        IUnitOfWorkFactory unitOfWorkFactory,
        IMovieRepository movieRepository,
        IConfiguration configuration)
    {
        this.backgroundJobClient = backgroundJobClient;
        this.videoEncodingJob = videoEncodingJob;
        this.logger = logger;
        this.unitOfWorkFactory = unitOfWorkFactory;
        this.movieRepository = movieRepository;
        this.configuration = configuration;
    }

    public async Task<Result<string>> EnqueueEncodeVideo(EnqueueEncodeVideoRequest request, CancellationToken ct)
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
                var inputPath = Path.Combine(configuration.GetValue<string>(nameof(Configuration.MoviesInputPath))!, request.Path);
                var outputPath = configuration.GetValue<string>(nameof(Configuration.MovieStoragePath))!;
                var jobId = backgroundJobClient.Enqueue(() => videoEncodingJob.Run(inputPath, outputPath, request.Name));
                var movie = await movieRepository.InsertNew(request.Name, request.Description, request.ReleaseYear, outputPath, jobId, ct);
                if (movie.IsFailure)
                    return Result<string>.Failure(Errors.Generic(movie.Error!.Value.Formatted));

                return Result<string>.Success(movie.Value!.Id.ToString());
            }
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