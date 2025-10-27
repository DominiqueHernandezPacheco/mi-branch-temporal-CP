namespace Application.Core.Domain.Entities;

public class TokenJWTDecodificado
{
    public int unique_name { get; set; }
    public int name { get; set; }
    public string email { get; set; }
    public int role { get; set; }
    public string? tokenLlave { get; set; }
    public string nbf { get; set; }
    public string exp { get; set; }
    public string iat { get; set; }
}