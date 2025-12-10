using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; // <--- OBLIGATORIO

namespace Application.Core.Domain.Entities;

public partial class SysRegistroModificacione
{
    public int IdLog { get; set; } // O long
    public int FkUsuario { get; set; }
    public int FkAsentamiento { get; set; }
    public string Accion { get; set; } = null!;
    public DateTime? FechaHora { get; set; }

    // --- AQUÍ ESTÁ LA SOLUCIÓN MÁGICA ---

    [ForeignKey("FkAsentamiento")]
    [InverseProperty("SysRegistroModificaciones")] // <--- Conecta con la lista en CpAsentamiento
    public virtual CpAsentamiento FkAsentamientoNavigation { get; set; } = null!;

    [ForeignKey("FkUsuario")]
    [InverseProperty("SysRegistroModificaciones")] // <--- Conecta con la lista en Usuario
    public virtual Usuario FkUsuarioNavigation { get; set; } = null!;

    public virtual SysRegistroModificacionesJson? SysRegistroModificacionesJson { get; set; }
}