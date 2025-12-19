using Application.Core.Infraestructure.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Core.Features.Catalogos.Queries
{
    // 1. La Petición
    public record TiposAsentamientoQuery : IRequest<List<CatalogoDto>>
    {
    }

    // 2. El Manejador
    public class TiposAsentamientoQueryHandler : IRequestHandler<TiposAsentamientoQuery, List<CatalogoDto>>
    {
        private readonly LoginDefaultDbContext _context;

        public TiposAsentamientoQueryHandler(LoginDefaultDbContext context)
        {
            _context = context;
        }

        public async Task<List<CatalogoDto>> Handle(TiposAsentamientoQuery request, CancellationToken cancellationToken)
        {
            var lista = await _context.CatTiposAsentamientos
                .Select(x => new CatalogoDto
                {
                    Id = x.IdTipoAsentamiento,
                    Nombre = x.NombreTipoAsentamiento
                })
                .OrderBy(x => x.Nombre)
                .ToListAsync(cancellationToken);

            return lista;
        }
    }
}