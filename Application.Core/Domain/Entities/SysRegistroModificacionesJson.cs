using System;
using System.Collections.Generic;

namespace Application.Core.Domain.Entities;

public partial class SysRegistroModificacionesJson
{
    public int IdLog { get; set; }

    public string? DatosAnteriores { get; set; }

    public virtual SysRegistroModificacione IdLogNavigation { get; set; } = null!;
}
