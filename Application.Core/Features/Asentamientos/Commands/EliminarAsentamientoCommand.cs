using MediatR;
using Application.Core.Infraestructure.Persistance;
using Application.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Core.Features.Asentamientos.Commands
{
    // 1. EL COMANDO (Solo necesitamos el ID)
    public class EliminarAsentamientoCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public EliminarAsentamientoCommand(int id)
        {
            Id = id;
        }
    }

    // 2. EL HANDLER
    public class EliminarAsentamientoCommandHandler : IRequestHandler<EliminarAsentamientoCommand, bool>
    {
        private readonly LoginDefaultDbContext _context;

        public EliminarAsentamientoCommandHandler(LoginDefaultDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(EliminarAsentamientoCommand request, CancellationToken cancellationToken)
        {
            // PASO A: Buscar
            var asentamiento = await _context.CpAsentamientos
                .FirstOrDefaultAsync(x => x.IdAsentamiento == request.Id, cancellationToken);

            if (asentamiento == null)
            {
                throw new Exception("Asentamiento no encontrado.");
            }

            // PASO B: "Borrado Lógico" (Desactivar)
            asentamiento.EstatusRegistro = false; 
            
            // Auditoría
            asentamiento.FechaUltimaModificacion = DateTime.Now;
            asentamiento.FkUsuarioUltimaMod = 1;

            // PASO C: Log
            var log = new SysRegistroModificacione
            {
                Accion = "ELIMINAR", // O "BAJA"
                FechaHora = DateTime.Now,
                FkAsentamiento = asentamiento.IdAsentamiento,
                FkUsuario = 1
            };
            _context.SysRegistroModificaciones.Add(log);

            // PASO D: Guardar
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}