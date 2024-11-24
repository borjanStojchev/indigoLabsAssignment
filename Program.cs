
using System.Text.Json.Serialization;
using IndigoLabsAssignment.Authentication;
using IndigoLabsAssignment.Interfaces;
using IndigoLabsAssignment.Models;
using IndigoLabsAssignment.Services;
using Microsoft.OpenApi.Models;

namespace IndigoLabsAssignment;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers()
         .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.AddMemoryCache();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Description = "API Key needed to access the endpoints.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "ApiKey"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "ApiKey"
                        }
                    },
                    new List<string>()
                }
            });
        });

        builder.Services.AddSingleton(sp =>
            builder.Configuration.GetSection("Settings").Get<Settings>());

        builder.Services.AddSingleton<ITemperatureService, TemperatureService>();
        builder.Services.AddHostedService<FileMonitorService>();
        builder.Services.AddScoped<ApiKeyAuthFilter>();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var temperatureService = scope.ServiceProvider.GetRequiredService<ITemperatureService>();
            temperatureService.CalculateCityAverages();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}

