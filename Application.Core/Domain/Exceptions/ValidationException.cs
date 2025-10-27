using FluentValidation.Results;

namespace Application.Core.Domain.Exceptions;

public class ValidationException : Exception
{
    public ValidationException()
        : base("Uno o más errores de validación han ocurrido.")
    {
        Errores = new List<ValidationError>();
    }
    public ValidationException(string message)
        : base(message)
    {
        Errores = null;

    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
         : this()
    {
        Errores = failures
            .Select(failure => new ValidationError
            {
                Error = failure.PropertyName,
                Detalle = failure.ErrorMessage
            })
            .ToList();
    }

    public List<ValidationError>? Errores { get; }

    public class ValidationError
    {
        public string Error { get; set; } = string.Empty;
        public string Detalle { get; set; } = string.Empty;
    }
}