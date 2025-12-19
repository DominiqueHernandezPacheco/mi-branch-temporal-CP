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
    // 1. QUERY
    public class ObtenerAsentamientoPorCpQuery : IRequest<List<AsentamientoDto>>
    {
        public string CodigoPostal { get; set; }

        public ObtenerAsentamientoPorCpQuery(string codigoPostal)
        {
            CodigoPostal = codigoPostal;
        }
    }

    // 2. HANDLER
    public class ObtenerAsentamientoPorCpQueryHandler : IRequestHandler<ObtenerAsentamientoPorCpQuery, List<AsentamientoDto>>
    {
        private readonly LoginDefaultDbContext _context;

        public ObtenerAsentamientoPorCpQueryHandler(LoginDefaultDbContext context)
        {
            _context = context;
        }

        public async Task<List<AsentamientoDto>> Handle(ObtenerAsentamientoPorCpQuery request, CancellationToken cancellationToken)
        {
            var lista = await _context.CpAsentamientos
                .Include(x => x.FkMunicipioNavigation)
                .Include(x => x.FkCiudadNavigation)
                .Include(x => x.FkTipoAsentamientoNavigation)
                .Include(x => x.FkUsuarioUltimaModNavigation) // Incluimos usuario para que no falle el mapeo
                .Where(x => x.CodigoPostal == request.CodigoPostal) 
                .Select(x => new AsentamientoDto
                {
                    Id = x.IdAsentamiento,
                    CodigoPostal = x.CodigoPostal,
                    Nombre = x.NombreAsentamiento,
                    Oficina = x.COficina,

                    // NOMBRES (Para ver)
                    Ciudad = x.FkCiudadNavigation != null ? x.FkCiudadNavigation.NombreCiudad : "Sin Ciudad",
                    Municipio = x.FkMunicipioNavigation != null ? x.FkMunicipioNavigation.NombreMunicipio : "Sin Municipio",
                    Tipo = x.FkTipoAsentamientoNavigation != null ? x.FkTipoAsentamientoNavigation.NombreTipoAsentamiento : "Desconocido",
                    Estatus = x.EstatusRegistro == true ? "Activo" : "Inactivo",

                    // --- AQUÍ ESTABA EL FALTANTE ---
                    // IDs y Booleanos (Para editar)
                    IdMunicipio = x.FkMunicipio ?? 0, 
                    IdCiudad = x.FkCiudad,
                    IdTipoAsentamiento = x.FkTipoAsentamiento ?? 0,
                    Activo = x.EstatusRegistro ?? false, 

                    // Auditoría
                    Fecha = x.FechaUltimaModificacion,
                    UsuarioModifico = x.FkUsuarioUltimaModNavigation != null ? x.FkUsuarioUltimaModNavigation.NombreUsuario : "Sistema"
                })
                .ToListAsync(cancellationToken);

            return lista;
        }
    }
}