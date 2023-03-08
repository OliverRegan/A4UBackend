using Microsoft.AspNetCore.Mvc;

namespace Audit4You.Backend.Controllers;

[ApiController]
[Route("secured")]
public class SecurityController : ControllerBase
{

	// private UserRepository userRepository;
//Testing
	// private ClientRepository clientRepository;

//End Testing
	// private RoleRepository roleRepository;

	// [HttpGet("users")]
	// public List<Users> GetUsers() {
		// return userRepository.findAll();
	// }

	[HttpDelete("delete")]
	public string Delete() {
		return "Delete request";
	}

	[HttpGet]
	public string Secured() {
		return "secured";
	}

	/* Testing */

// 	[HttpGet("clients")]
// 	public List<Client> GetClient() {
// //        Create dummy client
// 		List<Transaction> transactions = new ArrayList<Transaction>();
// 		Transaction test1 = new Transaction("1", "testSource", "testDate", "goodDescription", "test debit", "test credit");
// 		Transaction test3 = new Transaction("3", "testSource3", "testDate3", "goodDescription3", "test debit3", "test credit3");
// 		transactions.add(test1);
// 		transactions.add(test3);
// 		BankAccount bcn = new BankAccount("test acn", "test acn num", BigDecimal.valueOf(3002), BigDecimal.valueOf(6298), BigDecimal.valueOf(30002), transactions);
// 		Client test = new Client("testData", "test data", "test", "test data", "test data", "test data");
// 		clientRepository.save(test);
// 		return clientRepository.findAll();
// 	}

	// @PostMapping("/adduser")
	// public Collection<Users> postUsers(Users user) {
	// 	userRepository.save(user);
	// 	return userRepository.findAll();
	// }

	/* End testing */
}