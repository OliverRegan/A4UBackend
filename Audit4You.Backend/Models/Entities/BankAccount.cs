namespace Audit4You.Backend.Models.Entities;

public record BankAccount(
	string Name,
	string AccountNum,
	decimal TotalDebit,       // todo: BigDecimal
	decimal TotalCredit,      // todo: BigDecimal
	decimal TotalNetActivity, // todo: BigDecimal
	IReadOnlyList<Transaction> Transactions
);