using System.Diagnostics;
using System.Diagnostics.Metrics;

public class JobStateTracker
{
    private static int _active = 0;
    public static int ActiveJobs => _active;

    public void Increment() => Interlocked.Increment(ref _active);
    public void Decrement() => Interlocked.Decrement(ref _active);
}

public class VideoEncodingJob
{
    private static readonly ActivitySource ActivitySource = new("VideoApi");
    private readonly JobStateTracker _tracker;

    public VideoEncodingJob(JobStateTracker tracker)
    {
        _tracker = tracker;
    }

    public async Task Run(string inputPath)
    {
        using var activity = ActivitySource.StartActivity("FFmpeg HLS Job");

        _tracker.Increment();
        try
        {
            string outputDir = Path.Combine(Path.GetDirectoryName(inputPath)!, "hls");
            Directory.CreateDirectory(outputDir);

            string outputPath = Path.Combine(outputDir, "index.m3u8");

            var args = $"-i \"{inputPath}\" -codec: copy -start_number 0 -hls_time 10 -hls_list_size 0 -f hls \"{outputPath}\"";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = args,
                    RedirectStandardError = false,
                    RedirectStandardOutput = false,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            await process.WaitForExitAsync();

            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
        finally
        {
            _tracker.Decrement();
        }
    }
}
