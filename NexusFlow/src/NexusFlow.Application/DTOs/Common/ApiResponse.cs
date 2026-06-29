using System;
using System.Collections.Generic;
using System.Text;

namespace NexusFlow.Application.DTOs.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();
        public int StatusCode { get; set; }

        public static ApiResponse<T> Ok(T data, string message = "Success") =>
            new() { Success = true, Data = data, Message = message, StatusCode = 200 };

        public static ApiResponse<T> Created(T data, string message = "Created") =>
            new() { Success = true, Data = data, Message = message, StatusCode = 201 };

        public static ApiResponse<T> Fail(string error, int statusCode = 400) =>
            new() { Success = false, Errors = new List<string> { error }, StatusCode = statusCode };

        public static ApiResponse<T> Fail(List<string> errors, int statusCode = 400) =>
            new() { Success = false, Errors = errors, StatusCode = statusCode };
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;
    }
}