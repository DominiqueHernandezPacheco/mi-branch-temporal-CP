using Application.Core.Infraestructure.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Core.Features.Catalogos.Queries
{
    // 1. La Petición (Request)
    public record MunicipiosQuery : IRequest<List<CatalogoDto>>
    {
    }

    // 2. El Manejador (Handler)
    public class MunicipiosQueryHandler : IRequestHandler<MunicipiosQuery, List<CatalogoDto>>
    {
        private readonly LoginDefaultDbContext _context;

        public MunicipiosQueryHandler(LoginDefaultDbContext context)
        {
            _context = context;
        }

        public async Task<List<CatalogoDto>> Handle(MunicipiosQuery request, CancellationToken cancellationToken)
        {
            var lista = await _context.CatMunicipios
                .Select(x => new CatalogoDto
                {
                    Id = x.IdMunicipio,
                    Nombre = x.NombreMunicipio
                })
                .OrderBy(x => x.Nombre) // Ordenamos alfabéticamente
                .ToListAsync(cancellationToken);

            return lista;
        }
    }
}