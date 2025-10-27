using Application.Core.Infraestructure.Persistance;
using Application.Core.Infraestructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Core.Features.Catalogos.Queries;

/*
* En caso de devolver una lista impletementar la clase tipo List<T> , donde T seria RolesQuerieResponse
* Ej. IRequest<List<RolesQuerieResponse>>
*/

public record RolesQuerie : IRequest<List<RolesQuerieResponse>>
{
    //TODO: Implementar datos de entrada en caso de ser requerido 
}

public class RolesQuerieHandler : IRequestHandler<RolesQuerie, List<RolesQuerieResponse>>
{
    private readonly LoginDefaultDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public RolesQuerieHandler(LoginDefaultDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<RolesQuerieResponse>> Handle(RolesQuerie request, CancellationToken cancellationToken)
    {
        var roles = await _context.CatRoles             
                .Where(x => x.Activo == true)
                .Select(roles=> new RolesQuerieResponse
                {
                    IdRol = roles.IdRol,
                    Nombre = roles.Nombre
                })
                .ToListAsync(cancellationToken: cancellationToken);

        return await Task.FromResult(roles);
    }
}

public class RolesQuerieResponse
{
    public int IdRol { get; set; }
    public string Nombre { get; set; }
}