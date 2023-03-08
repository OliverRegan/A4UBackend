
using System.Net;
using System.Text.Json;



namespace Audit4You.Backend.ErrorHandling;

public class ErrorHandling
{

    private readonly RequestDelegate _next;

    public ErrorHandling(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch(Exception error)
        {
            var response = context.Response;
            string message;
            response.ContentType = "application/json";

            switch (error)
            {
                case NullReferenceException e:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    message = e.Message;
                    break;
                case HttpRequestException e:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    message = e.Message;
                    Console.WriteLine("asdsadasdasdasdasdasdasdas");
                    Console.WriteLine(message);
                    break;
                //case NullReferenceException e:
                //    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                //    break;
                //case NullReferenceException e:
                //    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                //    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    message = "Something went wrong.";
                    Console.WriteLine(error.GetObjectData);
                    break;
            }


            await response.WriteAsync(new ErrorDetails()
            {
                StatusCode = response.StatusCode,
                Message = message
            }.ToString());

        }
    }

}