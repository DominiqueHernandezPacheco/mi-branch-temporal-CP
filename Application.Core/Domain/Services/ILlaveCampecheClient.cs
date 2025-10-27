namespace Application.Core.Infraestructure.Services;

using Application.Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Refit;

/// <summary>
/// Interfaz cliente que se usa para consumir los servicios de Llave Campeche.
/// </summary>
public interface ILlaveCampecheClient
{
    /// <summary>
    /// Método POST para intercambiar el código proporcionado de llave campeche por un token de acceso.
    /// </summary>
    /// <seealso cref="My.Namespace.IMyInterface.InterfaceMethod" />
    [Post("/RSA/cambiarCodigoXToken?code={codigo}")]
    Task<ApiResponse<TokenSesion>> CambiarCodigoPorTokenAsync(string codigo);
    
    /// <summary>
    /// Método GET para validar un token de acceso.
    /// </summary>
    /// <seealso cref="My.Namespace.IMyInterface.InterfaceMethod" />
    [Get("/Acceso/ExisteToken")]
    Task<ApiResponse<string>> ExisteTokenAsync([Authorize] string token);

    /// <summary>
    /// Método GET para obtener un nuevo token de acceso.
    /// </summary>
    /// <seealso cref="My.Namespace.IMyInterface.InterfaceMethod" />
    [Get("/Acceso/RenovarToken")]
    Task<ApiResponse<TokenSesion>> RenovarTokenAsync([Authorize] string token);
    
    /// <summary>
    /// Método GET para obtener los datos personales del usuario actual.
    /// </summary>
    /// <param name="token">Token de Llave Campeche</param>
    /// <returns></returns>
    [Get("/Acceso/ObtenerDatosPersonales")]
    Task<ApiResponse<UsuarioLlave>> ObtenerDatosPersonalesAsync([Authorize] string token);

    /// <summary>
    /// Obtiene los datos personales del usuario de Llave Campeche con de acuerdo a un IdUsuario
    /// </summary>
    /// <param name="token">token de sesión de LlaveCampeche</param>
    /// <param name="idUsuario">El idUsuario del usuario del que se requiere obtener los datos</param>
    /// <returns>Un objeto de tipo UsuarioLlave que toda la información del usuario</returns>
   
    [Get("/Acceso/ObtenerPorIdUsuario/{idUsuario}")]
    Task<ApiResponse<UsuarioLlave>> ObtenerPorIdUsuarioAsync([Authorize] string token, [FromRoute] int idUsuario);

    /// <summary>
    ///  Obtiene los datos personales del usuario de Llave Campeche de acuerdo a una lista de IdUsuario
    /// </summary>
    /// <param name="token"></param>
    /// <param name="idUsuarios">El idUsuario del usuario del que se requiere obtener los datos</param>
    /// <returns>Una lista de tipo UsuarioLlave con toda la información del usuario </returns>
    [Post("/Acceso/ObtenerPorListaIdUsuario")]
    Task<ApiResponse<List<UsuarioLlave>>> ObtenerPorListaIdUsuarioAsync([Authorize] string token, List<int> idUsuarios);
  
    /// <summary>
    /// Método GET para obtener algunos de los datos personales de una lista de usuarios,
    /// esto nos permite validar además si están registrados en Llave Campeche.
    /// </summary>
    /// <param name="token">Token de Llave Campeche</param>
    /// <returns></returns>
    [Post("/Acceso/VerificarRFC")]
    Task<ApiResponse<List<DatosPublicos>>> ObtenerDatosPublicosPorRfcAsync([Authorize] string token, [Body] List<string> rfcs);

    /// <summary>
    /// Método GET para buscar un suuario por CURP o RFC,
    /// esto nos permite validar además si están registrados en Llave Campeche.
    /// </summary>
    /// <param name="token">Token de Llave Campeche</param>
    /// <returns></returns>
    [Post("/Usuarios/ObtenerPorCURPoRFC")]
    Task<ApiResponse<UsuarioLlave>> ObtenerUsuarioPorCURPoRFCAsync([Authorize] string token, string cadena);

}