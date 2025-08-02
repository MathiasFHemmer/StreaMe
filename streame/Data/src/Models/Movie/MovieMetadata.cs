namespace Streame.Data.Models.Movie;

public class MovieMetadata : Entity
{
    public Guid MovieId { get; set; }
    public MovieStatus Status { get; set; }
    public int LengthMinutes { get; set; }
    public string FileName { get; set; }
    public string FileLocation { get; set; }
    public string HangfireJobId { get; set; }
    public MovieMetadata(Guid movieId, string hangfireJobId, MovieStatus status = MovieStatus.Processing)
    {
        MovieId = movieId;
        Status = status;
        HangfireJobId = hangfireJobId;
    }
}
