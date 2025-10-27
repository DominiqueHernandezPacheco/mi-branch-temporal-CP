using System.Net.Mime;
using Application.Core.Features.Catalogos.Queries;
using Application.Core.Features.Usuarios;
using Application.Core.Features.Usuarios.Commands;
using Application.Core.Features.Usuarios.Commands.AgregarEditarUsuario;
using Application.Core.Features.Usuarios.Queries;
using Application.Core.Features.Usuarios.Queries.Modulos;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("Api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IMediator _mediator;

    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(IMediator mediator, ILogger<UsuariosController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger;
    }

    /// <summary>
    /// Inicio sesion LlaveCampeche
    /// </summary>
    /// <remarks>
    /// Código generado desde LlaveCampeche para realizar el cambio por un token de Efirma
    /// </remarks>
    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<TokenCommandResponse> Login([FromQuery] string codigo)
    {
        _logger.LogInformation("Código recibido: {codigo}", codigo);
        return await _mediator.Send(new CrearTokenAccesoCommand { Codigo = codigo });
    }
    
    
    [HttpPost("RefrescarToken")]
    public async Task<RefrescarTokenCommandResponse> RefrescarToken()
    {
        return await _mediator.Send(new RefrescarTokenCommand());
    }

    /// <summary>
    /// Refrescar token
    /// </summary>
    /*[HttpGet("RefrescarToken")]
    public async Task<RefrescarTokenCommandResponse> RefrescarToken()
    {
        return await _mediator.Send(new RefrescarTokenCommand());
    }*/
    [HttpGet("Modulos")]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<List<ModulosResponse>> Modulos()
    {
        return await _mediator.Send(new ModulosQuerie());
    }
    
    /// <summary>
    /// Obtiene datos del usuario
    /// </summary>
    ///<remarks>
    /// Los datos devueltos son los que se encuentran registrados en Llave Campeche. A excepción de la unidad, que es
    /// propio del sistema SEUP.
    /// </remarks>
    /// <response code="500">Oops! Ha ocurrido un error en el servidor</response>
    [HttpGet("Datos")]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<ObtenerDatosQuerieResponse> Datos()
    {
        return await _mediator.Send(new ObtenerDatosQuerie());
    }

    /// <summary>
    ///  Obtiene la lista de usuarios del Sistema SEUP para una tabla paginada.
    /// </summary>
    /// <param name="querie"></param>
    /// <returns></returns>
    [HttpPost("Paginado")]
    public async Task<UsuariosPaginadoQuerieResponse> ObtenerUsuarios( [FromBody] UsuariosPaginadoQuerie querie)
    {
        return await _mediator.Send(querie);
    }

    /// <summary>
    /// Agrega o actualiza un usuario en la Base de Datos
    /// </summary>
    /// <remarks> <b>Nota:</b> Se debe enviar el IdUsuario con cero cuando se desee agregar un nuevo usuario. En caso contrario, se tomará como una modificación. Cuando el usuario no requiera de una Sede entonces el IdSede debe mandarse 0 </remarks>
    /// <param name="usuario"></param>
    /// <returns></returns>
    [HttpPut("AgregarEditar")]
    public async Task<UsuarioResponse> AgregarOModificarUsuario([FromBody] AgregarEditarUsuarioCommand usuario)
    {
        return await _mediator.Send(usuario);
    }


    [HttpGet("BuscarCURPoRFC/{cadena}")]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<ObtenerPorCurpORfcQuerieResponse> BuscarCURPoRFC([FromRoute] string cadena)
    {
        return await _mediator.Send(new ObtenerPorCurpORfcQuerie { CurpORfc = cadena });
    }

    /// <summary>
    /// Elimina un usuario del Sistema SEUP
    /// </summary>
    /// <param name="idUsuario"></param>
    /// <returns></returns>
    [HttpDelete("Eliminar/{idUsuario}")]
    public async Task<UsuarioEliminarResponse> Eliminar([FromRoute] int idUsuario)
    {
        return await _mediator.Send(new  EliminarUsuarioCommand { IdUsuario = idUsuario });
    }
}