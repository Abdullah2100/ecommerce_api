namespace api.application.Result;

public class Result(
    bool isSuccessful,
    string? message,
    object? data,
    int statusCode)
{
    public bool IsSuccessful { get; set; } = isSuccessful;
    public string? Message { get; set; } = message;
    public object? Data { get; set; } = data;
    public int StatusCode { get; set; } = statusCode;
}