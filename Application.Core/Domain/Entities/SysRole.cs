using System;
using System.Collections.Generic;

namespace Application.Core.Domain.Entities;

public partial class Rol
{
    public int PkIdRol { get; set; }

    public string NombreRol { get; set; } = null!;

    public string? Descriptionn { get; set; }

    public bool? Estatus { get; set; }

    public virtual ICollection<Usuario> SysUsuarios { get; set; } = new List<Usuario>();

    public virtual ICollection<Opcion> FkIdPermisos { get; set; } = new List<Opcion>();
}
