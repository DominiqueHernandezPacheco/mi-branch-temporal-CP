namespace Application.Core.Domain.Enums;

public enum Rol
{

    /// <summary>
    /// Rol para la persona encargada para administrar el sistema.
    /// </summary>
    ADMINISTRADOR = 1,

    /// <summary>
    /// Rol para la persona que es la encargada de supervisar ciertas actividades del sistema.
    /// </summary>
    SUPERVISOR = 2,

    /// <summary>
    /// Rol para la persona que solo podrá consultar y ver la información de los productores y solicitudes
    /// </summary>
    CIUDADANO = 3,
    
    
   
}