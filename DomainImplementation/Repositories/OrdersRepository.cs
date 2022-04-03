using Domain.Entities;
using Domain.Interfaces.Repositories;
using MongoDB.Driver;

namespace DomainImplementation.Repositories
{
    public class OrdersRepository : MongoRepository<Order>, IOrdersRepository
    {
        public OrdersRepository(IMongoDatabase database, string collectionName)
            : base(database, collectionName)
        {
        }
    }
}