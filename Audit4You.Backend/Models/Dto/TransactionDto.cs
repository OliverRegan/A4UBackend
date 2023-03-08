using Audit4You.Backend.Models.Entities;

namespace Audit4You.Backend.Models.Dto;
public record TransactionDto(
	long Id,
	string AccountName,
	string AccountNum,
	string ExternalId,
	string Source,
	string Date,
	string Description,
	string Debit,
	string Credit)
{

	public string GetTransactionType()
    {
		if(decimal.Parse(Debit) > 0)
        {
			return "debit";
        }
        else
        {
			return "credit";
        }
    }
};

