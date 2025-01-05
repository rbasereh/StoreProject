using TP.Domain.UserSettings;

namespace TP.Application.BaseInfo;

public record ProvinceDto(int Id, string Name);
public record GetAllProvinceQuery : OptionalPaginationRequest, IRequest<PaginationResponse<Province>>;

public class GetAllProvinceQueryHandler(IApplicationDbContext dbContext, IUser user) : IRequestHandler<GetAllProvinceQuery, PaginationResponse<Province>>
{
    public async Task<PaginationResponse<Province>> Handle(GetAllProvinceQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Set<Province>().AsNoTracking();
        var total = await query.CountAsync();

        List<Province> items = await query.ApplyPaging(request).ToListAsync(cancellationToken: cancellationToken);

        return new(items, total, request.Index, request.PageSize);
    }
}
