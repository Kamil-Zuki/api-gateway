using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace glosslore_api_gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            

            builder.Configuration.AddJsonFile("ocelot.json");

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddOcelot();


            var app = builder.Build();

            //app.UseSwagger();
            app.UseSwaggerUI();

            app.UseRouting();
            app.UseHttpsRedirection();

            app.UseOcelot().Wait();

            app.MapControllers();



            app.Run();
        }
    }
}