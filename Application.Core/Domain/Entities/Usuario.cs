using System;
using System.Collections.Generic;

namespace Application.Core.Domain.Entities;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public int IdUsuarioLlave { get; set; }

    public int IdRol { get; set; }

  

    public bool Activo { get; set; }
    public string Token { get; set; } = String.Empty;
    public string TokenLlave { get; set; } = String.Empty;

    public virtual Rol Rol { get; set; } = null!;

}