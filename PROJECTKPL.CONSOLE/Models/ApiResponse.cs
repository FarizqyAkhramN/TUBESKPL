using System;
using System.Collections.Generic;
using System.Text;

namespace PROJECTKPL.CONSOLE.Models
{
    // Generic wrapper untuk semua response dari API
    // T bisa berupa: JsonElement, List<JsonElement>, string, dll
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static ApiResponse<T> Ok(T data) => new()
        {
            Success = true,
            Data = data
        };

        public static ApiResponse<T> Fail(string message) => new()
        {
            Success = false,
            Message = message
        };
    }
}
