using Audit4You.Backend.Models.Dto;
using Audit4You.Backend.Ports.Components;
using Audit4You.Backend.Ports.Services;
using Audit4You.Backend.Repositories;
using Audit4You.Backend.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Web;

namespace Audit4You.Backend.Controllers;

[ApiController]
[Route("audit")]
public class AuditingController : ControllerBase
{
	private readonly IFileService _fileService;
	private readonly IXlsxToWholeClient _xlsxToWholeClient;

	private readonly IMonetaryUnitSampling _monetaryUnitSampling;
	private readonly IClientRepository _clientRepository;

	public AuditingController(
		IFileService fileService,
		IXlsxToWholeClient xlsxToWholeClient,
		IMonetaryUnitSampling monetaryUnitSampling,
		IClientRepository clientRepository
	)
	{
		_fileService          = fileService;
		_xlsxToWholeClient    = xlsxToWholeClient;
		_monetaryUnitSampling = monetaryUnitSampling;
		_clientRepository     = clientRepository;
	}

	[HttpPost("excel")]
	public async Task<List<BankAccountDto>> UploadXlsxFile([FromForm] IFormFile file)
	{
		var filePath = await _fileService.UploadFileToDisk(file);

		return _xlsxToWholeClient.GetAccountsFromXlsx(filePath).ToList();
	}

	[HttpPost("transactions")]
	public TransactionResponseDto GetTransactionForAudit([FromBody] SampleInputDto request)

	{
        return _monetaryUnitSampling.GetTransactionsForAudit(request);
    }

	[HttpPost("search")]
	public TransactionResponseDto GetTransactionsForSearch([FromBody] SearchInputDto request)

	{
		return _monetaryUnitSampling.GetTransactionsForSearch(request);
	}

	[HttpGet("client")]
	public IActionResult GetClientByName([FromQuery] string name)
	{
		var client = _clientRepository.GetByName(name);
		if (client == null)
		{
			return NotFound();
		}

		return Ok(
			new ClientDto(
				client.Id,
				client.Name,
				client.Abn,
				client.Acn,
				client.Email,
				client.Address,
				client.Phone
			)
		);
	}
}