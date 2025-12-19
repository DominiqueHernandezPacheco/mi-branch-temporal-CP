using System;
using System.Collections.Generic;

namespace Application.Core.Domain.Entities;

public partial class Usuario
{
    public int IdUsuario { get; set; }
    public int? IdUsuarioLlave { get; set; }
    public int IdRol { get; set; }
    public string NombreUsuario { get; set; } 
    public bool? Activo { get; set; }

    // Propiedades extra
    public string? Token { get; set; }
    public string? TokenLlave { get; set; }

    // Navegaciones
    public virtual Rol Rol { get; set; } = null!;
    public virtual ICollection<CpAsentamiento> CpAsentamientos { get; set; } = new List<CpAsentamiento>();
}