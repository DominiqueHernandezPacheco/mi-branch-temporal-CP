namespace Application.Core.Domain.Enums;

public enum Estado
{
    /// <summary>
    /// Estado que indica que la solicitud ha sido registrada correctamente
    /// </summary>
    REGISTRO = 1,
    /// <summary>
    /// Estado que indica que la solicitud ha sido transferida a una Unidad Administrativa para su dictaminación
    /// </summary>
    TRANSFERIDA = 2,
    
    /// <summary>
    ///  Estado que indica que la solicitud ha sido dictaminada pero aún falta que se suba el Acuse de Repuesta Firmado (Es un estado intermedio)
    /// </summary>
    PORFIMAR = 3,
    /// <summary>
    ///  Estado que indica que la solicitud ha sido cancelada por algún motivo, pero antes de ser dictminada.
    /// </summary>
    CANCELADA = 4,
    /// <summary>
    /// Estado que indica que la solicitud ha sido concluida positivamente (Se dió el apoyo al productor)
    /// </summary>
    CONCLUIDAPOSITIVAMENTE = 5,
    /// <summary>
    /// Estado que indica que la solicitud ha sido concluida negativamente (No se dió el apoyo al productor)
    /// </summary>
    CONCLUIDANEGATIVA = 6,
    /// <summary>
    /// Estado que indica que la solicitud ha sido concluida por orientación (La atención no es del ámbito de la Secretaría)
    /// </summary>
    CONCLUIDAPORORIENTACION = 7,
    /// <summary>
    /// Estado que indica que la solicitud ha sido concluida por desinterés (El solicitante no acude al citario o no proporcionó los elementos solicitados)
    /// </summary>
    CONCLUIDAPORDESINTERES = 8,
    /// <summary>
    /// Estado que indica que la solicitud ha sido dictaminada y ahora está en programas calendarizados
    /// </summary>
    PROGRAMASCALENDARIZADOS = 9,
    /// <summary>
    /// Estado que indica que la solicitud ha sido dictaminada y ahora está en padrón de demanda
    /// </summary>
    PADRONDEMANDA = 10,
    /// <summary>
    /// Estado que indica que la solicitud es para un empleo
    /// </summary>
    BOLSADETRABAJO = 11,
    /// <summary>
    /// Estado que indica que la solicitud ha sido enviada a la fase de atención y análisis para una posterior programación de su resultado
    /// </summary>
    ENPROCESO = 12,

}