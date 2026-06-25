using TraineeManagement.Api.ErrorCodesUtils;

namespace TraineeManagement.Api.ExceptionUtils;
public class NotFoundException : Exception
{
    public readonly int _code;
    public readonly string _message;
    public NotFoundException(ErrorCode message) 
    {
        _code = message.Code;
        _message = message.Message;
    }
}

public class UnauthorizedException : Exception
{
    public readonly int _code;
    public readonly string _message;
    public UnauthorizedException(ErrorCode message) 
    {
        _code = message.Code;
        _message = message.Message;
    }
}

public class BadRequestException : Exception
{
    public readonly int _code;
    public readonly string _message;
    public BadRequestException(ErrorCode message) 
    {
        _code = message.Code;
        _message = message.Message;
    }
}
public class ServerCredentialException : Exception
{
    public readonly int _code;
    public readonly string _message;
    public ServerCredentialException(ErrorCode message) 
    {
        _code = message.Code;
        _message = message.Message;
    }
}

public class QueuingOperationExeception : Exception
{
    public readonly int _code;
    public readonly string _message;
    public QueuingOperationExeception(ErrorCode message) 
    {
        _code = message.Code;
        _message = message.Message;
    }
}

public class JwtOperationException : Exception
{
    public JwtOperationException()  { }
}