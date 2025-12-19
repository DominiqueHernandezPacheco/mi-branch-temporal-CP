using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Core.Infraestructure.Persistance;
using Application.Core.Features.Asentamientos.DTOs;

namespace Application.Core.Features.Asentamientos.Queries;

public class ObtenerAsentamientosQuery : IRequest<List<AsentamientoDto>> 
{ 
}

public class ObtenerAsentamientosQueryHandler : IRequestHandler<ObtenerAsentamientosQuery, List<AsentamientoDto>>
{
    private readonly LoginDefaultDbContext _context;

    public ObtenerAsentamientosQueryHandler(LoginDefaultDbContext context)
    {
        _context = context;
    }

    public async Task<List<AsentamientoDto>> Handle(ObtenerAsentamientosQuery request, CancellationToken cancellationToken)
    {
        var lista = await _context.CpAsentamientos
            // INCLUDES (Para asegurar que los nombres carguen bien)
            .Include(x => x.FkMunicipioNavigation)
            .Include(x => x.FkCiudadNavigation)
            .Include(x => x.FkTipoAsentamientoNavigation)
            .Include(x => x.FkUsuarioUltimaModNavigation)
            
            // FILTRO VITAL (Para ocultar los "borrados")
            .Where(x => x.EstatusRegistro == true) 

            .Select(x => new AsentamientoDto
            {
                Id = x.IdAsentamiento,
                CodigoPostal = x.CodigoPostal,
                Nombre = x.NombreAsentamiento,
                Oficina = x.COficina,

                // NOMBRES (Para la Tabla)
                Ciudad = x.FkCiudadNavigation != null ? x.FkCiudadNavigation.NombreCiudad : "Zona Rural",
                Municipio = x.FkMunicipioNavigation != null ? x.FkMunicipioNavigation.NombreMunicipio : "Sin Municipio",
                Tipo = x.FkTipoAsentamientoNavigation != null ? x.FkTipoAsentamientoNavigation.NombreTipoAsentamiento : "Desconocido",
                Estatus = x.EstatusRegistro == true ? "Activo" : "Inactivo",

                // IDs y BOOLEANOS (Para Editar)
                // Aquí aplicamos el mismo arreglo que en los otros queries
                IdMunicipio = x.FkMunicipio ?? 0,
                IdCiudad = x.FkCiudad,
                IdTipoAsentamiento = x.FkTipoAsentamiento ?? 0,
                Activo = x.EstatusRegistro ?? false,

                // Auditoría
                Fecha = x.FechaUltimaModificacion,
                UsuarioModifico = x.FkUsuarioUltimaModNavigation != null ? x.FkUsuarioUltimaModNavigation.NombreUsuario : "Sistema"
            })
            .Take(100) // Mantiene tu límite de 100 registros
            .ToListAsync(cancellationToken); 

        return lista;
    }
}