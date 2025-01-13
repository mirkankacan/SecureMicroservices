using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using IdentityModel;
using System.Security.Claims;
using System.Text.Json;

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
                    IdentityServerConstants.StandardScopes.Address,
                    IdentityServerConstants.StandardScopes.Email,
                    "moviesAPI",
                    "roles"
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
            new IdentityResources.Address(),
            new IdentityResources.Email(),
            new IdentityResource(
                "roles",
                "Your role(s)",
                new List<string>() {"role"})
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
                    new Claim(JwtClaimTypes.Name,"Mirkan Kaçan"),
                    new Claim(JwtClaimTypes.GivenName,"Mirkan"),
                    new Claim(JwtClaimTypes.FamilyName,"Kaçan"),
                    new Claim(JwtClaimTypes.Email,"kacanmirkan@gmail.com"),
                    new Claim(JwtClaimTypes.Role,"Admin"),
                    new Claim(JwtClaimTypes.EmailVerified,"true",ClaimValueTypes.Boolean),
                    new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(new
                    {
                        street_address = "One Hacker Way",
                        locality = "Heidelberg",
                        postal_code = 65156,
                        country = "Germany"
                    }), IdentityServerConstants.ClaimValueTypes.Json)
                }
            },
            new TestUser
            {
                SubjectId="E4D5C951-4425-46B5-A5BD-C3C3A39434BC",
                Username="test",
                Password="test",
                Claims= new List<Claim>
                {
                    new Claim(JwtClaimTypes.Name,"Test Test"),
                    new Claim(JwtClaimTypes.GivenName,"Test"),
                    new Claim(JwtClaimTypes.FamilyName,"Test"),
                    new Claim(JwtClaimTypes.Email,"Test@gmail.com"),
                    new Claim(JwtClaimTypes.Role,"User"),
                    new Claim(JwtClaimTypes.EmailVerified,"true",ClaimValueTypes.Boolean),
                    new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(new
                    {
                        street_address = "One Hacker Way",
                        locality = "Heidelberg",
                        postal_code = 65156,
                        country = "Germany"
                    }), IdentityServerConstants.ClaimValueTypes.Json)
                }
            },
        };
    }
}
