using System;
using System.Collections.Generic;

namespace Application.Core.Domain.Entities;

public partial class SysRegistroModificacionesJson
{
    // Es la llave primaria y foránea al mismo tiempo (Relación 1 a 1)
    public long IdLog { get; set; }

    // Aquí guardaremos el JSON, pero para la base de datos es solo un texto largo
    public string? DatosAnteriores { get; set; }

    // La conexión con el papá (SysRegistroModificacione)
    public virtual SysRegistroModificacione IdLogNavigation { get; set; } = null!;
}