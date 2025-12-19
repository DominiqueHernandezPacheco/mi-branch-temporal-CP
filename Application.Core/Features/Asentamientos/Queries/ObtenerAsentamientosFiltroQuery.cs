using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Core.Infraestructure.Persistance;
using Application.Core.Features.Asentamientos.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Core.Features.Asentamientos.Queries
{
    // 1. PARÁMETROS (IDs para catálogos, String para nombres)
    public class ObtenerAsentamientosFiltroQuery : IRequest<List<AsentamientoDto>>
    {
        // OBLIGATORIOS
        public int IdMunicipio { get; set; }        
        public int IdTipoAsentamiento { get; set; } 
        public string Asentamiento { get; set; } // Nombre (ej. "Centro")

        // OPCIONAL (Puede ser nulo)
        public int? IdCiudad { get; set; } 
    }

    // 2. HANDLER
    public class ObtenerAsentamientosFiltroQueryHandler : IRequestHandler<ObtenerAsentamientosFiltroQuery, List<AsentamientoDto>>
    {
        private readonly LoginDefaultDbContext _context;

        public ObtenerAsentamientosFiltroQueryHandler(LoginDefaultDbContext context)
        {
            _context = context;
        }

        public async Task<List<AsentamientoDto>> Handle(ObtenerAsentamientosFiltroQuery request, CancellationToken cancellationToken)
        {
            // LIMPIEZA DEL TEXTO
            var nombreAsentamiento = request.Asentamiento?.Trim();

            // VALIDACIÓN: Los 3 obligatorios deben tener valor
            if (request.IdMunicipio <= 0 || 
                request.IdTipoAsentamiento <= 0 || 
                string.IsNullOrWhiteSpace(nombreAsentamiento))
            {
                return new List<AsentamientoDto>(); // Retorna vacío si falta algo obligatorio
            }

            var query = _context.CpAsentamientos
                .Include(x => x.FkMunicipioNavigation)
                .Include(x => x.FkCiudadNavigation)
                .Include(x => x.FkTipoAsentamientoNavigation)
                .Include(x => x.FkUsuarioUltimaModNavigation)
                .Where(x => x.EstatusRegistro == true)
                .AsQueryable();

            // --- APLICAMOS FILTROS ---

            // 1. Municipio (ID Exacto)
            query = query.Where(x => x.FkMunicipio == request.IdMunicipio);

            // 2. Tipo (ID Exacto)
            query = query.Where(x => x.FkTipoAsentamiento == request.IdTipoAsentamiento);

            // 3. Asentamiento (Texto - Búsqueda flexible)
            query = query.Where(x => x.NombreAsentamiento.Contains(nombreAsentamiento));

            // 4. Ciudad (ID Exacto - OPCIONAL)
            if (request.IdCiudad.HasValue && request.IdCiudad > 0)
            {
                query = query.Where(x => x.FkCiudad == request.IdCiudad.Value);
            }

            // PROYECCIÓN
            var resultados = await query
                .Select(x => new AsentamientoDto
                {
                    Id = x.IdAsentamiento,
                    CodigoPostal = x.CodigoPostal,
                    Nombre = x.NombreAsentamiento,
                    Oficina = x.COficina,
                    // Si es nulo, mostramos "Sin Ciudad"
                    Ciudad = x.FkCiudadNavigation != null ? x.FkCiudadNavigation.NombreCiudad : "Sin Ciudad",
                    Municipio = x.FkMunicipioNavigation.NombreMunicipio,
                    Tipo = x.FkTipoAsentamientoNavigation.NombreTipoAsentamiento,
                    Estatus = x.EstatusRegistro == true ? "Activo" : "Inactivo",
                    Fecha = x.FechaUltimaModificacion,
                    UsuarioModifico = x.FkUsuarioUltimaModNavigation != null ? x.FkUsuarioUltimaModNavigation.NombreUsuario : "Sistema"
                })
                .Take(50) 
                .ToListAsync(cancellationToken);

            return resultados;
        }
    }
}