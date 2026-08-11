namespace WarehouseWeb.Api.Common;

public abstract class AppException : Exception
{
    public int StatusCode { get; }

    protected AppException(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}

public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(404, message) { }
}

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message) : base(401, message) { }
}

public class ForbiddenException : AppException
{
    public ForbiddenException(string message) : base(403, message) { }
}

public class ConflictException : AppException
{
    public ConflictException(string message) : base(409, message) { }
}

public class ValidationException : AppException
{
    public IReadOnlyList<ValidationError> Errors { get; }

    public ValidationException(IReadOnlyList<ValidationError> errors)
        : base(400, "Validation Error")
    {
        Errors = errors;
    }

    public ValidationException(string path, string message)
        : base(400, "Validation Error")
    {
        Errors = new List<ValidationError> { new ValidationError(path, new[] { message }) };
    }
}

public class UnprocessableException : AppException
{
    public UnprocessableException(string message) : base(422, message) { }
}

public record ValidationError(string Path, IReadOnlyList<string> Messages);
