namespace api.Modules.Movies.Repository;

public class Movie
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public int ReleaseYear { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
