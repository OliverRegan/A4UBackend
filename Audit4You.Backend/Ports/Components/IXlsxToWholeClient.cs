using Audit4You.Backend.Models.Dto;

namespace Audit4You.Backend.Ports.Components;

public interface IXlsxToWholeClient
{
	IEnumerable<BankAccountDto> GetAccountsFromXlsx(string filePath);
}