using Application.Core.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentation.Filters;

public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    private readonly ILogger<ApiExceptionFilterAttribute> _logger;

    public ApiExceptionFilterAttribute(ILogger<ApiExceptionFilterAttribute> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Manejador de excepciones que interviene cuando ocurre una excepción en el contexto de una acción de controlador.
    /// </summary>
    /// <param name="context">Contexto de la excepción.</param>
    public override void OnException(ExceptionContext context)
    {
        var logLevel = LogLevel.Error;

        // Procesa diferentes tipos de excepciones y configura la respuesta adecuada
        switch (context.Exception)
        {
            case ValidationException validationEx:

                logLevel = LogLevel.Debug;
                HandleValidationException(context, validationEx);
                break;

            case NotFoundException notFoundEx:

                logLevel = LogLevel.Debug;
                HandleNotFoundException(context, notFoundEx);
                break;

            case ForbiddenAccessException:

                logLevel = LogLevel.Warning;
                HandleForbiddenAccessException(context);
                break;

            case UnathorizedException:

                logLevel = LogLevel.Warning;
                HandleUnauthorizedAccessException(context);
                break;

            case ArgumentException argumentEx:
                HandleArgumentException(context, argumentEx);
                break;

            default:
                HandleUnknownException(context);
                break;
        }

        // Registrar la excepción
        _logger.Log(logLevel, context.Exception, "{message} en {@Result}", context.Exception.Message, context.Result);

        base.OnException(context);
    }

    /// <summary>
    /// Maneja excepciones de validaciones de los datos de una solicitud.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="exception"></param>
    private void HandleValidationException(ExceptionContext context, ValidationException exception)
    {
        object details;

        if (exception.Errores is not null && exception.Errores.Any())
        {

            details = new
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Se han detectado errores en los datos ingresados.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Detail = exception.Errores.Select(e => e.Detalle).ToList(),
            };

            context.Result = new BadRequestObjectResult(details);
            context.ExceptionHandled = true;
        }
        else
        {
            details = new
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Se han detectado errores en los datos ingresados.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Detail = new string[] { exception.Message },
            };

            context.Result = new BadRequestObjectResult(details);
            context.ExceptionHandled = true;
        }
    }

    /// <summary>
    /// Maneja excepciones resultantes de un estado de modelo inválido.
    /// </summary>
    /// <param name="context">Contexto de la excepción.</param>
    private void HandleInvalidModelStateException(ExceptionContext context)
    {
        var details = new
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Se ha producido un error.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Detail = new string[] { "Si el error persiste, póngase en contacto con Soporte." },


        };

        context.Result = new BadRequestObjectResult(details);
        context.ExceptionHandled = true;
    }

    /// <summary>
    /// Maneja excepciones de recursos no encontrados
    /// </summary>
    /// <param name="context"></param>
    /// <param name="exception"></param>
    private void HandleNotFoundException(ExceptionContext context, NotFoundException exception)
    {
        var details = new
        {
            Status = StatusCodes.Status404NotFound,
            Title = "No se pudieron encontrar algunos datos para completar su solictud.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Detail = new string[] { exception.Message }
        };

        context.Result = new NotFoundObjectResult(details);

        context.ExceptionHandled = true;
    }

    /// <summary>
    /// Maneja excepciones de argumentos no válidos
    /// </summary>
    /// <param name="context"></param>
    /// <param name="exception"></param>
    private void HandleArgumentException(ExceptionContext context, ArgumentException exception)
    {

        string[] detail;

        // Verificar subtipos de ArgumentException
        switch (exception)
        {
            case ArgumentNullException argNullEx:
                // "Falta un argumento requerido.";
                detail = new[] { argNullEx.Message.Split('(')[0] }; //$"Parámetro: {argNullEx.ParamName}" 
                break;

            case ArgumentOutOfRangeException argOutOfRangeEx:
                //"El argumento está fuera del rango permitido.";
                detail = new[] { argOutOfRangeEx.Message.Split('(')[0] }; //$"Parámetro: {argOutOfRangeEx.ParamName}" 
                break;

            default:
                //"Error en los argumentos proporcionados.";
                detail = new[] { exception.Message };
                break;
        }

        var details = new
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Error al intentar procesar los datos de su solicitud.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Detail = detail//new string [] { exception.Message }
        };

        context.Result = new BadRequestObjectResult(details);

        context.ExceptionHandled = true;
    }


    /// <summary>
    /// Maneja excepciones de permisos, generalmente cuando un rol no tiene acceso a un recurso.
    /// </summary>
    /// <param name="context"></param>
    private void HandleForbiddenAccessException(ExceptionContext context)
    {
        var details = new
        {
            Status = StatusCodes.Status403Forbidden,
            Title = "Acceso denegado.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            Detail = new string[] { "Verifique sus permiso del sistema." }
        };

        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status403Forbidden
        };

        context.ExceptionHandled = true;
    }

    /// <summary>
    /// Maneja excepciones de credenciales de sesión inválidas, generalmente cuando los datos de sesión no son válidos.
    /// </summary>
    /// <param name="context"></param>
    private void HandleUnauthorizedAccessException(ExceptionContext context)
    {
        var details = new
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Acceso no autorizado.",
            Type = "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1",
            Detail = new string[] { "Credenciales de sesión no válidas." }
        };

        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status401Unauthorized
        };

        context.ExceptionHandled = true;
    }

    /// <summary>
    /// Maneja excepciones desconocidas, generalmente errores internos del servidor.
    /// </summary>
    /// <param name="context">Contexto de la excepción.</param>
    private void HandleUnknownException(ExceptionContext context)
    {
        if (!context.ModelState.IsValid)
        {
            HandleInvalidModelStateException(context);
            return;
        }

        var details = new
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Se ha producido un error al procesar su solicitud.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Detail = new string[] { "Por favor, intente de nuevo más tarde o contacte a Soporte." }
        };

        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };

        context.ExceptionHandled = true;
    }



}