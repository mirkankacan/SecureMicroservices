using Bogus;
using Movies.API.Models;

namespace Movies.API.Data
{
    public static class MoviesContextSeed
    {
        public static Task SeedAsync(MoviesContext moviesContext)
        {
            if (!moviesContext.Movie.Any())
            {
                var faker = new Faker<Movie>()
                .RuleFor(m => m.Id, f => f.IndexFaker + 1)
                .RuleFor(m => m.Title, f => f.Lorem.Sentence(3))
                .RuleFor(m => m.Genre, f => f.PickRandom(new[] { "Action", "Comedy", "Drama", "Horror", "Romance", "Sci-Fi" }))
                .RuleFor(m => m.Rating, f => f.Random.Float(1, 10).ToString("0.0"))
                .RuleFor(m => m.ReleaseDate, f => f.Date.Past(20))
                .RuleFor(m => m.ImageUrl, f => f.Image.PicsumUrl())
                .RuleFor(m => m.Owner, f => f.Person.FullName);

                var movies = faker.Generate(100);

                moviesContext.Movie.AddRange(movies);
                return moviesContext.SaveChangesAsync();
            }
            return Task.CompletedTask;
        }
    }
}