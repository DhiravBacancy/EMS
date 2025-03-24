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
        // Call next middleware/controller first
        await _next(context);

        // Get the response type (Content-Type)
        var contentType = context.Response.ContentType;

        // ✅ **If response is JSON, wrap it**
        if (!string.IsNullOrEmpty(contentType) && contentType.Contains("application/json"))
        {
            await WrapJsonResponse(context);
        }
        // ✅ **If response is a file (Excel, PDF, etc.), let it pass unchanged**
        else if (!string.IsNullOrEmpty(contentType) &&
                 (contentType.Contains("application/pdf") ||
                  contentType.Contains("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")))
        {
            // Do nothing, let the file pass through
        }
    }

    private async Task WrapJsonResponse(HttpContext context)
    {
        // Store original response body
        var originalBodyStream = context.Response.Body;
        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        // Read the response body
        memoryStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();
        memoryStream.Seek(0, SeekOrigin.Begin);

        // Wrap response in a consistent format
        var wrappedResponse = new
        {
            success = context.Response.StatusCode < 400,
            data = context.Response.StatusCode < 400 ? JsonSerializer.Deserialize<object>(responseBody) : null,
            error = context.Response.StatusCode >= 400 ? responseBody : null
        };

        // Convert to JSON
        var jsonResponse = JsonSerializer.Serialize(wrappedResponse);
        var responseBytes = Encoding.UTF8.GetBytes(jsonResponse);

        // Reset response body
        context.Response.Body = originalBodyStream;
        context.Response.ContentType = "application/json";
        context.Response.ContentLength = responseBytes.Length;

        // Write wrapped response
        await context.Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);
    }
}
