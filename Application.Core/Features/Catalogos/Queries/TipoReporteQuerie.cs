using Application.Core.Domain.Entities;
using Application.Core.Infraestructure.Persistance;
using Application.Core.Infraestructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Core.Features.Catalogos.Queries;

/*
* En caso de devolver una lista impletementar la clase tipo List<T> , donde T seria SexoQueriResponse
* Ej. IRequest<List<SexoQueriResponse>>
*/

public record TipoReporteQuerie : IRequest<List<TipoReporteQuerieResponse>>
{

}

public class TipoReporteQuerieHandler : IRequestHandler<TipoReporteQuerie, List<TipoReporteQuerieResponse>>
{
    private readonly LoginDefaultDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public TipoReporteQuerieHandler(LoginDefaultDbContext context, ICurrentUserService currentUserService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    public async Task<List<TipoReporteQuerieResponse>> Handle(TipoReporteQuerie request, CancellationToken cancellationToken)
    {
        var usuarioActual = await _currentUserService.ObtenerUsuarioActualAsync();

        /*var tipos = await
         _context.CatTiposReportes
         .Where(x => x.Activo == true)
         .Select(tipos => new TipoReporteQuerieResponse
         {
             IdTipoReporte = tipos.IdTipoReporte,
             Nombre = tipos.Nombre,
             Activo = tipos.Activo
         })
                .ToListAsync(cancellationToken: cancellationToken);*/
        //Se colocó new List<TipoReporteQuerieResponse>() para que no marque error, modificar de acuerdo a lo que se nececesite
        return await Task.FromResult(new List<TipoReporteQuerieResponse>());
    }
}

public class TipoReporteQuerieResponse
{
    public int IdTipoReporte { get; set; }

    public string Nombre { get; set; }

    public bool Activo { get; set; }
}

