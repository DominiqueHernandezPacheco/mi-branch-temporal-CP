using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Core.Infraestructure.Persistance;
using Application.Core.Features.Asentamientos.DTOs;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace Application.Core.Features.Asentamientos.Queries
{
    // 1. EL QUERY (Pide un ID)
    public class ObtenerAsentamientoPorIdQuery : IRequest<AsentamientoDto>
    {
        public int Id { get; set; }

        public ObtenerAsentamientoPorIdQuery(int id)
        {
            Id = id;
        }
    }

    // 2. EL HANDLER
    public class ObtenerAsentamientoPorIdQueryHandler : IRequestHandler<ObtenerAsentamientoPorIdQuery, AsentamientoDto>
    {
        private readonly LoginDefaultDbContext _context;

        public ObtenerAsentamientoPorIdQueryHandler(LoginDefaultDbContext context)
        {
            _context = context;
        }

        public async Task<AsentamientoDto> Handle(ObtenerAsentamientoPorIdQuery request, CancellationToken cancellationToken)
        {
            var item = await _context.CpAsentamientos
                .Include(x => x.FkMunicipioNavigation)
                .Include(x => x.FkCiudadNavigation)
                .Include(x => x.FkTipoAsentamientoNavigation)
                .Include(x => x.FkUsuarioUltimaModNavigation)
                .FirstOrDefaultAsync(x => x.IdAsentamiento == request.Id, cancellationToken);

            if (item == null)
            {
                throw new Exception("Asentamiento no encontrado");
            }

            // Mapeamos TODO (Nombres para ver, IDs para editar)
            return new AsentamientoDto
            {
                Id = item.IdAsentamiento,
                CodigoPostal = item.CodigoPostal,
                Nombre = item.NombreAsentamiento,
                Oficina = item.COficina,
                
                // Nombres
                Ciudad = item.FkCiudadNavigation != null ? item.FkCiudadNavigation.NombreCiudad : "Sin Ciudad",
                Municipio = item.FkMunicipioNavigation.NombreMunicipio,
                Tipo = item.FkTipoAsentamientoNavigation.NombreTipoAsentamiento,
                Estatus = item.EstatusRegistro == true ? "Activo" : "Inactivo",

                // IDs y Booleanos (Vitales para tu Modal)
                IdMunicipio = item.FkMunicipio.Value, // Asumimos que tiene valor por ser obligatorio
                IdCiudad = item.FkCiudad,
                IdTipoAsentamiento = item.FkTipoAsentamiento.Value,
                Activo = item.EstatusRegistro ?? false, // Convertimos null a false por seguridad

                Fecha = item.FechaUltimaModificacion,
                UsuarioModifico = item.FkUsuarioUltimaModNavigation != null ? item.FkUsuarioUltimaModNavigation.NombreUsuario : "Sistema"
            };
        }
    }
}