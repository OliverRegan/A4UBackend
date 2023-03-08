namespace Audit4You.Backend.Models.Dto;

public record TransactionResponseDto(
    List<TransactionDto>? Transactions = null,
    string? SeedCode = null,
    string? SamplePercentage = null,
    string? SampleInterval = null
    );