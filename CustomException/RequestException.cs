namespace Seer.CustomException;

public class RequestException : Exception
{
    public RequestException() : base("请求错误")
    {
        
    }

    public RequestException(string message) : base(message)
    {
        
    }

    public RequestException(string message, Exception inner) : base(message, inner)
    {
        
    }
    
}