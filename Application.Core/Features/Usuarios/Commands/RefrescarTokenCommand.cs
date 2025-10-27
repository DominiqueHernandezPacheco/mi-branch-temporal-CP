using Application.Core.Domain.Entities;
using Application.Core.Domain.Helpers;
using Application.Core.Infraestructure.Services;
using MediatR;

namespace Application.Core.Features.Usuarios.Commands;

public record RefrescarTokenCommand : IRequest<RefrescarTokenCommandResponse>;

public sealed class RefrescarTokenCommandHandler : IRequestHandler<RefrescarTokenCommand, RefrescarTokenCommandResponse>
{
    private readonly AutenticacionService _autenticacionService;
    private readonly ILlaveCampecheClient _llaveCampecheClient;
    private readonly ICurrentUserService _currentUserService;


    public RefrescarTokenCommandHandler(
        AutenticacionService autenticacionService,
        ILlaveCampecheClient llaveCampecheClient,
        ICurrentUserService currentUserService)
    {
        _autenticacionService = autenticacionService;
        _llaveCampecheClient = llaveCampecheClient;
        _currentUserService = currentUserService;
    }

    public async Task<RefrescarTokenCommandResponse> Handle(RefrescarTokenCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _currentUserService.ObtenerUsuarioActualAsync();

        var responseApi = await _llaveCampecheClient.RenovarTokenAsync(usuario.TokenLlave);

        if (!responseApi.IsSuccessStatusCode)
        {
            throw new Exception("No se pudo refrescar el token de Llave Campeche");
        }

        var payloadJwtDecodificado = responseApi.Content?.token
            .DecodeBase64()
            .DeserializeObject<TokenJWTDecodificado>();

        var nuevoToken =
            _autenticacionService.GenerarTokenJwt(usuario, payloadJwtDecodificado.email, responseApi.Content.token);

        return new RefrescarTokenCommandResponse
        {
            Token = nuevoToken
        };
    }
}

public record RefrescarTokenCommandResponse
{
    public string Token { get; init; }
}