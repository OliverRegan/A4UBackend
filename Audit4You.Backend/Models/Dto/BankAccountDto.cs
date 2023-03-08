using System.Globalization;
using System.Text.Json.Serialization;
using Audit4You.Backend.Models.Entities;

namespace Audit4You.Backend.Models.Dto;

public class BankAccountDto
{
	public string Name { get; init; }
	public string AccountNum { get; init; }
	public string TotalDebit { get; init; }
	public string TotalCredit { get; init; }
	public string TotalNetActivity { get; init; }
	public List<Transaction> Transactions { get; set; } = new List<Transaction>(); // TODO

	[JsonConstructor]
	public BankAccountDto(string name, string accountNum, string totalDebit, string totalCredit,
		string totalNetActivity, List<Transaction> transactions)
	{
		Name             = name;
		AccountNum       = accountNum;
		TotalDebit       = totalDebit;
		TotalCredit      = totalCredit;
		TotalNetActivity = totalNetActivity;
		Transactions = transactions;

	}

	public BankAccountDto(BankAccount entity)
	{
		Name             = entity.Name;
		AccountNum       = entity.AccountNum;
		TotalDebit       = entity.TotalDebit.ToString(CultureInfo.InvariantCulture);
		TotalCredit      = entity.TotalCredit.ToString(CultureInfo.InvariantCulture);
		TotalNetActivity = entity.TotalNetActivity.ToString(CultureInfo.InvariantCulture);
	}

	public bool Equals(BankAccountDto toCompare)
	{
		if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(toCompare.Name))
		{
			return false;
		}

		return Name.Equals(toCompare.Name);
	}
}