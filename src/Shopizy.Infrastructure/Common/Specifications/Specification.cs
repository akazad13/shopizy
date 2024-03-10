using System.Linq.Expressions;

namespace shopizy.Infrastructure.Common.Specifications;

public abstract class Specification<TEntity>(Expression<Func<TEntity, bool>>? criteria) where TEntity : class
 {
    public Expression<Func<TEntity, bool>>? Criteria { get; } = criteria;
    public List<Expression<Func<TEntity, object>>> IncludeExpressions { get; } = [];
    public Expression<Func<TEntity, object>>? OrderByExpression { get; private set;}
    public Expression<Func<TEntity, object>>? OrderByDecendingExpression { get; private set;}

    protected void AddInclude(Expression<Func<TEntity, object>> includeExpression) =>
        IncludeExpressions.Add(includeExpression);

    protected void AddOrderBy(Expression<Func<TEntity, object>> orderByExpression) =>
        OrderByExpression = orderByExpression;
    
    protected void AddOrderByDecending(Expression<Func<TEntity, object>> orderByDecendingExpression) =>
        OrderByDecendingExpression = orderByDecendingExpression;
}
