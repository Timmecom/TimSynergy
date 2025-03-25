using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Cosmos;
using TimSynergy.API.Services;

namespace TimSynergy.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowVueApp", 
                    builder => builder
                        .WithOrigins("http://localhost:8080") // Vue.js dev server
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            // Add Authentication
            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "https://login.microsoftonline.com/{tenantId}/v2.0";
                    options.Audience = "api://TimSynergy";
                });

            // Configure Cosmos DB
            builder.Services.AddSingleton(InitializeCosmosClientInstanceAsync(builder.Configuration).GetAwaiter().GetResult());

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowVueApp");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            // Sample endpoint for testing
            app.MapGet("/weatherforecast", () =>
            {
                var summaries = new[]
                {
                    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
                };
                
                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    (
                        DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        Random.Shared.Next(-20, 55),
                        summaries[Random.Shared.Next(summaries.Length)]
                    ))
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecast");

            app.Run();
        }

        private static async Task<ICosmosDbService> InitializeCosmosClientInstanceAsync(IConfiguration configuration)
        {
            string databaseName = configuration["CosmosDb:DatabaseName"] ?? "TimSynergyDB";
            
            try
            {
                string account = configuration["CosmosDb:Endpoint"] ?? "https://localhost:8081";
                string key = configuration["CosmosDb:Key"] ?? "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
                
                CosmosClient client = new CosmosClient(account, key, new CosmosClientOptions
                {
                    ConnectionMode = ConnectionMode.Gateway,
                    ServerCertificateCustomValidationCallback = (_, __, ___, ____) => true
                });
                
                DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
                await database.Database.CreateContainerIfNotExistsAsync("Customers", "/id");
                await database.Database.CreateContainerIfNotExistsAsync("Interactions", "/id");
                
                return new CosmosDbService(client, databaseName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect to CosmosDB: {ex.Message}");
                Console.WriteLine("Using in-memory data service for development...");
                
                // Return an in-memory implementation for development
                return new InMemoryCosmosDbService();
            }
        }
    }

    public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
