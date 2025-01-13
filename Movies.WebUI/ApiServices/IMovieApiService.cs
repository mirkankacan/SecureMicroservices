using Movies.WebUI.Models;

namespace Movies.WebUI.ApiServices
{
    public interface IMovieApiService
    {
        Task<IEnumerable<Movie>> GetMovies(CancellationToken cancellationToken = default);

        Task<Movie> GetMovie(int id, CancellationToken cancellationToken = default);

        Task<Movie> CreateMovie(Movie movie, CancellationToken cancellationToken = default);

        Task<Movie> UpdateMovie(Movie movie, CancellationToken cancellationToken = default);

        Task<Movie> DeleteMovie(int id, CancellationToken cancellationToken = default);

        Task<UserInfoViewModel> GetUserInfo(CancellationToken cancellationToken = default);
    }
}