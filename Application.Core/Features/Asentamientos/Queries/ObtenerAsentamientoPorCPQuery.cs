using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Core.Infraestructure.Persistance;
using Application.Core.Features.Asentamientos.DTOs;

namespace Application.Core.Features.Asentamientos.Queries;

// 1. La Orden (Esta vez lleva un parámetro)
public class ObtenerAsentamientoPorCpQuery : IRequest<List<AsentamientoDto>>
{
    public string CodigoPostal { get; set; } // El dato que necesitamos para buscar

    public ObtenerAsentamientoPorCpQuery(string codigoPostal)
    {
        CodigoPostal = codigoPostal;
    }
}

// 2. (Handler)
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
            .Where(x => x.CodigoPostal == request.CodigoPostal) // Usamos el dato que viene en el request
            .Select(x => new AsentamientoDto
            {
                Id = x.IdAsentamiento,
                CodigoPostal = x.CodigoPostal,
                Nombre = x.NombreAsentamiento,
                Oficina = x.COficina,

                // Protecciones contra nulos (Igual que el anterior)
                Ciudad = x.FkCiudadNavigation != null ? x.FkCiudadNavigation.NombreCiudad : "Zona Rural",
                Municipio = x.FkMunicipioNavigation != null ? x.FkMunicipioNavigation.NombreMunicipio : "Sin Municipio",
                Tipo = x.FkTipoAsentamientoNavigation != null ? x.FkTipoAsentamientoNavigation.NombreTipoAsentamiento : "Desconocido",
                
                Estatus = x.EstatusRegistro == true ? "Activo" : "Inactivo",
                Fecha = x.FechaUltimaModificacion,
                UsuarioModifico = x.FkUsuarioUltimaModNavigation != null ? x.FkUsuarioUltimaModNavigation.NombreUsuario : "Sistema"
            })
            .ToListAsync(cancellationToken);

        return lista;
    }
}