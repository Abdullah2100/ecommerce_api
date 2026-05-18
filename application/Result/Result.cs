namespace api.application.Result;

public record class Result(
    bool IsSuccessful,
    string? Message,
    object? Data,
    int StatusCode)
{
}