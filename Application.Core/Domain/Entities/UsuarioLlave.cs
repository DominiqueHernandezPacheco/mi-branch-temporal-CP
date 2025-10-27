namespace Application.Core.Domain.Entities;

public class UsuarioLlave
{
    public int IdUsuario { get; set; }
    public int? IdRol { get; set; }
    public int IdSector { get; set; }
    public string Nombre { get; set; }
    public string PrimerApellido { get; set; }
    public string SegundoApellido { get; set; }
    public string CURP { get; set; }
    public string RFC { get; set; }
    public string Telefono { get; set; }
    public string Cargo { get; set; }
    public string Unidad { get; set; }
    public string Ramo { get; set; }
    public string CorreoElectronico { get; set; }
    public int IMunicipio { get; set; }
    public string Calle { get; set; }
    public string Colonia { get; set; }
    public string CodigoPostal { get; set; }
    public string NumExterior { get; set; }
    public string NumInterior { get; set; }
    public string Cruzamiento1 { get; set; }
    public string Cruzamiento2 { get; set; }
    public object Referencia { get; set; }

}


public class DatosPublicos
{
    public string RFC { get; set; }
    public string Nombre { get; set; }
    public string Cargo { get; set; }
    public string Organismo { get; set; }
    public bool Existencia { get; set; }
}