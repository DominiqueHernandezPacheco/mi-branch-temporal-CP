using System;
using System.Collections.Generic;

namespace Application.Core.Domain.Entities;

public partial class Opcion
{
    public int PkIdPermiso { get; set; }

    public string ClavePermiso { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Rol> FkIdRols { get; set; } = new List<Rol>();
}
