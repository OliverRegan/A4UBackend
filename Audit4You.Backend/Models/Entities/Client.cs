namespace Audit4You.Backend.Models.Entities;

public class Client
{
	// @Id
	// @GeneratedValue(strategy = GenerationType.AUTO)
	// @Getter
	public long Id { get; }

	public string Name { get; }

	public string Abn { get; }

	public string Acn { get; }

	public string Email { get; }

	public string Address { get; }

	public string Phone { get; }

	public Client(string name, string abn, string acn, string email, string address, string phone)
	{
		Name    = name;
		Abn     = abn;
		Acn     = acn;
		Email   = email;
		Address = address;
		Phone   = phone;
	}

	public Client() { }
}