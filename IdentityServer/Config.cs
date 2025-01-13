using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using IdentityModel;
using System.Security.Claims;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<Client> Clients => new Client[]
        {
                new Client
                {
                    ClientId = "movies_api_client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("secretkey".Sha256()) },
                    AllowedScopes={"moviesAPI"}
                },
              new Client
                {
                    ClientId = "movies_mvc_client",
                    ClientName = "Movies MVC Web UI",
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    RequirePkce=false,
                    AllowRememberConsent = false,
                    RedirectUris = { "https://localhost:3002/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:3002/signout-callback-oidc" },
                    ClientSecrets = { new Secret("secretkey".Sha256()) },
                    AllowedScopes = new List<string>()
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "moviesAPI"
                    }
                }
        };

        public static IEnumerable<ApiScope> ApiScopes => new ApiScope[]
        {
            new ApiScope("moviesAPI","Movies API")
        };

        public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
        {
        };

        public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

        public static List<TestUser> TestUsers => new List<TestUser>()
        {
            new TestUser
            {
                SubjectId="26FC46B0-1374-4A3F-BA25-38DC6975EDE6",
                Username="mirko",
                Password="mirko",
                Claims= new List<Claim>
                {
                    new Claim(JwtClaimTypes.GivenName,"mirkan"),
                    new Claim(JwtClaimTypes.FamilyName,"kaçan")
                }
            }
        };
    }
}