namespace TP.Domain.Exceptions;

public abstract class ApplicationCustomException : Exception
{
    public ApplicationCustomException(string mesage, params object[] args) : base(mesage)
    {
        Args = args;
    }
    public ApplicationCustomException(string mesage, int statusCode, params object[] args) : base(mesage)
    {
        StatusCode = statusCode;
        Args = args;
    }
    public object[] Args { get; }
    public int? StatusCode { get; set; }
}
public class AppWarningException : ApplicationCustomException
{
    public AppWarningException(string mesage, params object[] args) : base(mesage, args)
    {
    }
    public AppWarningException(string mesage, int statusCode, params object[] args) : base(mesage, statusCode, args)
    {
    }
}
public class AppValidationException(string mesage, params object[] args)
    : ApplicationCustomException(mesage, args)
{
}
