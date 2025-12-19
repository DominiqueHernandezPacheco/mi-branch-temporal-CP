using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Core.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Application.Core.Infraestructure.Services;

public class AutenticacionService
{
    private readonly IConfiguration _config;

    public AutenticacionService(IConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }
    
    /// <summary>
    /// Genera el token de autenticación
    /// </summary>
    /// <param name="usuario"></param>
    /// <param name="correoElectronico"></param>
    /// <param name="tokenLlave"></param>
    /// <returns>Un token codificado en base 64</returns>
    public string GenerarTokenJwt(Usuario usuario, string correoElectronico, string tokenLlave) 
    {

        var claims = new[]
        {
            new Claim("name", usuario.IdUsuario.ToString()),
            new Claim("email", correoElectronico),
            new Claim("rol", usuario.IdRol.ToString()),
            new Claim("tokenLlave",tokenLlave)
        };
        
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);
        
        var tokenDescriptor = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            claims: claims,
            notBefore: DateTime.Now,
            expires:  DateTime.Now.AddMinutes(Convert.ToInt32(_config["Jwt:Expires"])),
            signingCredentials:  credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        
    }
    

}