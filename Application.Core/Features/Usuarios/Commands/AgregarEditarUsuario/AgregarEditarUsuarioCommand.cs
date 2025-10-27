using Application.Core.Domain.Entities;
using Application.Core.Domain.Exceptions;
using Application.Core.Features.Usuarios.Queries;
using Application.Core.Infraestructure.Persistance;
using Application.Core.Infraestructure.Services;
using AutoMapper;
using FluentValidation;
using MediatR;
using ValidationException = Application.Core.Domain.Exceptions.ValidationException;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rol = Application.Core.Domain.Enums.Rol;

namespace Application.Core.Features.Usuarios.Commands.AgregarEditarUsuario
{
    public class AgregarEditarUsuarioCommand : IRequest<UsuarioResponse>
    {
        public int IdUsuario { get; set; } = 0;
        public int? IdUsuarioLlave { get; set; } = 0;
        public int IdRol { get; set; }

    }

    public class AgregarEditarUsuarioCommandHandler : IRequestHandler<AgregarEditarUsuarioCommand, UsuarioResponse>
    {
        private readonly IValidator<AgregarEditarUsuarioCommand> _registroValidator;
        private readonly LoginDefaultDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILlaveCampecheClient _LlaveCampecheClient;
        private readonly IMapper _mapper;

        public AgregarEditarUsuarioCommandHandler(LoginDefaultDbContext context, ICurrentUserService currentUserService,
            ILlaveCampecheClient llaveCampecheClient, IMapper mapper, IValidator<AgregarEditarUsuarioCommand> registroValidator)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _LlaveCampecheClient = llaveCampecheClient;
            _mapper = mapper;
            _registroValidator = registroValidator;
        }

        public async Task<UsuarioResponse> Handle(AgregarEditarUsuarioCommand request, CancellationToken cancellationToken)
        {

            var usuarioActual = await _currentUserService.ObtenerUsuarioActualAsync();

            if (usuarioActual.IdRol != (int)Rol.ADMINISTRADOR)
            {
                throw new ForbiddenAccessException();
            }
            //Validamos la petición (FLUENT VALIDATION), debe ser una validación asíncrona ya que se consulta la base de datos
            var validationResult = await _registroValidator.ValidateAsync(request, cancellationToken);

            //Si hay errores lo devolvemos en una excepción ValidationException (el filtro se encarga de convertir esta excepción en un BadRequest)
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
           
            Usuario usuarioSEUP = new Usuario();

            //Se valida si se trata de una inserción o una modificación. Si el IdUsuario es mayor a cero entonces es una modificación, en caso contrario es una inserción.
            if (request.IdUsuario > 0)
            {
                usuarioSEUP = await _context.Usuarios.Where(x => x.IdUsuario == request.IdUsuario).FirstOrDefaultAsync();
                usuarioSEUP.IdRol = request.IdRol;
                
                _context.Usuarios.Update(usuarioSEUP);

            }
            else
            {
                usuarioSEUP = await _context.Usuarios.Where(x => x.IdUsuarioLlave == request.IdUsuarioLlave && x.Activo == false).FirstOrDefaultAsync();
                if(usuarioSEUP != null)
                {
                    usuarioSEUP.IdRol = request.IdRol;
                    usuarioSEUP.Activo = true;
                    _context.Usuarios.Update(usuarioSEUP);
                }
                else
                {
                    usuarioSEUP = new Usuario();
                    usuarioSEUP.IdUsuarioLlave = request.IdUsuarioLlave.Value;
                    usuarioSEUP.IdRol = request.IdRol;
                    usuarioSEUP.Activo = true;
                    _context.Usuarios.Add(usuarioSEUP);
                }  
                
            }
              

            await _context.SaveChangesAsync();

            return new UsuarioResponse
            {
                IdUsuario = usuarioSEUP.IdUsuario,
                IdRol = usuarioSEUP.IdRol
            };
            
        }
    }


    public class UsuarioResponse
    {
        public int IdUsuario { get; set; } = 0;
        public int IdRol { get; set; }

    }
}
