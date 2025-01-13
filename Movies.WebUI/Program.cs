using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Movies.WebUI.ApiServices;
using Movies.WebUI.HttpHandlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthentication(opts =>
{
    opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    opts.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, opts =>
    {
        opts.Authority = builder.Configuration["Authentication:IdentityServer"]!;
        opts.ClientId = builder.Configuration["Authentication:ClientId"]!;
        opts.ClientSecret = builder.Configuration["Authentication:ClientSecret"]!;
        opts.ResponseType = builder.Configuration["Authentication:ResponseType"]!;

        var scopes = builder.Configuration.GetSection("Authentication:Scopes").Get<List<string>>()!;

        foreach (var scope in scopes)
        {
            opts.Scope.Add(scope);
        }
        opts.ClaimActions.MapUniqueJsonKey("role", "role", "role");

        opts.MapInboundClaims = false;
        opts.SaveTokens = true;
        opts.GetClaimsFromUserInfoEndpoint = true;
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = JwtClaimTypes.GivenName,
            RoleClaimType = JwtClaimTypes.Role,
        };
    });

builder.Services.AddTransient<AuthenticationDelegatingHandler>();

builder.Services.AddHttpClient("MoviesAPIClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Authentication:ApiAddress"]!);
    client.DefaultRequestHeaders.Clear();
    client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
}).AddHttpMessageHandler<AuthenticationDelegatingHandler>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient("IDPClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Authentication:IdentityServer"]!);
    client.DefaultRequestHeaders.Clear();
    client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
});

//builder.Services.AddSingleton(new ClientCredentialsTokenRequest
//{
//    Address = builder.Configuration["Authentication:TokenAddress"]!,
//    ClientId = builder.Configuration["Authentication:ApiClientId"]!,
//    ClientSecret = builder.Configuration["Authentication:ClientSecret"]!,
//    Scope = builder.Configuration["Authentication:ApiScope"]!
//});

builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddScoped<IMovieApiService, MovieApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Movie}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();