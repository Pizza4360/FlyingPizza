using Domain.DTO;
using Domain.Entities;

namespace Domain.RepositoryDefinitions;

public interface IOrdersRepository : IBaseRepository<Order, OrderUpdate>
{
}