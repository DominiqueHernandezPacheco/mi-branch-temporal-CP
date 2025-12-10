using System;
using System.Collections.Generic;
namespace Application.Core.Domain.Entities;

public partial class Rol
{
    public int IdRol { get; set; } // Antes PkIdRol
    public string Nombre { get; set; } = null!; // Antes NombreRol
    public string Descripcion { get; set; } = null!;
    public bool? Activo { get; set; } // Antes Estatus

    public virtual ICollection<OpcionRol> CatOpcionesRols { get; set; } = new List<OpcionRol>();
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}