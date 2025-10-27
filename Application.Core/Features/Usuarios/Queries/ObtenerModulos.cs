using Application.Core.Infraestructure.Persistance;
using Application.Core.Infraestructure.Services;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Application.Core.Features.Usuarios.Queries.Modulos
{
    public record ModulosQuerie : IRequest<List<ModulosResponse>>;


    public sealed class ModulosQuerieHandler : IRequestHandler<ModulosQuerie, List<ModulosResponse>>
    {
        private readonly LoginDefaultDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public ModulosQuerieHandler(LoginDefaultDbContext context, ICurrentUserService currentUserService, IMapper mapper)
        {
            _context = context;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<List<ModulosResponse>> Handle(ModulosQuerie request, CancellationToken cancellationToken)
        {
            var usuario = await _currentUserService.ObtenerUsuarioActualAsync();
            
            var modulosActivos = await _context.CatOpcionesRols
                .Include(x => x.IdOpcionNavigation)
                .ThenInclude(y => y.IdModuloNavigation)
                .Where(r => r.IdRol == usuario.IdRol
                            && r.Activo
                            && r.IdOpcionNavigation.Activo
                            && r.IdOpcionNavigation.IdModuloNavigation.Activo)
                .GroupBy(r => r.IdOpcionNavigation.IdModuloNavigation)
                .Select(group => new ModulosResponse
                {
                    IdModulo = group.Key.IdModulo,
                    Nombre = group.Key.Nombre,
                    Descripcion = group.Key.Descripcion,
                    Icono = group.Key.Icono,
                    CatOpciones = group
                        .Select(opcion => new OpcionResponse
                        {
                            IdOpcion = opcion.IdOpcion,
                            IdModulo = opcion.IdOpcionNavigation.IdModulo,
                            Nombre = opcion.IdOpcionNavigation.Nombre,
                            Descripcion = opcion.IdOpcionNavigation.Descripcion,
                            Url = opcion.IdOpcionNavigation.Url,
                            Icono = opcion.IdOpcionNavigation.Icono,
                            Activo = opcion.Activo
                        }).Distinct().ToList()
                }).ToListAsync(cancellationToken: cancellationToken);

            return modulosActivos;
        }
    }


    public record ModulosResponse
    {
        public int IdModulo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Icono { get; set; }
        public List<OpcionResponse> CatOpciones { get; set; }
    }

    public record OpcionResponse
    {
        public int IdOpcion { get; set; }

        public int IdModulo { get; set; }
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public string Url { get; set; }

        public string Icono { get; set; }
        public bool Activo { get; set; }
    }
}
