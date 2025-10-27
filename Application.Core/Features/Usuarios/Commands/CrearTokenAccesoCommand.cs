using System.Net;
using Application.Core.Domain.Entities;
using Application.Core.Domain.Enums;
using Application.Core.Domain.Helpers;
using Application.Core.Infraestructure.Persistance;
using Application.Core.Infraestructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Core.Features.Usuarios.Commands;

public record CrearTokenAccesoCommand : IRequest<TokenCommandResponse>
{
    /// <summary>
    /// Code enviado desde LlaveCampeche 
    /// </summary>
    public string? Codigo { get; init; }
}

public class CrearTokenAccesoCommandHandler : IRequestHandler<CrearTokenAccesoCommand, TokenCommandResponse>
{
    private readonly ILlaveCampecheClient _llaveCampecheClient;
    private readonly AutenticacionService _autenticacionService;
    private readonly LoginDefaultDbContext _context;


    public CrearTokenAccesoCommandHandler(ILlaveCampecheClient llaveCampecheClient,
        AutenticacionService autenticacionService, LoginDefaultDbContext context)
    {
        _llaveCampecheClient = llaveCampecheClient;
        _autenticacionService = autenticacionService;
        _context = context;
    }

    public async Task<TokenCommandResponse> Handle(CrearTokenAccesoCommand request, CancellationToken cancellationToken)
    {
        var responseApi = await _llaveCampecheClient.CambiarCodigoPorTokenAsync(request.Codigo);

        if (responseApi.StatusCode is not HttpStatusCode.OK)
        {
            throw new Exception($"No se pudo generar el token de acceso: {responseApi.Error}, {responseApi.Error?.Content}");
        }
        
        // Verifica la existencia del Token Obtenido.
        var response = await _llaveCampecheClient.ExisteTokenAsync(responseApi.Content?.token);

        if (response.StatusCode is not HttpStatusCode.OK)
        {
            throw new Exception($"Ha ocurrido un error al validar el token de acceso generado: {response.Error}," +
                                $" {response.Error?.Content} {response.RequestMessage}");
        }
        
        // Obtiene los datos personales del usuario.
        var responseDatosPersonales = await _llaveCampecheClient.ObtenerDatosPersonalesAsync(responseApi.Content?.token);
        
        if (responseDatosPersonales.StatusCode is not HttpStatusCode.OK)
        {
            throw new Exception($"Ha ocurrido un error al obtener los datos personales del usuario: {responseDatosPersonales.Error}," +
                                $" {responseDatosPersonales.Error?.Content} {responseDatosPersonales.RequestMessage}");
        }
        
        // Obtiene el payload del JWT decodificado. 
        var payloadJwtDecodificado = responseApi.Content?.token
            .DecodeBase64()
            .DeserializeObject<TokenJWTDecodificado>();
        
        // Busca al usuario en la base de datos con los datos del token.
        var usuario = _context
            .Usuarios
            .FirstOrDefaultAsync(
                x => x.IdUsuarioLlave == payloadJwtDecodificado.unique_name).Result;

        // Valida si el usuario es nulo para insertarlo en la base de datos
        if (usuario is null)
        {
            if (payloadJwtDecodificado.role == 2)
            {
                usuario = new Usuario
                {
                    IdUsuarioLlave = payloadJwtDecodificado.unique_name,
                    IdRol = (int)Domain.Enums.Rol.ADMINISTRADOR,
                    Activo = true
                };
            }
            else {
                usuario = new Usuario()
                {
                    IdUsuarioLlave = payloadJwtDecodificado.unique_name,
                    IdRol = (int)Domain.Enums.Rol.CIUDADANO,
                    Activo = true
                };
            }
            await _context.AddAsync(usuario, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
       
     
        //if (payloadJwtDecodificado.role != usuario.IdRol)
        //{
        //    usuario.IdRol = payloadJwtDecodificado.role;
        //    await _context.SaveChangesAsync(cancellationToken);
        //}

        var tokenAccesso = _autenticacionService
            .GenerarTokenJwt(usuario, payloadJwtDecodificado.email, responseApi.Content?.token);


        return new TokenCommandResponse()
        {
            Token = tokenAccesso
        };
    }
}

public record TokenCommandResponse
{
    public string Token { get; set; }
}