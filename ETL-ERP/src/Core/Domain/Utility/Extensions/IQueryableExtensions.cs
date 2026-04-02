using Domain.ModelInterface;
using System.Linq.Expressions;

namespace Domain.Utility.Extensions;

public static class IQueryableExtensions
{
    public static IQueryable<TEntity> ApplyOrdering<TEntity>(this IQueryable<TEntity> query, IQueryObject queryObj, Dictionary<string, Expression<Func<TEntity, object>>> columnsMap)
    {
        if (queryObj == null || columnsMap == null)
            return query;

        if (String.IsNullOrWhiteSpace(queryObj.SortBy) || !columnsMap.ContainsKey(queryObj.SortBy))
            return query;

        if (queryObj.IsSortAscending)
            return query.OrderBy(columnsMap[queryObj.SortBy]);
        else
            return query.OrderByDescending(columnsMap[queryObj.SortBy]);
    }

    public static IQueryable<TEntity> ApplyPaging<TEntity>(this IQueryable<TEntity> query, IQueryObject queryObj)
    {
        if (queryObj == null)
            return query;

        if (queryObj.Page <= 0)
            queryObj.Page = 1;

        if (queryObj.PageSize <= 0)
            queryObj.PageSize = 10;

        return query.Skip((queryObj.Page - 1) * queryObj.PageSize).Take(queryObj.PageSize);
    }
}