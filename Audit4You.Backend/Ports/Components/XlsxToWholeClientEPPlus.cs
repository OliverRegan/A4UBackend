using System.Diagnostics;
using Audit4You.Backend.Models.Dto;
using Audit4You.Backend.Models.Entities;
using Audit4You.Backend.Models.Enums;
using Microsoft.EntityFrameworkCore.Storage;
using OfficeOpenXml;

namespace Audit4You.Backend.Ports.Components;

// TODO: port to something better
public class XlsxToWholeClientEPPlus : IXlsxToWholeClient
{
	private const int SearchLimit = 1000000; // TODO: Why is this the limit? Check!

	public IEnumerable<BankAccountDto> GetAccountsFromXlsx(string filePath)
	{
		if (!File.Exists(filePath))
		{
			throw new FileNotFoundException(filePath);
		}
		// Needs to be changed
		ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
		using var package = new ExcelPackage(new FileInfo(filePath));


		var worksheet = package.Workbook.Worksheets[0];

        // Delete file
        try
        {
            if (File.Exists(filePath))
            {
				File.Delete(filePath);
				Console.WriteLine(filePath);
            }
            else
            {
				// do nothing
            }
        } catch(FileNotFoundException e)
        {

        }

		return GetWholeClient(worksheet).GetAccounts();
	}

	private WholeClient GetWholeClient(ExcelWorksheet ws)
	{
		for (var i = 1; i <= 100; i++)
		{
			
			var value = GetCellValueAsString(ws, i, 2);
			Console.WriteLine(value);
			if (value != null && value.StartsWith("Beginning Balance:"))
			{
				return GetClientFromMyob(ws);
			}

			if (value != null && value.StartsWith("Source"))
			{
				return GetClientFromXero(ws);
			}
		}

		return new WholeClient();
	}

	private WholeClient GetClientFromMyob(ExcelWorksheet sheet)
	{
		var accountList = new List<BankAccount>();

		for (var i = 1; i <= SearchLimit; i++)
		{
			if (GetCellValueAsString(sheet, i, 6) != null && GetCellValueAsString(sheet, i, 6).Equals("Grand Total :"))
			{
				return new WholeClient(accountList);
			}

			if (GetCellValueAsString(sheet, i, 2) != null && GetCellValueAsString(sheet, i, 2).Contains('-'))
			{
				accountList.Add(GetAccountFromMyob(sheet, i));
			}
		}

		throw new RetryLimitExceededException(); // TODO: switch to something better
	}

	//Add class to return BankAccount with current i
	private BankAccount GetAccountFromMyob(ExcelWorksheet sheet, int cornerRow)
	{
		var transactionList = new List<Transaction>();

		var i = cornerRow + 2;
		while (!GetCellValueAsString(sheet, i, 6).Equals("Total :"))
		{
			var debit = GetCellValueAsString(sheet, i, 7);
			if (string.IsNullOrWhiteSpace(debit))
			{
				debit = "0.00";
			}

			var credit = GetCellValueAsString(sheet, i, 8);
			if (string.IsNullOrWhiteSpace(credit))
			{
				credit = "0.00";
			}
			transactionList.Add(
				new Transaction(
					GetCellValueAsString(sheet, i, 3) ?? "No externalId provided" ,
					GetCellValueAsString(sheet, i, 4)  ?? "No source provided",
					GetCellValueAsString(sheet, i, 5),
					GetCellValueAsString(sheet, i, 6),
					debit,
					credit
				)
			);
			i++;

			if (i > SearchLimit)
			{
				throw new RetryLimitExceededException(); // TODO: switch to something better
			}
		}

		decimal totalDebit  = GetTotal(transactionList, PaymentType.DEBIT);  // TODO: BigDecimal
		decimal totalCredit = GetTotal(transactionList, PaymentType.CREDIT); // TODO: BigDecimal
		decimal totalNet    =  totalDebit - totalCredit;                      // TODO: BigDecimal

		return new BankAccount(
			GetCellValueAsString(sheet, cornerRow, 3),
			GetCellValueAsString(sheet, cornerRow, 2),
			totalDebit,
			totalCredit,
			totalNet,
			transactionList
		);
	}

	private static decimal GetTotal(List<Transaction> transactions, PaymentType paymentType) // TODO: BigDecimal
	{
		decimal total = 0m; // TODO: BigDecimal
		// TODO: BigDecimal
		total += paymentType == PaymentType.DEBIT
			? transactions.Sum(transaction => decimal.TryParse(transaction.Debit,  out var debit) ? debit : 0m)
			: transactions.Sum(transaction => decimal.TryParse(transaction.Credit, out var credit) ? credit : 0m);

		return total;
	}

	private WholeClient GetClientFromXero(ExcelWorksheet sheet)
	{
		var accountList = new List<BankAccount>();

		for (var i = 1; i <= SearchLimit; i++)
		{
			if (GetCellValueAsString(sheet, i, 1) != null)
			{ 
				if (GetCellValueAsString(sheet, i, 1).Equals("Total"))
				{
					return new WholeClient(accountList);
				}

				if (GetCellValueAsString(sheet, i, 1).Contains("Total"))
				{
					accountList.Add(GetAccountFromXero(sheet, i));
				}
			}
		}

		throw new RetryLimitExceededException(); // TODO: switch to something better
	}

	private BankAccount GetAccountFromXero(ExcelWorksheet sheet, int cornerRow)
	{
		var transactionList = new List<Transaction>();

		var i = cornerRow - 1;
		while (GetCellValueAsString(sheet, i, 3) != null)
		{
			var debit  = GetCellValueAsString(sheet, i, 5);
			var credit = GetCellValueAsString(sheet, i, 6);
			var dateNum = long.Parse(sheet.Cells[i, 1].Value.ToString());
			DateTime date = DateTime.FromOADate(dateNum);
			transactionList.Add(
				new Transaction(
					GetCellValueAsString(sheet, i, 4) ?? "",
					GetCellValueAsString(sheet, i, 2) ?? "No source available",
					date.ToString("dd/MM/yyyy"),
					GetCellValueAsString(sheet, i, 3),
					debit,
					credit
				)
			);
			i--;

			if (i <= 0)
			{
				throw new RetryLimitExceededException(); // TODO: switch to something better
			}
		}

		decimal totalDebit  = GetTotal(transactionList, PaymentType.DEBIT);  // TODO: BigDecimal
		decimal totalCredit = GetTotal(transactionList, PaymentType.CREDIT); // TODO: BigDecimal
		decimal totalNet    = totalCredit + totalDebit;                      // TODO: BigDecimal
		Console.WriteLine(GetCellValueAsString(sheet, i, 2));
		return new BankAccount(
			GetCellValueAsString(sheet, i, 1),
			"Dummy Acc Num",
			totalDebit,
			totalCredit,
			totalNet,
			transactionList
		);
	}

	private static string GetCellValueAsString(ExcelWorksheet sheet, int row, int col)
	{
		Debug.Assert(row > 0);
		Debug.Assert(col > 0);
		return sheet.Cells[row, col].GetCellValue<string>();
	}

}