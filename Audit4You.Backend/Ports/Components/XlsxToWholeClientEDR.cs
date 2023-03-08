using Audit4You.Backend.Models.Dto;
using Audit4You.Backend.Models.Entities;
using Audit4You.Backend.Models.Enums;

namespace Audit4You.Backend.Ports.Components;

// TODO: port to something better
public class XlsxToWholeClientEDR : IXlsxToWholeClient
{
	public IEnumerable<BankAccountDto> GetAccountsFromXlsx(string filePath)
	{
		throw new NotImplementedException();
		// using var    document = SpreadsheetDocument
		// XSSFWorkbook excelWB  = new XSSFWorkbook(fis);
		//
		// return GetWholeClient(excelWB).GetAccounts();
	}

	/*
	private WholeClient GetWholeClient(XSSFWorkbook wb)
	{
		Sheet sheet = wb.getSheetAt(0);
		for (var i = 0; i < 100; i++)
		{
			Row    row   = sheet.getRow(i);
			Cell   cell  = row.getCell(1);
			string value = cell.getStringCellValue();
			switch (value)
			{
				case "Beginning Balance: ":
					return GetClientFromMyob(wb);
				case "Source":
					return GetClientFromXero(wb);
			}
		}

		return new WholeClient();
	}

	private WholeClient GetClientFromMyob(XSSFWorkbook wb)
	{
		Sheet sheet       = wb.getSheetAt(0);
		var   accountList = new List<BankAccount>();

		var i = 0;
		while (!GetCellData(sheet, i, 5).Equals("Grand Total :"))
		{
			if (GetCellData(sheet, i, 1).Contains('-'))
			{
				accountList.Add(GetAccountFromMyob(sheet, i));
			}

			i++;
		}

		return new WholeClient(accountList);
	}

	//Add class to return BankAccount with current i
	private BankAccount GetAccountFromMyob(Sheet sheet, int cornerRow)
	{
		var transactionList = new List<Transaction>();
		var i               = cornerRow + 2;
		while (!GetCellData(sheet, i, 5).Equals("Total :"))
		{
			var debit = GetCellData(sheet, i, 6);
			if (string.IsNullOrWhiteSpace(debit))
			{
				debit = "0.00";
			}

			var credit = GetCellData(sheet, i, 7);
			if (string.IsNullOrWhiteSpace(credit))
			{
				credit = "0.00";
			}

			transactionList.Add(
				new Transaction(
					GetCellData(sheet, i, 2),
					GetCellData(sheet, i, 3),
					GetCellData(sheet, i, 4),
					GetCellData(sheet, i, 5),
					debit,
					credit
				)
			);
			i++;
		}

		decimal totalDebit  = GetTotal(transactionList, PaymentType.DEBIT);  // TODO: BigDecimal
		decimal totalCredit = GetTotal(transactionList, PaymentType.CREDIT); // TODO: BigDecimal
		decimal totalNet    = totalCredit + totalDebit;                      // TODO: BigDecimal

		return new BankAccount(
			GetCellData(sheet, cornerRow, 2),
			GetCellData(sheet, cornerRow, 1),
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

	private WholeClient GetClientFromXero(XSSFWorkbook wb)
	{
		Sheet sheet       = wb.getSheetAt(0);
		var   accountList = new List<BankAccount>();

		var i = 0;
		while (!GetCellData(sheet, i, 0).Equals("Total"))
		{
			if (GetCellData(sheet, i, 0).Contains("Total"))
			{
				accountList.Add(GetAccountFromXero(sheet, i));
			}

			i++;
		}

		return new WholeClient(accountList);
	}

	private BankAccount GetAccountFromXero(Sheet sheet, int cornerRow)
	{
		var transactionList = new List<Transaction>();

		var i = cornerRow - 1;
		while (!GetCellData(sheet, i, 2).Equals(""))
		{
			var debit  = GetCellData(sheet, i, 4).Replace("-", "");
			var credit = GetCellData(sheet, i, 5).Replace("-", "");
			transactionList.Add(
				new Transaction(
					GetCellData(sheet, i, 3),
					GetCellData(sheet, i, 1),
					GetCellData(sheet, i, 0).Replace(".", "/"),
					GetCellData(sheet, i, 2),
					debit,
					credit
				)
			);
			i--;
		}

		decimal totalDebit  = GetTotal(transactionList, PaymentType.DEBIT);  // TODO: BigDecimal
		decimal totalCredit = GetTotal(transactionList, PaymentType.CREDIT); // TODO: BigDecimal
		decimal totalNet    = totalCredit + totalDebit;                      // TODO: BigDecimal

		return new BankAccount(
			GetCellData(sheet, i, 0),
			"DUMMY ACCOUNT NUM",
			totalDebit,
			totalCredit,
			totalNet,
			transactionList
		);
	}

	private string GetCellData(Sheet sheet, int y, int x)
	{
		Row  row  = sheet.getRow(y);
		Cell cell = row.getCell(x);
		return cell.getCellType() == CellType.NUMERIC ? cell.toString() : cell.getStringCellValue();
	}
	*/
}