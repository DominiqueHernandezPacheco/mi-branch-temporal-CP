using System;
using System.Collections.Generic;

namespace Application.Core.Domain.Entities;

public partial class CatEstado
{
    public int IdEstado { get; set; }

    public string NombreEstado { get; set; } = null!;

    public virtual ICollection<CatCiudade> CatCiudades { get; set; } = new List<CatCiudade>();

    public virtual ICollection<CatMunicipio> CatMunicipios { get; set; } = new List<CatMunicipio>();
}
