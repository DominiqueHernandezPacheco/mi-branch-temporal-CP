namespace Application.Core.Features.Asentamientos.DTOs;

public class AsentamientoDto
{
    public int Id { get; set; }
    public string CodigoPostal { get; set; }
    public string Nombre { get; set; }
    public string Oficina { get; set; }
    public string Ciudad { get; set; }
    public string Municipio { get; set; }
    public string Tipo { get; set; }
    public string Estatus { get; set; }
    public DateTime? Fecha { get; set; }
    public string UsuarioModifico { get; set; }
}