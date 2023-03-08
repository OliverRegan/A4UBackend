using Audit4You.Backend.Models.Dto;

namespace Audit4You.Backend.Ports.Components;

public interface IMonetaryUnitSampling
{
	TransactionResponseDto GetTransactionsForAudit(SampleInputDto request);

	TransactionResponseDto GetTransactionsForSearch(SearchInputDto request);
}