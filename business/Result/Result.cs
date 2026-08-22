namespace api.application;

public record class Result(
    bool IsSuccessful,
    string? Message,
    object? Data,
    int StatusCode);