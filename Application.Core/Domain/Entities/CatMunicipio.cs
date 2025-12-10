using System;
using System.Collections.Generic;

namespace Application.Core.Domain.Entities;

public partial class CatMunicipio
{
    public int IdMunicipio { get; set; }

    public int FkEstado { get; set; }

    public string NombreMunicipio { get; set; } = null!;

    public virtual ICollection<CpAsentamiento> CpAsentamientos { get; set; } = new List<CpAsentamiento>();

    public virtual CatEstado FkEstadoNavigation { get; set; } = null!;
}
