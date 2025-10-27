using Application.Core.Domain.Entities;

namespace Application.Core.Infraestructure.Services;

public interface ICurrentUserService
{
    /// <summary>
    /// Método que permite obtener el usuario actual de la sesión,
    /// esto incluye sus propiedades de navegación.
    /// </summary>
    /// <param name="idUsuario"></param>
    /// <returns></returns>
    Task<Usuario> ObtenerUsuarioActualAsync();
}