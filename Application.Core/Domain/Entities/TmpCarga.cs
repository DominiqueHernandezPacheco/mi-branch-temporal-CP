using System;
using System.Collections.Generic;

namespace Application.Core.Domain.Entities;

public partial class TmpCarga
{
    public int? DCodigo { get; set; }

    public string? DAsenta { get; set; }

    public string? DTipoAsenta { get; set; }

    public string? DMnpio { get; set; }

    public string? DEstado { get; set; }

    public string? DCiudad { get; set; }

    public int? DCp { get; set; }

    public int? CEstado { get; set; }

    public int? COficina { get; set; }

    public int? CCp { get; set; }

    public int? CTipoAsenta { get; set; }

    public int? CMnpio { get; set; }

    public int? IdAsentaCpcons { get; set; }

    public string? DZona { get; set; }

    public int? CCveCiudad { get; set; }
}
