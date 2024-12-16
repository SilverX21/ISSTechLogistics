using System.Net;

namespace ISSTechLogistics.Models.Base;

public class BaseOutput<T> where T : class
{
    public BaseOutput()
    {
        Success = false;
        ErrorMessage = string.Empty;
    }

    public T Output { get; set; }

    public HttpStatusCode Status { get; set; }

    public bool Success { get; set; }

    public string ErrorMessage { get; set; }
}