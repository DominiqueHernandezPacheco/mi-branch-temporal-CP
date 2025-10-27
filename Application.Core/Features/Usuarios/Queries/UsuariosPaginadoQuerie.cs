using Application.Core.Domain.Entities;
using Application.Core.Infraestructure.Persistance;
using Application.Core.Infraestructure.Services;
using FluentValidation;
using MediatR;
using QueryableExtensions;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using AutoMapper;
using Application.Core.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Rol = Application.Core.Domain.Enums.Rol;

namespace Application.Core.Features.Usuarios.Queries
{
    public record UsuariosPaginadoQuerie : IRequest<UsuariosPaginadoQuerieResponse>
    {
        public string Filtro { get; set; }
        [RegularExpression(@"^[1-9]+[0-9]*$", ErrorMessage = "El numero de pagina debe ser mayor a cero")]
        public int Pagina { get; set; }
        [RegularExpression(@"^[1-9]+[0-9]*$", ErrorMessage = "El numero de elementos debe ser mayor a cero")]
        public int NumElementos { get; set; }
        public int Columna { get; set; }
        public SortOrder Orden { get; set; }
    }


    public class UsuariosPaginadoQuerieHandler : IRequestHandler<UsuariosPaginadoQuerie, UsuariosPaginadoQuerieResponse>
    {

        private readonly LoginDefaultDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILlaveCampecheClient _LlaveCampecheClient;
        private readonly IMapper _mapper;

        public UsuariosPaginadoQuerieHandler(LoginDefaultDbContext context, ICurrentUserService currentUserService,
            ILlaveCampecheClient llaveCampecheClient, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _LlaveCampecheClient = llaveCampecheClient;
            _mapper = mapper;
        }

        public async Task<UsuariosPaginadoQuerieResponse> Handle(UsuariosPaginadoQuerie request, CancellationToken cancellationToken)
        {
            var usuarioActual = await _currentUserService.ObtenerUsuarioActualAsync();

            if (usuarioActual.IdRol != (int)Rol.ADMINISTRADOR)
            {
                throw new ForbiddenAccessException();
            }

            // Se obtiene la lista de usuarios en Llave sin el nombre de UDA O DUDA. 
            List<Usuario> listaPersonal = await _context.Usuarios.
                                            Where(x => x.Activo == true).Include(x => x.Rol).ToListAsync();

            // sacar la lista de IDS de LlaveCamp de los usuarios filtrados
            List<int> listaIdsLlave = new List<int>();
            foreach (var usuario in listaPersonal)
            {
                listaIdsLlave.Add(usuario.IdUsuarioLlave);
            }

            //conexion con llave ------------

            var responseApi = await _LlaveCampecheClient.ObtenerPorListaIdUsuarioAsync(usuarioActual.TokenLlave, listaIdsLlave);
            if (!responseApi.IsSuccessStatusCode)
            {
                throw new Exception("No se pudo obtener la información de Llave Campeche");
            }

            List<UsuarioLlave> usuariosLlave = responseApi.Content.ToList();

            List<DatosUsuario> DatosUsuariosCompletos = new List<DatosUsuario>();

            // Combinar los datos en  DatosUsuario
            foreach (var usuariollave in usuariosLlave)
            {
                Usuario usuarioSEUP = listaPersonal.Where(x => x.IdUsuarioLlave == usuariollave.IdUsuario).FirstOrDefault();
                DatosUsuariosCompletos.Add(await UnirDatosCompletosUsuario(usuariollave, usuarioSEUP, _mapper));
            }

            request.Filtro = request.Filtro.ToUpper();

            KeyValuePair<Expression<Func<DatosUsuario, object>>, SortOrder> sort =
                new KeyValuePair<Expression<Func<DatosUsuario, object>>,
                    QueryableExtensions.SortOrder>(x => x.Id, request.Orden);

            if (request.Columna == 1)
                sort = new KeyValuePair<Expression<Func<DatosUsuario, object>>,
                    QueryableExtensions.SortOrder>(x => x.Nombre, request.Orden);
            if (request.Columna == 2)
                sort = new KeyValuePair<Expression<Func<DatosUsuario, object>>,
                    QueryableExtensions.SortOrder>(x => x.CURP, request.Orden);
            if (request.Columna == 3)
                sort = new KeyValuePair<Expression<Func<DatosUsuario, object>>,
                    QueryableExtensions.SortOrder>(x => x.Rol, request.Orden);



            var (page, countTotal, index) = DatosUsuariosCompletos.AsQueryable().QueryPage(
                page: request.Pagina,
                size: request.NumElementos,
                filter: x => x.Nombre.ToUpper().Contains(request.Filtro)
                             || x.Nombre.ToUpper().Contains(request.Filtro)
                             || x.CURP.ToUpper().Contains(request.Filtro)
                             || x.Rol.ToUpper().Contains(request.Filtro)

                ,
                project: x => x,
                sort);

            return new UsuariosPaginadoQuerieResponse()
            {
                PaginaActual = request.Pagina,
                RegistrosPorPagina = request.NumElementos,
                TotalRegistros = countTotal(),
                TotalPaginas = index.totalPaginas,
                Registros = page.ToList(),
                PrimerIndice = index.primerIndice,
                UltimoIndice = index.ultimoIndice
            };

        }
        public static async Task<DatosUsuario> UnirDatosCompletosUsuario(UsuarioLlave usuarioLlave, Usuario usuarioSEUP, IMapper mapper)
        {


            DatosUsuario usuarioCompleto = mapper.Map<DatosUsuario>(usuarioLlave);
            usuarioCompleto.Id = usuarioSEUP.IdUsuario;
            usuarioCompleto.IdRol = usuarioSEUP.IdRol;
            usuarioCompleto.Rol = usuarioSEUP.Rol.Descripcion;


            return usuarioCompleto;

        }


    }

    public class DatosUsuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string CURP { get; set; }
        public int IdRol { get; set; }
        public string Rol { get; set; }

    }

    public class UsuariosPaginadoQuerieResponse
    {

        public int PaginaActual { get; set; }
        public int RegistrosPorPagina { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
        public int PrimerIndice { get; set; }
        public int UltimoIndice { get; set; }
        public List<DatosUsuario> Registros { get; set; }

    }
}