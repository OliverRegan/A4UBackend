using Audit4You.Backend.Models.Dto;
using Audit4You.Backend.Models.Entities;

namespace Audit4You.Backend.Ports.Components;

// TODO: port to something better
public class WholeClient
{
	public List<BankAccount> BankAccounts { get; set; }

	public WholeClient() : this(new List<BankAccount>()) { }
	public WholeClient(IEnumerable<BankAccount> bankAccounts) { BankAccounts = bankAccounts.ToList(); }

	public IEnumerable<BankAccountDto> GetAccounts()
	{
		var accountDtoList = new List<BankAccountDto>();
		foreach (var bankAccount in BankAccounts)
		{
			var bankAccountDto = new BankAccountDto(bankAccount);
			bankAccountDto.Transactions.AddRange(bankAccount.Transactions);
			accountDtoList.Add(bankAccountDto);
		}

		return accountDtoList.OrderBy(x => x.Name);
	}
}