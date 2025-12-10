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
            .Select(x => new AsentamientoDto
            {
                Id = x.IdAsentamiento,
                CodigoPostal = x.CodigoPostal,
                Nombre = x.NombreAsentamiento,
                Oficina = x.COficina,

                // TUS PROTECCIONES CONTRA NULOS (Copy-Paste del controlador)
                Ciudad = x.FkCiudadNavigation != null ? x.FkCiudadNavigation.NombreCiudad : "Zona Rural",
                Municipio = x.FkMunicipioNavigation != null ? x.FkMunicipioNavigation.NombreMunicipio : "Sin Municipio",
                Tipo = x.FkTipoAsentamientoNavigation != null ? x.FkTipoAsentamientoNavigation.NombreTipoAsentamiento : "Desconocido",
                
                Estatus = x.EstatusRegistro == true ? "Activo" : "Inactivo",
                Fecha = x.FechaUltimaModificacion,
                UsuarioModifico = x.FkUsuarioUltimaModNavigation != null ? x.FkUsuarioUltimaModNavigation.NombreUsuario : "Sistema"
            })
            .Take(100)
            .ToListAsync(cancellationToken); // Agregamos el token

        return lista;
    }
}