using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Core.Domain.Entities;

public partial class SysRegistroModificacione
{
    public long IdLog { get; set; } // O int, según tu BD
    public int FkUsuario { get; set; }
    public int FkAsentamiento { get; set; }
    public string Accion { get; set; } = null!;
    public DateTime? FechaHora { get; set; }

    // --- BORRA EL [InverseProperty] DE AQUÍ ---
    [ForeignKey("FkAsentamiento")]
    public virtual CpAsentamiento FkAsentamientoNavigation { get; set; } = null!;

    // --- BORRA EL [InverseProperty] DE AQUÍ TAMBIÉN ---
    [ForeignKey("FkUsuario")]
    public virtual Usuario FkUsuarioNavigation { get; set; } = null!;

}