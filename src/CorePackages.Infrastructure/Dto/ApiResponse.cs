using System.Text.Json.Serialization;

namespace CorePackages.Infrastructure.Dto
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Code { get; set; }

        public T Data { get; set; }
        [JsonConstructor]
        public ApiResponse(bool success, T data)
        {
            Data = data;
            Success = success;
        }

        public ApiResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }
        public ApiResponse(bool success, string message, string code)
        {
            Success = success;
            Message = message;
            Code = code;
        }
    }
}
