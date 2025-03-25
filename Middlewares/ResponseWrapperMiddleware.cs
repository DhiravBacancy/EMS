using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EMS.DTOs;
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
        // Store original response body
        var originalBodyStream = context.Response.Body;

        // Create a new memory stream to capture the response body
        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        // Call next middleware/controller
        await _next(context);

        // Get the response type (Content-Type)
        var contentType = context.Response.ContentType;

        // If response is JSON, wrap it
        if (!string.IsNullOrEmpty(contentType) && contentType.Contains("application/json"))
        {
            await WrapJsonResponse(context, memoryStream);
        }
        // If response is a file (Excel, PDF, etc.), let it pass unchanged
        else if (!string.IsNullOrEmpty(contentType) &&
                 (contentType.Contains("application/pdf") ||
                  contentType.Contains("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")))
        {
            // Do nothing, let the file pass through
            memoryStream.Seek(0, SeekOrigin.Begin);
            await memoryStream.CopyToAsync(originalBodyStream);
        }
        else
        {
            // If the response is not JSON or file, just pass it through unchanged
            memoryStream.Seek(0, SeekOrigin.Begin);
            await memoryStream.CopyToAsync(originalBodyStream);
        }
    }

    private async Task WrapJsonResponse(HttpContext context, MemoryStream memoryStream)
    {
        // Read the response body
        memoryStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

        // If the response is empty, provide an empty object (fallback case)
        if (string.IsNullOrEmpty(responseBody))
        {
            responseBody = "{}"; // Empty object for empty responses
        }

        try
        {
            // If response is a TokenResponseDTO, deserialize accordingly
            TokenResponseDTO tokenResponse = null;
            if (context.Response.StatusCode < 400)  // Only deserialize on success
            {
                tokenResponse = JsonSerializer.Deserialize<TokenResponseDTO>(responseBody);
            }

            // Wrap response in a consistent format
            var wrappedResponse = new
            {
                success = context.Response.StatusCode < 400,
                data = tokenResponse,  // If it's valid, return the token response
                error = context.Response.StatusCode >= 400 ? responseBody : null
            };

            // Convert to JSON
            var jsonResponse = JsonSerializer.Serialize(wrappedResponse);
            var responseBytes = Encoding.UTF8.GetBytes(jsonResponse);

            // Reset response body to original stream
            context.Response.Body = memoryStream;
            context.Response.ContentType = "application/json";
            context.Response.ContentLength = responseBytes.Length;  // Ensure correct Content-Length

            // Write wrapped response to the original response body
            await context.Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);
        }
        catch (JsonException ex)
        {
            // Log the error and provide a fallback
            // Log the exception here
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Internal server error");
        }
    }

}
