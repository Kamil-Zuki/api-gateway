using api_gateway.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MMLib.SwaggerForOcelot.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
string routes = string.Empty;
//string? routes = string.Empty;
//if (environment == "Development")
//    routes = "Routes_Dev";
//else

routes = environment == "Development" ? "Routes_Dev" : "Routes_Prod";
#if RELEASE
builder.WebHost.UseUrls("http://*:80");
#endif

builder.Configuration.AddOcelotWithSwaggerSupport(options =>
{
    options.Folder = routes;
});


builder.Services.AddOcelot(builder.Configuration).AddPolly();
builder.Services.AddSwaggerForOcelot(builder.Configuration);

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddOcelot(routes, builder.Environment)
    .AddEnvironmentVariables();


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("cors",
                      policy =>
                      {
                          policy
                          .AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                      });
});



builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseSwagger();

app.UseHttpsRedirection();

app.UseCors("cors");

//app.UseAuthentication();
//app.UseAuthorization();

app.UseSwaggerForOcelotUI(options =>
{
    options.PathToSwaggerGenerator = "/swagger/docs";

    options.ReConfigureUpstreamSwaggerJson = AlterUpstream.AlterUpstreamSwaggerJson;
}).UseOcelot().Wait();


app.MapControllers();

app.Run();