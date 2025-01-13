using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Movies.WebUI.Models;
using Newtonsoft.Json;
using System.Text;

namespace Movies.WebUI.ApiServices
{
    public class MovieApiService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor) : IMovieApiService
    {
        public async Task<Movie> CreateMovie(Movie movie, CancellationToken cancellationToken)
        {
            var httpClient = httpClientFactory.CreateClient("MoviesAPIClient");
            var request = new HttpRequestMessage(HttpMethod.Post, $"/api/movies");
            request.Content = new StringContent(JsonConvert.SerializeObject(movie), Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Movie>(content);
        }

        public async Task<Movie> DeleteMovie(int id, CancellationToken cancellationToken)
        {
            var httpClient = httpClientFactory.CreateClient("MoviesAPIClient");
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/movies/{id}");

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Movie>(content);
        }

        public async Task<Movie> GetMovie(int id, CancellationToken cancellationToken)
        {
            var httpClient = httpClientFactory.CreateClient("MoviesAPIClient");
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/movies/{id}");

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Movie>(content);
        }

        public async Task<IEnumerable<Movie>> GetMovies(CancellationToken cancellationToken)
        {
            // WAY 1
            var httpClient = httpClientFactory.CreateClient("MoviesAPIClient");
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/movies");

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Movie>>(content);

            // WAY 2
            //var apiClientCredentials = new ClientCredentialsTokenRequest
            //{
            //    Address = configuration["Authentication:TokenAddress"]!,
            //    ClientId = configuration["Authentication:ApiClientId"]!,
            //    ClientSecret = configuration["Authentication:ClientSecret"]!,
            //    Scope = configuration["Authentication:ApiScope"]!
            //};
            //using var client = new HttpClient();
            //var disco = await client.GetDiscoveryDocumentAsync(configuration["Authentication:IdentityServer"]!);
            //if (disco.IsError)
            //{
            //    return null;
            //}
            //var tokenResponse = await client.RequestClientCredentialsTokenAsync(apiClientCredentials);
            //if (tokenResponse.IsError)
            //{
            //    return null;
            //}

            //using var apiClient = new HttpClient();
            //apiClient.SetBearerToken(tokenResponse.AccessToken);

            //var response = await apiClient.GetFromJsonAsync<List<Movie>>(configuration["Authentication:ApiAddress"] + "/api/movies");

            //return response.Any() ? response : null;
        }

        public async Task<UserInfoViewModel> GetUserInfo(CancellationToken cancellationToken = default)
        {
            var idpClient = httpClientFactory.CreateClient("IDPClient");
            var metaDataResponse = await idpClient.GetDiscoveryDocumentAsync();
            if (metaDataResponse.IsError)
            {
                throw new HttpRequestException("Something went wrong while requesting the discovery document");
            }
            var accessToken = await httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new HttpRequestException("Something went wrong while requesting the access token");
            }
            var userInfoResponse = await idpClient.GetUserInfoAsync(
                   new UserInfoRequest
                   {
                       Address = metaDataResponse.UserInfoEndpoint,
                       Token = accessToken
                   });
            if (userInfoResponse.IsError)
            {
                throw new HttpRequestException("Something went wrong while getting user info");
            }

            var userInfoDictionary = new Dictionary<string, string>();
            foreach (var claim in userInfoResponse.Claims)
            {
                userInfoDictionary.Add(claim.Type, claim.Value);
            }
            return new UserInfoViewModel(userInfoDictionary);
        }

        public async Task<Movie> UpdateMovie(Movie movie, CancellationToken cancellationToken)
        {
            var httpClient = httpClientFactory.CreateClient("MoviesAPIClient");
            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/movies/{movie.Id}");
            request.Content = new StringContent(JsonConvert.SerializeObject(movie), Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Movie>(content);
        }
    }
}