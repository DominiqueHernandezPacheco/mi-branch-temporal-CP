using Microsoft.AspNetCore.Mvc;
using MediatR; 
using Application.Core.Features.Asentamientos.Queries; // Para ver el Query
using Application.Core.Features.Asentamientos.Commands;
using Application.Core.Features.Catalogos.Queries;
using Application.Core.Features.Asentamientos.DTOs;


namespace Presentation.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    public class AsentamientosController : ControllerBase
    {
        private readonly IMediator _mediator; // <-- CAMBIO: Ya no usa DbContext

        public AsentamientosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult> GetAsentamientos()
        {
            // El controlador solo grita: "¡Una orden de Asentamientos!"
            var respuesta = await _mediator.Send(new ObtenerAsentamientosQuery());
            return Ok(respuesta);
        }

        // GET: Api/Asentamientos/Buscar/24050
        [HttpGet("Buscar/{codigoPostal}")]
        public async Task<ActionResult> BuscarPorCP(string codigoPostal)
        {
            // Creamos el paquete del Query con el código postal y se lo damos al Mediator
            var query = new ObtenerAsentamientoPorCpQuery(codigoPostal);
            var respuesta = await _mediator.Send(query);

            if (respuesta.Count == 0) return NotFound("No encontrado");
            
            return Ok(respuesta);
        }


        [HttpPost] // <--- ¡ESTA ES LA ETIQUETA IMPORTANTE!
        public async Task<ActionResult> Crear([FromBody] CrearAsentamientoCommand command)
        {
            try 
            {
                // Enviamos el comando al Handler (la cocina)
                var idNuevo = await _mediator.Send(command);
                
                // Devolvemos éxito con el ID nuevo
                return Ok(new { Id = idNuevo, Mensaje = "Asentamiento creado exitosamente" });
            }
            catch (Exception ex)
            {
                // Si algo falla (ej. base de datos caída), avisamos
                return Problem("Error al crear: " + ex.Message);
            }
        }
        
        [HttpGet("Filtros")]
        public async Task<ActionResult> BuscarConFiltros([FromQuery] ObtenerAsentamientosFiltroQuery filtros)
        {
            var respuesta = await _mediator.Send(filtros);
            return Ok(respuesta);
        }


    [HttpPut("{id}")]
        public async Task<ActionResult> Editar(int id, [FromBody] EditarAsentamientoCommand command)
        {
            command.Id = id; // <--- ESTO ES VITAL. Asigna el ID de la URL al comando oculto.

            try
            {
                await _mediator.Send(command);
                return Ok(new { Mensaje = "Actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }


    [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            try
            {
                await _mediator.Send(new EliminarAsentamientoCommand(id));
                return Ok(new { Mensaje = "Asentamiento eliminado (desactivado) correctamente." });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

         }


    [HttpGet("{id}")]
        public async Task<ActionResult<AsentamientoDto>> ObtenerPorId(int id)
        {
            try 
            {
                var resultado = await _mediator.Send(new ObtenerAsentamientoPorIdQuery(id));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id}/Historial")]
        public async Task<ActionResult<List<HistorialDto>>> ObtenerHistorial(int id)
        {
            var historial = await _mediator.Send(new ObtenerHistorialAsentamientoQuery(id));
            return Ok(historial);
        }
    
    }
}