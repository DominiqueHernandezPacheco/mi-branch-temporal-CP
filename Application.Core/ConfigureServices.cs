using System.Reflection;
using System.Text;
using Application.Core.Domain.Helpers;
using Application.Core.Features.Usuarios.Commands.AgregarEditarUsuario.Validators;
using Application.Core.Infraestructure.Persistance;
using Application.Core.Infraestructure.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Refit;

namespace Application.Core;

public static class ConfigureServices
{
    public static IServiceCollection AddAplicationCore(this IServiceCollection services)
    {
        services.AddScoped(typeof(AutenticacionService));
        
        services.AddMediatR(Assembly.GetExecutingAssembly());
        return services;
    }
    public static IServiceCollection AddInfraestructure(this IServiceCollection services, IConfiguration config)
    {
        // Configuración del cliente de llave Campeche.
         services.AddRefitClient<ILlaveCampecheClient>()
             .ConfigureHttpClient(client =>
             {
                 client.BaseAddress = new Uri(config["LlaveCampeche:Url"]);
                 client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
             });
        
        // Conexion a BD
         services.AddDbContext<LoginDefaultDbContext>(options => options.UseSqlServer(config.GetConnectionString("DefaultConnection") ));

        //Registra todos los validadores del assembly actual ('Application.Core')
        // services.AddValidatorsFromAssemblyContaining<RegistrarProductorValidator>();
        services.AddValidatorsFromAssemblyContaining<AgregarEditarValidator>();

        // Registra automapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
         
         //Registra el servicio para interacturar con el sistema de archivos.
         services.AddFileStorageService(config);
         
         services.AddTransient<ICurrentUserService, CurrentUserService>();
         
        return services;
    }
    public static IServiceCollection AddSecurity(this IServiceCollection services,  IConfiguration config)
    {
        services.AddAuthorization();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"])),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });
        
        return services;
    }
}
