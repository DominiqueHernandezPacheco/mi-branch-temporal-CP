using System.IdentityModel.Claims;
using System.IdentityModel.Tokens.Jwt;
using Application.Core.Domain.Entities;
using Application.Core.Infraestructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Application.Core.Infraestructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly LoginDefaultDbContext _context;
    private readonly IHttpContextService _httpContextService;

    public CurrentUserService(LoginDefaultDbContext context, IHttpContextService httpContextService)
    {
        _context = context;
        _httpContextService = httpContextService;
    }
    

    public async Task<Usuario> ObtenerUsuarioActualAsync()
    {
        var token = _httpContextService.ObtenerToken();
        
        if (string.IsNullOrEmpty(token))
            throw new Exception("No se ha podido obtener el token del usuario actual");
        
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(token);
        
        var id = int.Parse(jwtSecurityToken.Claims.First(claim => claim.Type == "name").Value);
        var tokenLlave = jwtSecurityToken.Claims.First(claim => claim.Type == "tokenLlave").Value;
        
        var usuario = await _context.Usuarios
            .Include(x => x.Rol)
            .FirstOrDefaultAsync(u => u.IdUsuario == id);
        
        //TODO: Devolver otra execpcion al intentar obtener el usuario actual.
        if (usuario == null)
            throw new Exception("Ha ocurrido un error al obtener el usuario actual");
        
        usuario.Token = token;
        usuario.TokenLlave = tokenLlave;
        
        return usuario;
    }
}