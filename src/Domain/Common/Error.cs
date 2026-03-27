namespace AiSoftwareFactory.Domain.Common;

/// <summary>Classifies the nature of a domain error.</summary>
public enum ErrorType
{
    Failure,
    Validation,
    NotFound,
    Conflict
}

/// <summary>Immutable error descriptor used throughout the Result pattern.</summary>
public sealed record Error(string Code, string Description, ErrorType Type)
{
    /// <summary>Creates a not-found error.</summary>
    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    /// <summary>Creates a validation error.</summary>
    public static Error Validation(string code, string description) =>
        new(code, description, ErrorType.Validation);

    /// <summary>Creates a conflict error.</summary>
    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);

    /// <summary>Creates a general failure error.</summary>
    public static Error Failure(string code, string description) =>
        new(code, description, ErrorType.Failure);
}
