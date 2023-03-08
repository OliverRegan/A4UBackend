namespace Audit4You.Backend.Models.Dto;

public record SampleInputDto(
        List<BankAccountDto> BankAccounts,
    long CreditIn,
    long DebitIn,
    long MaterialityIn,
    string SeedCode
    );