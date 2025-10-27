namespace Application.Core.Domain.Enums;


/// <summary>
/// Tipo de persona que va a realizar la solicitud. Puede usar el valor de la enumeración o
/// el nombre de la enumeración. Ambos son equivalentes.
/// - **1**: Persona física.
/// - **2**: Persona moral.
/// </summary>
public enum TipoPersona
{
    FISICA = 1,
    MORAL = 2,
}