using Application.Core.Domain.Entities;
using Application.Core.Domain.Exceptions;
using Application.Core.Infraestructure.Persistance;
using Application.Core.Infraestructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Core.Features.Usuarios.Queries;



public record ObtenerPorCurpORfcQuerie : IRequest<ObtenerPorCurpORfcQuerieResponse>
{
    public string CurpORfc { get; set; }
}

public class ObtenerPorCurpORfcQuerieHandler : IRequestHandler<ObtenerPorCurpORfcQuerie, ObtenerPorCurpORfcQuerieResponse>
{
    private readonly LoginDefaultDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILlaveCampecheClient _LlaveCampecheClient;


    public ObtenerPorCurpORfcQuerieHandler(LoginDefaultDbContext context, ICurrentUserService currentUserService, ILlaveCampecheClient llaveCampecheClient)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        _LlaveCampecheClient = llaveCampecheClient;
    }

    public async Task<ObtenerPorCurpORfcQuerieResponse> Handle(ObtenerPorCurpORfcQuerie request, CancellationToken cancellationToken)
    {
        var usuarioActual = await _currentUserService.ObtenerUsuarioActualAsync();
        var datosLlave = await _LlaveCampecheClient.ObtenerDatosPersonalesAsync(usuarioActual.TokenLlave);

        string cadenaBuscar = request.CurpORfc;
                

        var usuario = await _LlaveCampecheClient.ObtenerUsuarioPorCURPoRFCAsync(usuarioActual.TokenLlave, cadenaBuscar);

        if (usuario.Content==null)
        {
            throw new NotFoundException($"El curp {request.CurpORfc} no existe");
        }

        return new ObtenerPorCurpORfcQuerieResponse
        {
            IdUsuario = usuario.Content!.IdUsuario,
            Nombre = usuario.Content!.Nombre,
            PrimerApellido = usuario.Content!.PrimerApellido,
            SegundoApellido = usuario.Content!.SegundoApellido,
            NombreCompleto = usuario.Content!.Nombre +" "+ usuario.Content!.PrimerApellido +(usuario.Content!.SegundoApellido is null?"":" "+ usuario.Content!.SegundoApellido)
        };
    }
}

public class ObtenerPorCurpORfcQuerieResponse
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; }
    public string PrimerApellido { get; set; }
    public string SegundoApellido { get; set; }
    public string NombreCompleto { get; set; }

}
