using Application.Core.Domain.Entities;
using Application.Core.Infraestructure.Persistance;
using Application.Core.Infraestructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Rol = Application.Core.Domain.Enums.Rol;

namespace Application.Core.Features.Catalogos.Queries;

/*
* En caso de devolver una lista impletementar la clase tipo List<T> , donde T seria ActividadProductivaQueriResponse
* Ej. IRequest<List<ActividadProductivaQueriResponse>>
*/

public record ObtenerDatosQuerie : IRequest<ObtenerDatosQuerieResponse>;

public class ObtenerDatosQuerieHandler : IRequestHandler<ObtenerDatosQuerie, ObtenerDatosQuerieResponse>
{
    private readonly LoginDefaultDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILlaveCampecheClient _LlaveCampecheClient;


    public ObtenerDatosQuerieHandler(LoginDefaultDbContext context, ICurrentUserService currentUserService,
        ILlaveCampecheClient llaveCampecheClient)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        _LlaveCampecheClient = llaveCampecheClient;
    }

    public async Task<ObtenerDatosQuerieResponse> Handle(ObtenerDatosQuerie request,
        CancellationToken cancellationToken)
    {
        var usuarioActual = await _currentUserService.ObtenerUsuarioActualAsync();
        if (usuarioActual == null)
        {
            throw new Exception("Usuario actual no encontrado");
        }


        var datosLlave = await _LlaveCampecheClient.ObtenerDatosPersonalesAsync(usuarioActual.TokenLlave);

        //TODO: Hay que validar que datosLlave no sea nulo, de lo contrario producirá un error inesperado.

        return new ObtenerDatosQuerieResponse
        {
            Nombre = datosLlave.Content.Nombre,
            Curp = datosLlave.Content.CURP,
            PrimerApellido = datosLlave.Content.PrimerApellido,
            SegundoApellido = datosLlave.Content.SegundoApellido
        };
    }
}

public record ObtenerDatosQuerieResponse
{
    /// <summary>
    /// Nombre del usuario registrado en llave campeche.
    /// </summary>
    public string Nombre { get; set; }

    /// <summary>
    /// Primer apellido del usuario registrado en llave campeche.
    /// </summary>
    public string PrimerApellido { get; set; }

    /// <summary>
    /// Segundo apellido del usuario registrado en llave campeche.
    /// </summary>
    public string SegundoApellido { get; set; }

    /// <summary>
    /// Curp del usuario registrado en llave campeche.
    /// </summary>
    public string Curp { get; set; }


}

/// <summary>
/// Unidad a la que pertenece el usuario. Si la unidad es <code>null</code>, significa que el usuario no tiene una unidad
/// asignada, se encuentra desactivada o no tiene un rol válido, por ejemplo, el rol de administrador y el rol de lector,
/// no tienen una unidad asignada.
/// </summary>
public record Unidad
{
    public string Nombre { get; set; }
    public int IdUnidad { get; set; }
}