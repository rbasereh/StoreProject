using Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TP.Application.BaseInfo;
using TP.Application.Common.Pagination;
using TP.Domain.UserSettings;
namespace TP.Api.Controllers.Users;

[ApiController]
[Authorize]
[Route("api/user/province")]
[ApiExplorerSettings(GroupName = ApiGroup.User)]
public class ProvinceController(IMediator mediator, ILogger<ProvinceController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ApiResponse<PaginationResponse<Province>>> Get([FromQuery] GetAllProvinceQuery query)
    {
        var data = await mediator.Send(query);
        return new() { Data = data };
    }
}


