using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ErpSupportDesk.Data;
using ErpSupportDesk.Models;
using ErpSupportDesk.Services;

namespace ErpSupportDesk.Controllers.Api;

[ApiController]
[Route("api/tickets")]
public sealed class TicketsApiController(AppDbContext db, TicketNumberGenerator ticketNumberGenerator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TicketListItemResponse>>> GetTickets(
        [FromQuery] string search = "",
        [FromQuery] string status = "",
        [FromQuery] string priority = "",
        [FromQuery] string module = "")
    {
        var query = db.ServiceTickets
            .AsNoTracking()
            .Include(ticket => ticket.Client)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(ticket =>
                ticket.Number.Contains(term) ||
                ticket.Title.Contains(term) ||
                ticket.Client!.Name.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(ticket => ticket.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(priority))
        {
            query = query.Where(ticket => ticket.Priority == priority);
        }

        if (!string.IsNullOrWhiteSpace(module))
        {
            query = query.Where(ticket => ticket.Module == module);
        }

        var tickets = await query
            .OrderBy(ticket => ticket.Status == "Zamkniete")
            .ThenBy(ticket => ticket.DueAt)
            .ThenBy(ticket => ticket.Priority)
            .Select(ticket => new TicketListItemResponse(
                ticket.Id,
                ticket.Number,
                ticket.Title,
                ticket.Client!.Name,
                ticket.Module,
                ticket.Priority,
                ticket.Status,
                ticket.AssignedEngineer,
                ticket.PlannedHours,
                ticket.SpentHours,
                ticket.DueAt))
            .ToListAsync();

        return Ok(tickets);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TicketDetailsResponse>> GetTicket(int id)
    {
        var ticket = await db.ServiceTickets
            .AsNoTracking()
            .Include(item => item.Client)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (ticket is null)
        {
            return NotFound();
        }

        return Ok(new TicketDetailsResponse(
            ticket.Id,
            ticket.Number,
            ticket.Title,
            ticket.Description,
            ticket.Client?.Name ?? string.Empty,
            ticket.Module,
            ticket.Priority,
            ticket.Status,
            ticket.AssignedEngineer,
            ticket.SourceChannel,
            ticket.AffectedVersion,
            ticket.PlannedHours,
            ticket.SpentHours,
            ticket.DueAt,
            ticket.RequiresDeployment,
            ticket.IsBillable));
    }

    [HttpPost]
    public async Task<ActionResult<TicketDetailsResponse>> CreateTicket([FromBody] CreateTicketRequest request)
    {
        var clientExists = await db.Clients.AnyAsync(client => client.Id == request.ClientId);

        if (!clientExists)
        {
            ModelState.AddModelError(nameof(request.ClientId), "Wybrany klient nie istnieje.");
            return ValidationProblem(ModelState);
        }

        var now = DateTime.Now;
        var ticket = new ServiceTicket
        {
            ClientId = request.ClientId,
            Number = await ticketNumberGenerator.GenerateAsync(),
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Module = request.Module,
            Status = request.Status,
            Priority = request.Priority,
            AssignedEngineer = request.AssignedEngineer,
            PlannedHours = request.PlannedHours,
            SpentHours = request.SpentHours,
            SourceChannel = request.SourceChannel,
            AffectedVersion = request.AffectedVersion.Trim(),
            CreatedAt = now,
            UpdatedAt = now,
            DueAt = request.DueAt,
            RequiresDeployment = request.RequiresDeployment,
            IsBillable = request.IsBillable
        };

        db.ServiceTickets.Add(ticket);
        await db.SaveChangesAsync();

        var clientName = await db.Clients
            .AsNoTracking()
            .Where(client => client.Id == ticket.ClientId)
            .Select(client => client.Name)
            .FirstAsync();

        var response = new TicketDetailsResponse(
            ticket.Id,
            ticket.Number,
            ticket.Title,
            ticket.Description,
            clientName,
            ticket.Module,
            ticket.Priority,
            ticket.Status,
            ticket.AssignedEngineer,
            ticket.SourceChannel,
            ticket.AffectedVersion,
            ticket.PlannedHours,
            ticket.SpentHours,
            ticket.DueAt,
            ticket.RequiresDeployment,
            ticket.IsBillable);

        return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, response);
    }
}

public sealed record TicketListItemResponse(
    int Id,
    string Number,
    string Title,
    string Client,
    string Module,
    string Priority,
    string Status,
    string AssignedEngineer,
    decimal PlannedHours,
    decimal SpentHours,
    DateTime DueAt);

public sealed record TicketDetailsResponse(
    int Id,
    string Number,
    string Title,
    string Description,
    string Client,
    string Module,
    string Priority,
    string Status,
    string AssignedEngineer,
    string SourceChannel,
    string AffectedVersion,
    decimal PlannedHours,
    decimal SpentHours,
    DateTime DueAt,
    bool RequiresDeployment,
    bool IsBillable);

public sealed class CreateTicketRequest
{
    [Required]
    public int ClientId { get; init; }

    [Required]
    [StringLength(160)]
    public string Title { get; init; } = string.Empty;

    [Required]
    [StringLength(2500)]
    public string Description { get; init; } = string.Empty;

    [Required]
    public string Module { get; init; } = string.Empty;

    [Required]
    public string Status { get; init; } = "Nowe";

    [Required]
    public string Priority { get; init; } = "Sredni";

    [Required]
    public string AssignedEngineer { get; init; } = string.Empty;

    [Range(0, 500)]
    public decimal PlannedHours { get; init; }

    [Range(0, 500)]
    public decimal SpentHours { get; init; }

    [Required]
    public string SourceChannel { get; init; } = string.Empty;

    [Required]
    public string AffectedVersion { get; init; } = string.Empty;

    public DateTime DueAt { get; init; }

    public bool RequiresDeployment { get; init; }

    public bool IsBillable { get; init; }
}
