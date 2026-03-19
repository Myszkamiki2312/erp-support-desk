using Microsoft.AspNetCore.Mvc;
using ErpSupportDesk.Services;
using ErpSupportDesk.ViewModels;

namespace ErpSupportDesk.Controllers.Api;

[ApiController]
[Route("api/dashboard")]
public sealed class DashboardApiController(DashboardService dashboardService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<DashboardViewModel>> GetDashboard()
    {
        var dashboard = await dashboardService.BuildDashboardAsync();
        return Ok(dashboard);
    }
}
