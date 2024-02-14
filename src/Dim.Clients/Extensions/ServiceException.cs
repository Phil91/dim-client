using System.Net;

namespace Dim.Clients.Extensions;

[Serializable]
public class ServiceException : Exception
{
    public HttpStatusCode? StatusCode { get; }
    public bool IsRecoverable { get; }

    public ServiceException() : base() { }

    public ServiceException(string message, bool isRecoverable = false) : base(message)
    {
        StatusCode = null;
        IsRecoverable = isRecoverable;
    }

    public ServiceException(string message, HttpStatusCode httpStatusCode, bool isRecoverable = false) : base(message)
    {
        StatusCode = httpStatusCode;
        IsRecoverable = isRecoverable;
    }

    public ServiceException(string message, Exception inner, bool isRecoverable = false) : base(message, inner)
    {
        StatusCode = null;
        IsRecoverable = isRecoverable;
    }

    public ServiceException(string message, Exception inner, HttpStatusCode httpStatusCode, bool isRecoverable = false) : base(message, inner)
    {
        StatusCode = httpStatusCode;
        IsRecoverable = isRecoverable;
    }

    protected ServiceException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
