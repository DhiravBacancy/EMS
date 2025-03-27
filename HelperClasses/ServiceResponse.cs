namespace EMS.HelperClasses
{
    public class ServiceResponse<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string FileDownloadName { get; set; } 
        public string ContentType { get; set; }

        public static ServiceResponse<T> SuccessResponse(T data, string message = null, int statusCode = 200, string fileDownloadName = null, string contentType = null)
        {
            return new ServiceResponse<T>
            {
                Data = data,
                Message = message ?? "Operation successful.",
                Success = true,
                StatusCode = statusCode,
                FileDownloadName = fileDownloadName,
                ContentType = contentType
            };
        }
        public static ServiceResponse<T> FailureResponse(string message, int statusCode = 400)
        {
            return new ServiceResponse<T>
            {
                Data = default,
                Message = message,
                Success = false,
                StatusCode = statusCode
            };
        }

        public static ServiceResponse<T> CustomResponse(T data, string message, bool success, int statusCode, string fileDownloadName = null, string contentType = null)
        {
            return new ServiceResponse<T>
            {
                Data = data,
                Message = message,
                Success = success,
                StatusCode = statusCode,
                FileDownloadName = fileDownloadName,
                ContentType = contentType
            };
        }
    }
}
