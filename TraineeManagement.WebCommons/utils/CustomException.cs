using TraineeManagement.WebCommons.ErrorCodesUtils;

namespace TraineeManagement.WebCommons.ExceptionUtils;
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

public class ForbiddenException : Exception
{
    public readonly int _code;
    public readonly string _message;
    public ForbiddenException(ErrorCode message) 
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
public class InterServiceOperationExeception : Exception
{
    public readonly int _code;
    public readonly string _message;
    public InterServiceOperationExeception(ErrorCode message) 
    {
        _code = message.Code;
        _message = message.Message;
    }
}

public class DataBaseOperationFailed : Exception
{
    public readonly Exception _ex;
    public readonly int _code;
    public readonly string _message;
    public DataBaseOperationFailed(Exception ex , ErrorCode message) 
    {
        _ex = ex;
        _code = message.Code;
        _message = message.Message;
    }
}

public class JwtOperationException : Exception
{
    public JwtOperationException()  { }
}