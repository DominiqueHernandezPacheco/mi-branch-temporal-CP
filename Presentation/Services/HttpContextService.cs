using Application.Core.Infraestructure.Services;
using Microsoft.Net.Http.Headers;

namespace Presentacion.Services;

public class HttpContextService : IHttpContextService
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public HttpContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public string ObtenerToken()
    {
        if (_httpContextAccessor is not null && _httpContextAccessor.HttpContext is not null)
        {
            var token = _httpContextAccessor
                .HttpContext
                .Request
                .Headers[HeaderNames.Authorization]
                .ToString().Replace("Bearer ", "");
            
            return token;
        }
        
        return string.Empty;
    }
}