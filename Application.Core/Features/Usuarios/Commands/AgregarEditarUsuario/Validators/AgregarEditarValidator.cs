using Application.Core.Infraestructure.Persistance;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Application.Core.Infraestructure.Services;

namespace Application.Core.Features.Usuarios.Commands.AgregarEditarUsuario.Validators
{
    public class AgregarEditarValidator : AbstractValidator<AgregarEditarUsuarioCommand>
    {
        private readonly LoginDefaultDbContext _context;
        private readonly ILlaveCampecheClient _LlaveCampecheClient;
        private readonly ICurrentUserService _currentUserService;
        public AgregarEditarValidator(LoginDefaultDbContext context, ILlaveCampecheClient llaveCampecheClient, ICurrentUserService currentUserService)
        {
            //Dependencias
            _context = context ?? throw new ArgumentException(nameof(context));
            _LlaveCampecheClient = llaveCampecheClient ?? throw new ArgumentException(nameof(llaveCampecheClient));
            _currentUserService = currentUserService ?? throw new ArgumentException(nameof(currentUserService));
             string tokenLlave =  _currentUserService.ObtenerUsuarioActualAsync().Result.TokenLlave;

           

            //Si el IdUsuario es 0 significa que se va a agregar un nuevo usuario, por lo tanto, se valida que el nuevo usuario no tenga un mismo Id de Usuario de Llave
            When(r => r.IdUsuario == 0, () =>
            {
                //Valida que el usuario de Llave sí exista
                RuleFor(x => x.IdUsuarioLlave)
                        .MustAsync(async (IdUsuarioLlave, token) => await ExisteIdUsuarioLlave(IdUsuarioLlave.Value, token, tokenLlave))
                        .WithMessage("No existe el usuario en Llave Campeche.");

                RuleFor(x => x.IdUsuarioLlave)
                    .MustAsync(
                       async (IdUsuarioLlave, token) => await UsuarioLlaveDuplicado(IdUsuarioLlave, token))
                    .WithMessage("Ya existe un usuario registrado con la misma información.");
            });

            //Si el IdUsuario es mayor a 0 significa que se va a modificar un usuario, por lo tanto, se valida que el usuario exista en la BD
            When(r => r.IdUsuario > 0, () =>
            {

                RuleFor(x => x.IdUsuario)
                   .MustAsync(
                       async (idUsuario, token) => await ExisteUsuarioSEUP(idUsuario, token))
                   .WithMessage("El usuario no se encuntra registrado en el sistema.");
                
            });


            RuleFor(x => x.IdRol)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .NotNull()
                .MustAsync(
                    async (idRol, token) => await ExisteRol(idRol, token))
                .WithMessage("No existe un rol  con el identificador {PropertyValue}.");


        }

        private async Task<bool> UsuarioLlaveDuplicado(int? idUsuarioLlave, CancellationToken token)
        {
            var respuesta = await _context.Usuarios
                .AnyAsync(x => x.IdUsuarioLlave == idUsuarioLlave && x.Activo == true, token);

            return !respuesta;
        }

        private async Task<bool> ExisteUsuarioSEUP(int? idUsuario, CancellationToken token)
        {
            return await _context.Usuarios
                .AnyAsync(x => x.IdUsuario == idUsuario, token);
        }

        private async Task<bool> ExisteRol(int? idRol, CancellationToken token)
        {
            return await _context.CatRoles
                .AnyAsync(x => x.IdRol == idRol, token);
        }

       

        private async Task<bool> ExisteIdUsuarioLlave(int idUsuarioLlave,  CancellationToken token, string tokenLlave)
        {
            if (idUsuarioLlave == 0)
                return false;

            var usuario = await _LlaveCampecheClient.ObtenerPorIdUsuarioAsync(tokenLlave, idUsuarioLlave);

            if (!usuario.IsSuccessStatusCode)
                throw new Exception("No se pudo obtener la información de Llave Campeche");
            
            if (usuario.Content != null)
                return true;
          
            return  false;
        }


    }
}
