using System;
using System.Collections.Generic;

namespace Application.Core.Domain.Entities;

public partial class CatCiudade
{
    public int IdCiudad { get; set; }

    public int FkEstado { get; set; }

    public string? TipoZona { get; set; }

    public string NombreCiudad { get; set; } = null!;

    public virtual ICollection<CpAsentamiento> CpAsentamientos { get; set; } = new List<CpAsentamiento>();

    public virtual CatEstado FkEstadoNavigation { get; set; } = null!;
}
