using MediatR;
using Application.Core.Infraestructure.Persistance;
using Application.Core.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Core.Features.Asentamientos.Commands
{
    // 1. EL COMANDO (Los datos que recibes)
    public class CrearAsentamientoCommand : IRequest<int>
    {
        public string Nombre { get; set; }
        public string CodigoPostal { get; set; }
        public string Oficina { get; set; }
        
        // Aceptamos nulos (int?) por si es un rancho o zona sin municipio/ciudad
        public int? IdMunicipio { get; set; }
        public int? IdCiudad { get; set; }
        public int? IdTipoAsentamiento { get; set; }
    }

    // 2. EL MANEJADOR (La lógica con detector de errores)
    public class CrearAsentamientoCommandHandler : IRequestHandler<CrearAsentamientoCommand, int>
    {
        private readonly LoginDefaultDbContext _context;

        public CrearAsentamientoCommandHandler(LoginDefaultDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CrearAsentamientoCommand request, CancellationToken cancellationToken)
        {
            // PASO A: Preparamos el Asentamiento
            var nuevoAsentamiento = new CpAsentamiento
            {
                NombreAsentamiento = request.Nombre,
                CodigoPostal = request.CodigoPostal,
                COficina = request.Oficina,
                FkMunicipio = request.IdMunicipio,
                FkCiudad = request.IdCiudad,
                FkTipoAsentamiento = request.IdTipoAsentamiento,
                
                // Datos automáticos
                EstatusRegistro = true, 
                FechaUltimaModificacion = DateTime.Now,
                FkUsuarioUltimaMod = 1 // Usuario 1 (SuperAdmin) fijo por ahora
            };

            // PASO B: Intentamos guardar el Asentamiento (TRAMPA #1)
            try 
            {
                _context.CpAsentamientos.Add(nuevoAsentamiento);
                await _context.SaveChangesAsync(cancellationToken); 
            }
            catch (Exception ex)
            {
                // Extraemos el mensaje real de SQL
                var mensajeReal = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                // Lanzamos un error que se verá claro en Swagger
                throw new Exception($"ERROR AL GUARDAR ASENTAMIENTO (Tabla Principal): {mensajeReal}");
            }

            // Si llegamos aquí, el asentamiento se guardó bien y ya tenemos ID nuevo.

            // PASO C: Intentamos guardar el Log (TRAMPA #2)
            try
            {
                var log = new SysRegistroModificacione
                {
                    Accion = "CREAR",
                    FechaHora = DateTime.Now,
                    FkAsentamiento = nuevoAsentamiento.IdAsentamiento, // Usamos el ID nuevo
                    FkUsuario = 1 // El mismo usuario 1
                };

                _context.SysRegistroModificaciones.Add(log);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                var mensajeReal = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                // Si falla aquí, sabremos que es la tabla de Logs la culpable
                throw new Exception($"ERROR AL GUARDAR LOG (Tabla Sys): {mensajeReal}");
            }

            // Si todo salió bien, devolvemos el ID
            return nuevoAsentamiento.IdAsentamiento;
        }
    }
}