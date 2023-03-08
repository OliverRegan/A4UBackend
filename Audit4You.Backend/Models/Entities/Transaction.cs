namespace Audit4You.Backend.Models.Entities;

public record Transaction
(
	string ExternalId,
	string Source,
	string Date,
	string Description,
	string Debit,
	string Credit
)
{
	// @Id
	// @GeneratedValue(strategy = GenerationType.AUTO)
	public long Id { get; } = default;
}