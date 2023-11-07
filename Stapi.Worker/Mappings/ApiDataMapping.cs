namespace Stapi.Worker.Mappings;

public class ApiDataMapping
{
    public static readonly Dictionary<string, string> DataMapping = new()
    {
        { "animal", "animals" },
        { "astronomicalObject", "astronomicalObjects" },
        { "book", "books" },
        { "bookCollection", "bookCollections" },
        { "bookSeries", "bookSeries" },
        { "character", "characters" },
        { "weapon", "weapons" },
        { "movie", "movies" },
        { "videoGame", "videoGames" }
    };
}