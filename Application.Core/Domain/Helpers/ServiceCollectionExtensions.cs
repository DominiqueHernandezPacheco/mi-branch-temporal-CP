using Application.Core.Infraestructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Core.Domain.Helpers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFileStorageService(this IServiceCollection services, IConfiguration config)
    {
        string rutaAlmacenamiento = config["RutaAlmacenamiento"] ?? throw new ArgumentNullException(nameof(config));
        
        string rutaPdfError = config["RutaPdfError"] ?? throw new ArgumentNullException(nameof(config));

        if (!Directory.Exists(rutaAlmacenamiento))
        {
            try
            {
                // Intenta crear el directorio
                Directory.CreateDirectory(rutaAlmacenamiento);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    $"La ruta de almacenamiento proporcionada '{rutaAlmacenamiento}' no es accesible o no se puede crear. Por favor, comprueba la ruta y los permisos.",
                    ex);
            }
        }
        services.AddTransient<IFileStorageService>(_ => new FileStorageService(rutaAlmacenamiento, rutaPdfError));
        
        return services;
    }
}