namespace api.Core.Entities;

public class Movie : Entity
{
    public string Title { get; set; }
    public int ReleaseYear { get; set; }
    public string Description { get; set; }
    public Movie(string title, int releaseYear, string description)
    {
        Title = title;
        ReleaseYear = releaseYear;
        Description = description;
    }
}
