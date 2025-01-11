using Movies.WebUI.Models;

namespace Movies.WebUI.ApiServices
{
    public interface IMovieApiService
    {
        Task<IEnumerable<Movie>> GetMovies();

        Task<Movie> GetMovie(int id);

        Task<Movie> CreateMovie(Movie movie);

        Task<Movie> UpdateMovie(Movie movie);

        Task<Movie> DeleteMovie(int id);
    }
}