namespace Shopizy.Domain.Common.Models;

/// <summary>
/// Base class for aggregate roots in the domain model.
/// </summary>
/// <typeparam name="TId">The type of the aggregate root's identifier.</typeparam>
/// <typeparam name="TIdType">The underlying type of the identifier.</typeparam>
public abstract class AggregateRoot<TId, TIdType> : Entity<TId> where TId : AggregateRootId<TIdType>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRoot{TId, TIdType}"/> class with the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    protected AggregateRoot(TId id)
    {
        Id = id;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRoot{TId, TIdType}"/> class.
    /// </summary>
    protected AggregateRoot() { }
}
