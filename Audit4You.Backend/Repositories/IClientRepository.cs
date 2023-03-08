using Audit4You.Backend.Models.Entities;

namespace Audit4You.Backend.Repositories;

public interface IClientRepository
{
	Client? GetByName(string name);
}