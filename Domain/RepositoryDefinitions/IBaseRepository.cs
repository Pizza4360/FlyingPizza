﻿using System.Threading.Tasks;
using MongoDB.Driver;

namespace Domain.RepositoryDefinitions;

public interface IBaseRepository<DomainEntity>
{
    /// <summary>
    /// Creates a new domain entity in the repository
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>The domain entity, as it was created</returns>
    public Task CreateAsync(DomainEntity entity);

    /// <summary>
    /// Retrieves a domain entity based on id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The matching entity, or null if it does not exist</returns>
    public Task<DomainEntity> GetByIdAsync(string id);

    /// <summary>
    /// Deletes the domain entity with the provided id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>true if the operation succeeded, or false if it failed (including if the entity doesn't exist)</returns>
    public Task<bool> RemoveAsync(string id);

    /// <summary>
    /// Updates an existing entity in the repository
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>The entity, as it was updated</returns>
    public Task<UpdateResult> UpdateAsync(DomainEntity entity);
}