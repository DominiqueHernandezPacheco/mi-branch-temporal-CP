using Application.Core.Infraestructure.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Core.Features.Catalogos.Queries
{
    public record CiudadesQuery : IRequest<List<CatalogoDto>>;

    public class CiudadesQueryHandler : IRequestHandler<CiudadesQuery, List<CatalogoDto>>
    {
        private readonly LoginDefaultDbContext _context;

        public CiudadesQueryHandler(LoginDefaultDbContext context)
        {
            _context = context;
        }

        public async Task<List<CatalogoDto>> Handle(CiudadesQuery request, CancellationToken cancellationToken)
        {
            var lista = await _context.CatCiudades
                .Select(x => new CatalogoDto
                {
                    Id = x.IdCiudad,
                    Nombre = x.NombreCiudad
                })
                .OrderBy(x => x.Nombre)
                .ToListAsync(cancellationToken);

            return lista;
        }
    }
}