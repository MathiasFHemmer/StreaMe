using System.Diagnostics;
using System.Text;
using OpenTelemetry.Trace;

public class JobStateTracker
{
    private static int _active = 0;
    public static int ActiveJobs => _active;

    public void Increment() => Interlocked.Increment(ref _active);
    public void Decrement() => Interlocked.Decrement(ref _active);
}

public class VideoEncodingJob
{
    public ActivitySource ActivitySource { get; }
    private readonly JobStateTracker _tracker;

    public VideoEncodingJob(ActivitySource activitySource, JobStateTracker tracker)
    {
        this.ActivitySource = activitySource;
        _tracker = tracker;
    }

    public async Task Run(string inputPath)
{
    using var activity = ActivitySource.StartActivity("FFmpeg HLS Job");
    _tracker.Increment();
    
    try
    {
        string outputDir = Path.Combine(Path.GetDirectoryName(inputPath)!, "hls", Path.GetFileNameWithoutExtension(inputPath));
        Directory.CreateDirectory(outputDir);

        string outputPath = Path.Combine(outputDir, "index.m3u8");

        var args = $"-y -i \"{inputPath}\" " +
            // Video streams
            "-filter_complex \" " +
            "[0:v]split=4[v1][v2][v3][v4]; " +
            "[v1]scale=-2:360[v1out]; " +
            "[v2]scale=-2:480[v2out]; " +
            "[v3]scale=-2:720[v3out]; " +
            "[v4]scale=-2:1080[v4out]; " +
            // Split audio for each stream
            "[0:a]asplit=4[a1][a2][a3][a4]\" " +
            // Video + Audio mappings
            "-map \"[v1out]\" -map \"[a1]\" -c:v:0 libx264 -b:v:0 800k -c:a:0 aac -b:a:0 128k " +
            "-map \"[v2out]\" -map \"[a2]\" -c:v:1 libx264 -b:v:1 1200k -c:a:1 aac -b:a:1 128k " +
            "-map \"[v3out]\" -map \"[a3]\" -c:v:2 libx264 -b:v:2 2500k -c:a:2 aac -b:a:2 128k " +
            "-map \"[v4out]\" -map \"[a4]\" -c:v:3 libx264 -b:v:3 4500k -c:a:3 aac -b:a:3 128k " +
            // HLS settings
            "-var_stream_map \"v:0,a:0 v:1,a:1 v:2,a:2 v:3,a:3\" " +
            "-f hls -hls_time 4 -hls_list_size 0 " +
            "-master_pl_name \"master.m3u8\" " +
            $"-hls_segment_filename \"{outputDir}/segment_%v_%03d.ts\" " +
            $"{outputDir}/stream_%v.m3u8";

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = args,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        // For logging FFmpeg output
        var output = new StringBuilder();
        process.ErrorDataReceived += (sender, e) => output.AppendLine(e.Data);
        process.OutputDataReceived += (sender, e) => output.AppendLine(e.Data);

        process.Start();
        process.BeginErrorReadLine();
        process.BeginOutputReadLine();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new Exception($"FFmpeg failed with exit code {process.ExitCode}\n{output}");
        }

        activity?.SetStatus(ActivityStatusCode.Ok);
        }
    catch (Exception ex)
    {
        activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
        activity?.RecordException(ex);
        throw;
    }
    finally
    {
        _tracker.Decrement();
    }
    }
}
