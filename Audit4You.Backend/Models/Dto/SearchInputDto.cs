namespace Audit4You.Backend.Models.Dto;

public record SearchInputDto(
    List<BankAccountDto> BankAccounts,
    string StartDate,
    string EndDate,
    double StartAmount,
    double EndAmount,
    string Description
    );