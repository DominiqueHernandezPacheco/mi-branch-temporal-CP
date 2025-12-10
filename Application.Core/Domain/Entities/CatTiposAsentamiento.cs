using System;
using System.Collections.Generic;

namespace Application.Core.Domain.Entities;

public partial class CatTiposAsentamiento
{
    public int IdTipoAsentamiento { get; set; }

    public string NombreTipoAsentamiento { get; set; } = null!;

    public virtual ICollection<CpAsentamiento> CpAsentamientos { get; set; } = new List<CpAsentamiento>();
}
