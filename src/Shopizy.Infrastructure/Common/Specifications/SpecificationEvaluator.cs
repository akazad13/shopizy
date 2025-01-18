using Microsoft.EntityFrameworkCore;

namespace Shopizy.Infrastructure.Common.Specifications;

public static class SpecificationEvaluator
{
    public static IQueryable<TEntity> GetQuery<TEntity>(
        IQueryable<TEntity> query,
        Specification<TEntity> specification
    )
        where TEntity : class
    {
        var queryable = query;

        if (specification.Criteria is not null)
        {
            queryable = queryable.Where(specification.Criteria);
        }

        if (specification.OrderByExpression is not null)
        {
            var orderedQuery = queryable.OrderBy(specification.OrderByExpression);
            if (specification.ThenOrderByExpression is not null)
            {
                queryable = orderedQuery.ThenBy(specification.ThenOrderByExpression);
            }
            else if (specification.ThenOrderByDescendingExpression is not null)
            {
                queryable = orderedQuery.ThenByDescending(
                    specification.ThenOrderByDescendingExpression
                );
            }
            else
            {
                queryable = orderedQuery;
            }
        }

        if (specification.OrderByDescendingExpression is not null)
        {
            var orderedQuery = queryable.OrderBy(specification.OrderByDescendingExpression);
            if (specification.ThenOrderByExpression is not null)
            {
                queryable = orderedQuery.ThenBy(specification.ThenOrderByExpression);
            }
            else if (specification.ThenOrderByDescendingExpression is not null)
            {
                queryable = orderedQuery.ThenByDescending(
                    specification.ThenOrderByDescendingExpression
                );
            }
            else
            {
                queryable = orderedQuery;
            }
        }

        if (specification.OrderByDescendingExpression is not null)
        {
            queryable = queryable.OrderByDescending(specification.OrderByDescendingExpression);
        }

        if (specification.IncludeExpressions.Count != 0)
        {
            queryable = specification.IncludeExpressions.Aggregate(
                queryable,
                (current, includeExpression) => current.Include(includeExpression)
            );
        }

        if (specification.IncludeStrings.Count != 0)
        {
            queryable = specification.IncludeStrings.Aggregate(
                queryable,
                (current, IncludeStrings) => current.Include(IncludeStrings)
            );
        }

        if (specification.IsPagingEnabled)
        {
            queryable = queryable.Skip(specification.Skip).Take(specification.Take);
        }

        return queryable;
    }
}
