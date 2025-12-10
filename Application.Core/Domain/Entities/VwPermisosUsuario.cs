using System;
using System.Collections.Generic;

namespace Application.Core.Domain.Entities;

public partial class VwPermisosUsuario
{
    public int IdUsuarios { get; set; }

    public string? IdentificadorExterno { get; set; }

    public string NombreRol { get; set; } = null!;

    public string ClavePermiso { get; set; } = null!;
}
