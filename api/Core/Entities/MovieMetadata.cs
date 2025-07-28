namespace api.Modules.Movies.Repository;

public class MovieMetadata
{
    public Guid Id { get; set; }
    public Guid MovieId { get; set; }
    public MovieStatus Status { get; set; }
    public int LengthMinutes { get; set; }
    public string FileName { get; set; }
    public string FileLocation { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
