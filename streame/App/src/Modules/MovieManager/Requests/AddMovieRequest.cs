using Streame.App.Errors;

namespace api.Modules.MovieManager.Requests;

public sealed record class AddMovieRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int ReleaseYear { get; set; }
    public string Path { get; set; }

    public bool IsInvalid(out List<InvalidProperty> errors)
    {
        errors = new List<InvalidProperty>();

        if (string.IsNullOrEmpty(Title))
            errors.Add(new InvalidProperty(nameof(Title), "Title cannot be null or empty!"));
        if (string.IsNullOrEmpty(Description))
            errors.Add(new InvalidProperty(nameof(Description), "Description cannot be null or empty!"));
        if (ReleaseYear <= 0)
            errors.Add(new InvalidProperty(nameof(ReleaseYear), "Release year must be greater than 0!"));
        if (string.IsNullOrEmpty(Path))
            errors.Add(new InvalidProperty(nameof(Path), "Path cannot be null or empty!"));
            
        return errors.Count > 0;
    }
}