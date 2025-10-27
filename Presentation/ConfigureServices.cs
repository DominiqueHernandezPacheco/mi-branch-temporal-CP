using System.Globalization;
using System.Reflection;
using Application.Core.Infraestructure.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Presentacion.Services;
using Presentation.Filters;

namespace Presentacion;

public static class ConfigureServices
{
    private static string ReadDescriptionFromFile(string path)
    {
        var ruta = $"{Directory.GetCurrentDirectory()}/{path}";

        if (!File.Exists(ruta))
        {
            return string.Empty;
        }

        return File.ReadAllText(ruta);
    }

    public static IServiceCollection AddPresentationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = config["Documentation:Title"] ?? "Api",
                Description = ReadDescriptionFromFile(config["Documentation:DescriptionPath"]),
                Contact = new OpenApiContact
                {
                    Name = "Administrador",
                    Email = config["Documentation:EmailContact"],
                },
                Extensions = new Dictionary<string, IOpenApiExtension>
                {
                    {
                        "x-logo", new OpenApiObject
                        {
                            { "url", new OpenApiString(config["Documentation:Logo"]) },
                            { "altText", new OpenApiString(config["Documentation:Dependencia"]) }
                        }
                    }
                }
            });


            options.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Porfavor inserta tu JWT Bearer en este campo",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    new List<string>()
                }
            });

            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Presentation.xml"));
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Application.Core.xml"));
        });
        services.AddHttpContextAccessor();
        services.AddTransient<IHttpContextService, HttpContextService>();

        services.AddControllers(options =>
            options.Filters.Add<ApiExceptionFilterAttribute>()).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(
                    new System.Text.Json.Serialization.JsonStringEnumConverter());
                options.JsonSerializerOptions.ReferenceHandler =
                    System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            }
        ).AddNewtonsoftJson(options =>
        {
            /*
             * Cambia el formato de las fechas (DateTime) al formato indicado.
             * Se aplican al enviar la respuesta. 
             */
            options.SerializerSettings.DateFormatString = "dd/MM/yyyy hh:mm:ss tt";

            options.SerializerSettings.Culture = new CultureInfo("es-MX")
            {
                DateTimeFormat = { ShortDatePattern = "dd/MM/yyyy", LongTimePattern = "hh:mm:ss tt" }
            };
        });


        services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

        return services;
    }
}