using System.Net.Mime;
using Application.Core.Features.Catalogos.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("Api/[controller]")]
public class CatalogosController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CatalogosController> _logger;

    public CatalogosController(IMediator mediator, ILogger<CatalogosController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


    /// <summary>
    /// Roles de usuarios
    /// </summary>
    /// <remarks>Obtiene los posibles roles que un usuario puede tener.</remarks>
    /// <response code="200">Estado</response>
    /// <response code="500">Oops! Ha ocurrido un error en el servidor</response>
    [HttpGet("Roles")]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<List<RolesQuerieResponse>> Roles()
    {
        return await _mediator.Send(new RolesQuerie());
    }

    // 1. Catálogo de Municipios
        [HttpGet("Municipios")]
        public async Task<ActionResult<List<CatalogoDto>>> GetMunicipios()
        {
            return Ok(await _mediator.Send(new MunicipiosQuery()));
        }

        // 2. Catálogo de Tipos de Asentamiento
        [HttpGet("TiposAsentamiento")]
        public async Task<ActionResult<List<CatalogoDto>>> GetTiposAsentamiento()
        {
            return Ok(await _mediator.Send(new TiposAsentamientoQuery()));
        }

        [HttpGet("Ciudades")]
        public async Task<ActionResult<List<CatalogoDto>>> GetCiudades()
        {
            return Ok(await _mediator.Send(new CiudadesQuery()));
        }
    }

