using Audit4You.Backend.Models.Entities;

namespace Audit4You.Backend.Models.Dto;

public record ClientDto(
	long Id,
	string Name,
	string Abn,
	string Acn,
	string Email,
	string Address,
	string Phone)
{
	public ClientDto(Client entity) : this(
		entity.Id,
		entity.Name,
		entity.Abn,
		entity.Acn,
		entity.Email,
		entity.Address,
		entity.Phone
	)
	{
	}
}