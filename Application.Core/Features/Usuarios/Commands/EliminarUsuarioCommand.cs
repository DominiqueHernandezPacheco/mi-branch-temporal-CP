using Application.Core.Domain.Entities;
using Rol = Application.Core.Domain.Enums.Rol;
using Application.Core.Domain.Exceptions;
using Application.Core.Infraestructure.Persistance;
using Application.Core.Infraestructure.Services;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Core.Features.Usuarios.Commands
{

    public class EliminarUsuarioCommand : IRequest<UsuarioEliminarResponse>
    {
        public int IdUsuario { get; set; }
    }

    public class EliminarUsuarioCommandHandler : IRequestHandler<EliminarUsuarioCommand, UsuarioEliminarResponse>
    {
        private readonly LoginDefaultDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public EliminarUsuarioCommandHandler(LoginDefaultDbContext context, ICurrentUserService currentUserService,
         IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _mapper = mapper;
          
        }

        public async Task<UsuarioEliminarResponse> Handle(EliminarUsuarioCommand request, CancellationToken cancellationToken)
        {
            var usuarioActual = await _currentUserService.ObtenerUsuarioActualAsync();

            if (usuarioActual.IdRol != (int)Rol.ADMINISTRADOR)
            {
                throw new ForbiddenAccessException();
            }
            Usuario usuarioSEUP = await _context.Usuarios.Where(x => x.IdUsuario == request.IdUsuario).FirstOrDefaultAsync();

            if (usuarioSEUP == null)
                throw new NotFoundException("El usuario que desea eliminar no existe");

            usuarioSEUP.Activo = false;
            _context.Usuarios.Update(usuarioSEUP);
            await _context.SaveChangesAsync();

            return new UsuarioEliminarResponse
            {
                IdUsuario = usuarioSEUP.IdUsuario
            };
           
        }
    }
    public class UsuarioEliminarResponse
    {
        public int IdUsuario { get; set; } = 0;

    }

}