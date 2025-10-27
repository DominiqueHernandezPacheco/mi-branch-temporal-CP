using System;
using System.Collections.Generic;

namespace Application.Core.Domain.Entities;

public partial class Modulo
{
    public int IdModulo { get; set; }

    public string Nombre { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public string Icono { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<Opcion> CatOpciones { get; set; } = new List<Opcion>();
}
