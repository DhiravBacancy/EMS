using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class ResponseWrapperMiddleware
{
    private readonly RequestDelegate _next;

    public ResponseWrapperMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // Store the original response body stream
        var originalBodyStream = context.Response.Body;

        // Use a memory stream to temporarily store the response
        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        try
        {
            // Call the next middleware/controller
            await _next(context);

            // Ensure the response type is JSON before wrapping
            if (context.Response.ContentType != null && context.Response.ContentType.Contains("application/json"))
            {
                await ForwardJsonResponse(context, memoryStream, originalBodyStream);
            }
            else
            {
                await CopyResponse(memoryStream, originalBodyStream);
            }
        }
        catch
        {
            // Handle unexpected errors gracefully
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            var errorResponse = JsonSerializer.Serialize(new { success = false, error = "Internal Server Error" });

            await originalBodyStream.WriteAsync(Encoding.UTF8.GetBytes(errorResponse));
        }
        finally
        {
            // Restore the original response stream
            context.Response.Body = originalBodyStream;
        }
    }

    private async Task ForwardJsonResponse(HttpContext context, MemoryStream memoryStream, Stream originalBodyStream)
    {
        // Read the response body
        memoryStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

        // Ensure the Content-Length is correctly updated
        context.Response.Headers.Remove("Content-Length");
        context.Response.ContentType = "application/json";

        // Write the original response to the client without modification
        var responseBytes = Encoding.UTF8.GetBytes(responseBody);
        await originalBodyStream.WriteAsync(responseBytes, 0, responseBytes.Length);
    }

    private async Task CopyResponse(MemoryStream memoryStream, Stream originalBodyStream)
    {
        memoryStream.Seek(0, SeekOrigin.Begin);
        await memoryStream.CopyToAsync(originalBodyStream);
    }
}
