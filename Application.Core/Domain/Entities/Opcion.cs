using System;
using System.Collections.Generic;

namespace Application.Core.Domain.Entities;

public partial class Opcion
{
    public int IdOpcion { get; set; }

    public int IdModulo { get; set; }

    public string Nombre { get; set; }

    public string Descripcion { get; set; }

    public string Url { get; set; }

    public string Icono { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<OpcionRol> CatOpcionesRols { get; set; } = new List<OpcionRol>();

    public virtual Modulo? IdModuloNavigation { get; set; }
}
