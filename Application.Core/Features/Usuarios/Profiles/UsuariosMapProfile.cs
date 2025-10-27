using Application.Core.Domain.Entities;
using Application.Core.Features.Usuarios.Queries;
using Application.Core.Features.Usuarios.Queries.Modulos;
using AutoMapper;

namespace Application.Core.Profiles;

public class UsuariosMapProfile: Profile
{
    public UsuariosMapProfile()
    {
        //TODO: Agregar los mapeos de los perfiles de los usuarios
        CreateMap<UsuarioLlave, DatosUsuario>()
              .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdUsuario))
              .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => $"{src.Nombre} {src.PrimerApellido}  {src.SegundoApellido}"))
              .ForMember(dest => dest.CURP, opt => opt.MapFrom(src => src.CURP));
    }
    
}