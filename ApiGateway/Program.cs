using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication().AddJwtBearer(builder.Configuration["Authentication:AuthenticationProviderKey"], x =>
{
    x.Authority = builder.Configuration["Authentication:IdentityServer"];
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false
    };
});

#region Ocelot

builder.Services.AddOcelot(builder.Configuration);
builder.Configuration.AddJsonFile("ocelot.json");

#endregion Ocelot

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


#region Ocelot

await app.UseOcelot();

#endregion Ocelot

await app.RunAsync();