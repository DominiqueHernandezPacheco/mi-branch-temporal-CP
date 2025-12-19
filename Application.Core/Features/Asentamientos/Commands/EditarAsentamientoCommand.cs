using MediatR;
using Application.Core.Infraestructure.Persistance;
using Application.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json; // <--- NUEVO: Para convertir a texto
using System.Text.Json.Serialization; 
using System.Threading;
using System.Threading.Tasks;

namespace Application.Core.Features.Asentamientos.Commands
{
    // 1. EL COMANDO (Igual que el tuyo)
    public class EditarAsentamientoCommand : IRequest<bool>
    {
        [JsonIgnore]
        public int Id { get; set; }

        public string CodigoPostal { get; set; }
        public string Nombre { get; set; }
        public int IdMunicipio { get; set; }
        public int? IdCiudad { get; set; }
        public int IdTipoAsentamiento { get; set; }
        public bool Activo { get; set; }
        public string Oficina { get; set; }
    }

    // 2. EL HANDLER (Con la lógica del JSON)
    public class EditarAsentamientoCommandHandler : IRequestHandler<EditarAsentamientoCommand, bool>
    {
        private readonly LoginDefaultDbContext _context;

        public EditarAsentamientoCommandHandler(LoginDefaultDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(EditarAsentamientoCommand request, CancellationToken cancellationToken)
        {
            // A. BUSCAR
            var asentamiento = await _context.CpAsentamientos
                .Include(x => x.FkMunicipioNavigation) // Para nombres en el JSON
                .FirstOrDefaultAsync(x => x.IdAsentamiento == request.Id, cancellationToken);

            if (asentamiento == null) throw new Exception("No encontrado");

            // B. TOMAR LA FOTO (JSON) 📸
            var opcionesJson = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles, WriteIndented = false };
            string jsonAnterior = JsonSerializer.Serialize(asentamiento, opcionesJson);

            

           // C. DETECTAR CAMBIOS
            var cambios = new List<string>();

            // Texto y CP (Estos ya estaban bien)
            if (asentamiento.CodigoPostal != request.CodigoPostal)
                cambios.Add($"CP: '{asentamiento.CodigoPostal}' -> '{request.CodigoPostal}'");

            if (asentamiento.NombreAsentamiento != request.Nombre)
                cambios.Add($"Nombre: '{asentamiento.NombreAsentamiento}' -> '{request.Nombre}'");

            // --- AQUÍ EL CAMBIO: Usamos IDs para que el formato sea "Campo: A -> B" ---
            
            if (asentamiento.FkMunicipio != request.IdMunicipio)
                cambios.Add($"Municipio: '{asentamiento.FkMunicipio}' -> '{request.IdMunicipio}'");

            if (asentamiento.FkTipoAsentamiento != request.IdTipoAsentamiento)
                cambios.Add($"Tipo: '{asentamiento.FkTipoAsentamiento}' -> '{request.IdTipoAsentamiento}'");

            if (asentamiento.EstatusRegistro != request.Activo)
                cambios.Add($"Estatus: '{asentamiento.EstatusRegistro}' -> '{request.Activo}'");

            string resumenCambios = cambios.Count > 0 ? string.Join(" | ", cambios) : "Sin cambios aparentes";


            // D. ACTUALIZAR (Sobrescribir datos)
            asentamiento.CodigoPostal = request.CodigoPostal;
            asentamiento.NombreAsentamiento = request.Nombre;
            asentamiento.FkMunicipio = request.IdMunicipio;
            asentamiento.FkCiudad = request.IdCiudad;
            asentamiento.FkTipoAsentamiento = request.IdTipoAsentamiento;
            asentamiento.EstatusRegistro = request.Activo;
            if (!string.IsNullOrEmpty(request.Oficina)) asentamiento.COficina = request.Oficina;

            asentamiento.FechaUltimaModificacion = DateTime.Now;
            asentamiento.FkUsuarioUltimaMod = 1;

            // E. GUARDAR LOGS
            var logPrincipal = new SysRegistroModificacione
            {
                // AQUÍ GUARDAMOS EL RESUMEN PARA EL HOVER 👇
                Accion = $"EDITAR: {resumenCambios}", 
                FechaHora = DateTime.Now,
                FkAsentamiento = asentamiento.IdAsentamiento,
                FkUsuario = 1
            };

            var logJson = new SysRegistroModificacionesJson
            {
                DatosAnteriores = jsonAnterior, // Y AQUÍ EL RESPALDO COMPLETO
                IdLogNavigation = logPrincipal
            };

            _context.SysRegistroModificaciones.Add(logPrincipal);
            _context.Set<SysRegistroModificacionesJson>().Add(logJson);

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}