using System;
using System.Collections.Generic;

namespace Application.Core.Domain.Entities;

public partial class Usuario
{
    public int IdUsuarios { get; set; }

    public string NombreUser { get; set; } = null!;

    public string? IdentificadorExterno { get; set; }

    public int FkRol { get; set; }

    public bool? Estatus { get; set; }

    public virtual ICollection<CpAsentamiento> CpAsentamientos { get; set; } = new List<CpAsentamiento>();

    public virtual Rol FkRolNavigation { get; set; } = null!;

    public virtual ICollection<SysRegistroModificacione> SysRegistroModificacion { get; set; } = new List<SysRegistroModificacione>();
}
