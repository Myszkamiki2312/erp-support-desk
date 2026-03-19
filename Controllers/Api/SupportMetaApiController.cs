using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ErpSupportDesk.Data;
using ErpSupportDesk.Models;

namespace ErpSupportDesk.Controllers.Api;

[ApiController]
[Route("api/support/meta")]
public sealed class SupportMetaApiController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<SupportMetaResponse>> GetMeta()
    {
        var clients = await db.Clients
            .AsNoTracking()
            .OrderBy(client => client.Name)
            .Select(client => new SupportClientResponse(
                client.Id,
                client.Name,
                client.ErpEnvironment,
                client.SupportPlan))
            .ToListAsync();

        return Ok(new SupportMetaResponse(
            clients,
            SupportCatalog.TicketStatuses,
            SupportCatalog.TicketPriorities,
            SupportCatalog.Modules,
            SupportCatalog.SourceChannels,
            SupportCatalog.Engineers));
    }
}

public sealed record SupportMetaResponse(
    IReadOnlyList<SupportClientResponse> Clients,
    IReadOnlyList<string> Statuses,
    IReadOnlyList<string> Priorities,
    IReadOnlyList<string> Modules,
    IReadOnlyList<string> SourceChannels,
    IReadOnlyList<string> Engineers);

public sealed record SupportClientResponse(
    int Id,
    string Name,
    string ErpEnvironment,
    string SupportPlan);
