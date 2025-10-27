using System;
using System.Collections.Generic;

namespace Application.Core.Domain.Entities;

public partial class OpcionRol
{
    public int IdOpcionRol { get; set; }

    public int IdRol { get; set; }

    public int IdOpcion { get; set; }

    public bool Activo { get; set; }

    public virtual Opcion IdOpcionNavigation { get; set; }

    public virtual Rol IdRolNavigation { get; set; }
}
