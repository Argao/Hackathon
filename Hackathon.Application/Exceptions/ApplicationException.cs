namespace Hackathon.Application.Exceptions;

/// <summary>
/// Base exception for application layer errors exposed to outer layers.
/// </summary>
public abstract class ApplicationExceptionBase : Exception
{
    protected ApplicationExceptionBase(string message) : base(message)
    {
    }

    protected ApplicationExceptionBase(string message, Exception innerException) : base(message, innerException)
    {
    }
}

