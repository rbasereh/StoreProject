using System.ComponentModel.DataAnnotations;

namespace TP.Application.Common.Pagination;

public interface IPaginationResponse;
public record PaginationResponse<TResult>(List<TResult> Items, long? Total, int? Index, int? PageSize) : IPaginationResponse;
public record PaginationRequest<TFilter> : PaginationRequest
{
    public TFilter Filter { get; set; }
}

public record OptionalPaginationRequest
{
    public int Index { get; set; } = 0;

    [Range(0, 50)]
    public int? PageSize { get; set; }

}
public record PaginationRequest
{
    public int Index { get; set; } = 0;

    [Range(0, 50)]
    public int PageSize { get; set; } = 10;
}

public static class PaginationFilter
{
    public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> queryable, PaginationRequest request)
    {
        return queryable.Skip((request.Index) * request.PageSize)
                   .Take(request.PageSize);
    }
    
    public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> queryable, OptionalPaginationRequest request)
    {
        if (!request.PageSize.HasValue)
            return queryable;
        return queryable.Skip((request.Index) * request.PageSize.Value)
                   .Take(request.PageSize.Value);
    }
}
