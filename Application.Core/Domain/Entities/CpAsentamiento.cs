using System;
using System.Collections.Generic;

namespace Application.Core.Domain.Entities;

public partial class CpAsentamiento
{
    public int IdAsentamiento { get; set; }

    public string CodigoPostal { get; set; } = null!;

    public string NombreAsentamiento { get; set; } = null!;

    public string? COficina { get; set; }

    public int? FkMunicipio { get; set; }

    public int? FkTipoAsentamiento { get; set; }

    public int? FkCiudad { get; set; }

    public bool? EstatusRegistro { get; set; }

    public DateTime? FechaUltimaModificacion { get; set; }

    public int? FkUsuarioUltimaMod { get; set; }

    public virtual CatCiudade? FkCiudadNavigation { get; set; }

    public virtual CatMunicipio FkMunicipioNavigation { get; set; } = null!;

    public virtual CatTiposAsentamiento FkTipoAsentamientoNavigation { get; set; } = null!;

    public virtual Usuario? FkUsuarioUltimaModNavigation { get; set; }

    public virtual ICollection<SysRegistroModificacione> SysRegistroModificaciones { get; set; } = new List<SysRegistroModificacione>();
}
