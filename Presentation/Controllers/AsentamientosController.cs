using Microsoft.AspNetCore.Mvc;
using MediatR; 
using Application.Core.Features.Asentamientos.Queries; // Para ver el Query
using Application.Core.Features.Asentamientos.Commands;

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
    }
    
}