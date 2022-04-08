using System.Linq.Expressions;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using MongoDB.Driver;

namespace Dispatch.Repositories
{
    public abstract class MongoRepository<DomainEntity> : IBaseRepository<DomainEntity>
        where DomainEntity : IBaseEntity
    {
        private readonly IMongoCollection<DomainEntity> _collection;

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            _collection = database.GetCollection<DomainEntity>(collectionName);
        }

        public async Task<DomainEntity> CreateAsync(DomainEntity entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task<bool> Delete(string id)
        {
            return (await _collection.DeleteOneAsync(id)).IsAcknowledged;
        }

        public async Task<DomainEntity> GetByIdAsync(string id)
        {
            return (await _collection.FindAsync(entity => entity.Id == id)).FirstOrDefault();
        }

        public async Task<IEnumerable<DomainEntity>> GetByIdsAsync(IEnumerable<string> ids)
        {
            return (await _collection.FindAsync(entity => ids.Contains(entity.Id))).ToEnumerable();
        }

        public async Task<DomainEntity> Update(DomainEntity entity)
        {
            await _collection.ReplaceOneAsync(e => e.Id == entity.Id, entity);
            return entity;
        }

        private protected async Task<IEnumerable<DomainEntity>> GetAllWhereAsync(Expression<Func<DomainEntity, bool>> filter)
        {
            return (await _collection.FindAsync(filter)).ToEnumerable();
        }
    }
}
