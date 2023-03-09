using Audit4You.Backend.Models.Dto;

namespace Audit4You.Backend.Ports.Components;

// TODO: port to something better
public class MonetaryUnitSampling : IMonetaryUnitSampling
{

	public TransactionResponseDto GetTransactionsForAudit(SampleInputDto request)
	{
		
		if(request.BankAccounts.Count() == 0)
        {
			throw new NullReferenceException("You need to select accounts before you can sample transactions");
        }
		var transactionDtoList = GetIndividualTransactions(request.BankAccounts);
		return DoSampling(transactionDtoList, request);
	}
	public TransactionResponseDto GetTransactionsForSearch(SearchInputDto request)
	{

		if (request.BankAccounts.Count() == 0)
		{
			throw new NullReferenceException("You need to select accounts before you can sample transactions");
		}
		var transactionDtoList = GetIndividualTransactions(request.BankAccounts);
		return DoSearching(transactionDtoList, request);
	}

	private List<TransactionDto> GetIndividualTransactions(IEnumerable<BankAccountDto> bankAccountDtoList)
	{
		return (from bankAccountDto in bankAccountDtoList
			from transaction in bankAccountDto.Transactions
			select new TransactionDto(
				transaction.Id,
				bankAccountDto.Name,
				bankAccountDto.AccountNum,
				transaction.ExternalId != null ? transaction.ExternalId : "No external id present",
				transaction.Source != null ? transaction.Source : "No source present",
				transaction.Date,
				transaction.Description,
				transaction.Debit,
				transaction.Credit
			)).ToList();
	}

	private TransactionResponseDto DoSearching(List<TransactionDto> transactionDtoList,
		SearchInputDto request)
	{

		// Use search request to narrow down transactions list

		List<TransactionDto> filtered = new List<TransactionDto>();

		// Loop through and filter transactions
		transactionDtoList.ForEach(transactionDto =>
        {
			bool descValid = false;
			bool endDateValid = false;
			bool endAmountValid = false;
			bool startDateValid = false;
			bool startAmountValid = false;

			// Test desc
			if (transactionDto.Description.ToLower().Contains(request.Description.ToLower()) || request.Description == "")
            {
				descValid = true;
            }
			
			// Test start amount (minimum)
			if(request.StartAmount == 0 || ((Double.Parse(transactionDto.Debit) - Double.Parse(transactionDto.Credit)) > request.StartAmount))
            {
				startAmountValid = true;
			}

			// Test end amount (maximum)
			if (request.EndAmount == 0 || ((Double.Parse(transactionDto.Debit) - Double.Parse(transactionDto.Credit)) < request.EndAmount))
			{
				endAmountValid = true;
			}

            // Start Date
            if (request.StartDate == "" || DateTime.Compare(DateTime.Parse(request.StartDate), DateTime.Parse(transactionDto.Date)) <= 0)
            {
				startDateValid = true;
			}

			// End Date
			if (request.EndDate == "" || DateTime.Compare(DateTime.Parse(request.EndDate), DateTime.Parse(transactionDto.Date)) >= 0)
			{
				endDateValid = true;
			}


			if (descValid && startAmountValid && endAmountValid && startDateValid && endDateValid)
            {
                filtered.Add(transactionDto);
            }
		});

		TransactionResponseDto response = new TransactionResponseDto(filtered);

		return response;

	}

		private TransactionResponseDto DoSampling(List<TransactionDto> transactionDtoList,
		SampleInputDto request)
	{

		// Check accounts have been selected
		// TODO change to throw BAD_REQUEST exception
		if(transactionDtoList.Count == 0) return null;

		// Create response object fields
		string? responseSeedCode = null;
		string? responseSampleInterval = null;
		string? responseSamplePercentage = null;
		List<TransactionDto> sampledTransactions = new List<TransactionDto>();

		// Get request data
		decimal requestSeedCode = decimal.Parse(request.SeedCode);
		decimal creditSample = (decimal) request.CreditIn;
		decimal debitSample = (decimal) request.DebitIn;
		decimal materialitySample = (decimal) request.MaterialityIn;
		List<BankAccountDto> bankAccountsDtoList = request.BankAccounts;

		// List to be returned to user
		List<TransactionDto> transactionsForAudit = new List<TransactionDto>();

		// Create a queue so looping is easier when getting a certain amount of transactions
		Queue<TransactionDto> queue = new Queue<TransactionDto>(transactionDtoList);

		// Sort transactions by account name
		transactionDtoList = transactionDtoList.OrderBy(x => x.AccountName).ToList();

		// If materiality sample is 0, return all that meet that criteria
		if(materialitySample > 0)
        {
			foreach(TransactionDto transaction in transactionDtoList)
            {
				if(decimal.Parse(transaction.Credit) > materialitySample || decimal.Parse(transaction.Debit) > materialitySample)
                {
					transactionsForAudit.Add(transaction);
                }
            }
			sampledTransactions.AddRange(transactionsForAudit);
			responseSampleInterval = "0";
			responseSamplePercentage = "0";
			responseSeedCode = "No seed code for materiality sampling.";
        }
        else
        {
			// Init sampling interval
			decimal sample = CalculateSamplingInterval(transactionDtoList, bankAccountsDtoList, (creditSample + debitSample));

			decimal currentSample;

			// Get sample interval

			// If seed code has been received, recreate sample
			if(Int32.Parse(request.SeedCode) != 0)
            {
				currentSample = Int32.Parse(request.SeedCode) / 156;
				responseSeedCode = request.SeedCode;
				responseSampleInterval = sample.ToString();

            } 
			// If no sampling numbers, return all samples
			else if(creditSample == 0 && debitSample == 0)
            {
				currentSample = GetStartingSample(sample);
				responseSeedCode = "No Transactions Sampled";
				responseSampleInterval = "0";
            }
            // Otherwise proceed with sampling, set seed code and sampling interval
            else
            {
				currentSample = GetStartingSample(sample);
				// Multiplied by 156 for aesthetic purposes
				responseSeedCode = (currentSample * 156).ToString();
				responseSampleInterval = sample.ToString();
            }

			// Init running total
			decimal runningTotal = 0;

			// Init counters for db and cr 
			int crActual = 0, dbActual = 0;

			// If no specs, return all transactions
			if(creditSample == 0 && debitSample == 0)
            {
				sampledTransactions.AddRange(DeleteDuplicates(transactionDtoList));
			}
            else
            {
				// Actual sampling based no inputs
				for(; ; )
                {
					TransactionDto current;
					if (queue.Count != 0)
                    {
						// Get first on 
						current = queue.Peek();

                    }
                    else
                    {
						break;
                    }



					// If transaction is 0 0, skip
					if((decimal.Parse(current.Debit) == 0) && (decimal.Parse(current.Credit) == 0))
                    {
						queue.Dequeue();
						continue;
                    }

					runningTotal += (decimal.Parse(current.Debit) + decimal.Parse(current.Credit));

					if(runningTotal > currentSample)
                    {
						// Check if request conditions have been satisfied then break
						if (((crActual == creditSample) || (crActual == getTransactionsByType(transactionDtoList, "credit").Count()))
							 && ((dbActual == debitSample) || (dbActual == getTransactionsByType(transactionDtoList, "debit").Count())))
							{
							Console.WriteLine("test");
							Console.WriteLine(getTransactionsByType(transactionDtoList, "debit").Count());
							Console.WriteLine(dbActual);
							Console.WriteLine(debitSample);
							break;
                        }

						// Check db
						if(current.GetTransactionType() == "debit" && dbActual != debitSample)
                        {

							transactionsForAudit.Add(current);
							dbActual++;
							currentSample += sample;
							queue.Dequeue();

                        }

						// Check cr
						if (current.GetTransactionType() == "credit" && crActual != creditSample)
						{

							transactionsForAudit.Add(current);
							crActual++;
							currentSample += sample;
							queue.Dequeue();

                        }
                        else
                        // Remov current from start of queue and re-add to the end
						{
                            try
                            {
								queue.Dequeue();
								queue.Enqueue(current);
								continue;
							}
							catch(Exception ex)
                            {
								Console.WriteLine(ex.ToString());
                            }

                        }
					}
                    try
                    {
						queue.Dequeue();
						queue.Enqueue(current);
					}
					catch(Exception ex)
                    {
						Console.WriteLine(ex.ToString());

					}

				}

				sampledTransactions.AddRange(transactionsForAudit);

            }


        }

		responseSamplePercentage = (transactionsTotal(transactionsForAudit) / (transactionsTotal(transactionDtoList)) * 100).ToString();

		TransactionResponseDto response = new TransactionResponseDto(sampledTransactions,
															responseSeedCode,
															responseSamplePercentage,
															responseSampleInterval);

        return response;
	}

	private decimal CalculateSamplingInterval(List<TransactionDto> transactionDtoList, // TODO: BigDecimal
		IEnumerable<BankAccountDto> bankAccountDtoList, decimal numberOfTransactions)
	{
		decimal sampleInterval;

		if(numberOfTransactions == 0)
        {
			numberOfTransactions = 1;
        }

		//var numberOfTransactions = transactionDtoList.Count;

        try
        {
			sampleInterval = CalculateSumOfTransactions(bankAccountDtoList) / numberOfTransactions;

			//var sampleInterval = CalculateSumOfTransactions(bankAccountDtoList) / numberOfTransactions;
			// .divide(numberOfTransactions, 2, RoundingMode.HALF_UP); // TODO:s

		} catch(DivideByZeroException exception)
        {
			Console.WriteLine(exception.Message);
			Console.WriteLine(exception.StackTrace);
			sampleInterval = 0; // TODO : handle better
        }

		return sampleInterval;
	}

	private decimal CalculateSumOfTransactions(IEnumerable<BankAccountDto> bankAccountDtoList) // TODO: BigDecimal
	{
		return bankAccountDtoList.Sum(
			bankAccountDto =>
				decimal.TryParse(bankAccountDto.TotalNetActivity, out var result) ? result : 0m
		); // TODO: BigDecimal
	}

	private static List<TransactionDto> DeleteDuplicates(List<TransactionDto> transactionDtoList) =>
		transactionDtoList.Distinct().OrderBy(x => x.AccountName).ToList();

	private decimal GetStartingSample(decimal sample) // TODO: BigDecimal,  BigDecimal
	{
	 Random Random = new();

	// var variable       = sample.setScale(0, RoundingMode.HALF_UP).intValue(); // TODO:
	var variable          = Convert.ToInt32(sample);
		var randomFirstSample = Random.Next(variable) + 1;
		return Convert.ToDecimal(randomFirstSample); // TODO: BigDecimal
	}
	private List<TransactionDto> getTransactionsByType(List<TransactionDto> wholeTransactionsList, string type)
	{

		List<TransactionDto> transactionsList = new List<TransactionDto>();

		// Loop through and pull transactions that meet reqs
		foreach (TransactionDto transaction in wholeTransactionsList)
		{

			if (decimal.Parse(transaction.Debit) > 0 && type.Equals("debit"))
			{

				transactionsList.Add(transaction);

			}
			else if (decimal.Parse(transaction.Credit) > 0 && type.Equals("credit"))
			{

				transactionsList.Add(transaction);

			}
		}
		return transactionsList;

	}
	private decimal transactionsTotal(List<TransactionDto> transactions) // TODO: bigDecimal
	{

		decimal total = 0;


		foreach (TransactionDto transaction in transactions)
		{

			total += decimal.Parse(transaction.Debit);
			total += decimal.Parse(transaction.Credit);
		}


		return total;


	}

}