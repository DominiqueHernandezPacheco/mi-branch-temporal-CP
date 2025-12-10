using System;
using System.Collections.Generic;
namespace Application.Core.Domain.Entities;

public partial class Usuario
{
    public int IdUsuario { get; set; } // Antes IdUsuarios
    public int? IdUsuarioLlave { get; set; } // Antes IdentificadorExterno (ajusta tipo int/string según corresponda)
    public int IdRol { get; set; } // Antes FkRol
    public string NombreUsuario { get; set; } 
    public bool? Activo { get; set; } // Antes Estatus

    // Propiedades extra que necesita tu App
    public string? Token { get; set; }
    public string? TokenLlave { get; set; }

    public virtual Rol Rol { get; set; } = null!;
    // Relaciones inversas (puedes dejarlas o comentarlas si dan error)
    public virtual ICollection<SysRegistroModificacione> SysRegistroModificaciones { get; set; } = new List<SysRegistroModificacione>();
}