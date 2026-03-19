using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Palickaj.Models;

namespace Palickaj.ViewModels;

public sealed class TicketFormViewModel
{
    public int? Id { get; set; }

    [Required]
    [Display(Name = "Klient")]
    public int ClientId { get; set; }

    [Required]
    [StringLength(160)]
    [Display(Name = "Temat")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2500)]
    [Display(Name = "Opis techniczny")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Modul")]
    public string Module { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Status")]
    public string Status { get; set; } = "Nowe";

    [Required]
    [Display(Name = "Priorytet")]
    public string Priority { get; set; } = "Sredni";

    [Required]
    [Display(Name = "Osoba odpowiedzialna")]
    public string AssignedEngineer { get; set; } = string.Empty;

    [Range(0, 500)]
    [Display(Name = "Planowane godziny")]
    public decimal PlannedHours { get; set; }

    [Range(0, 500)]
    [Display(Name = "Przepracowane godziny")]
    public decimal SpentHours { get; set; }

    [Required]
    [Display(Name = "Zrodlo zgloszenia")]
    public string SourceChannel { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Wersja systemu")]
    public string AffectedVersion { get; set; } = string.Empty;

    [Display(Name = "Termin")]
    [DataType(DataType.DateTime)]
    public DateTime DueAt { get; set; } = DateTime.Today.AddDays(1).AddHours(12);

    [Display(Name = "Wymaga wdrozenia")]
    public bool RequiresDeployment { get; set; }

    [Display(Name = "Praca platna")]
    public bool IsBillable { get; set; }

    public IEnumerable<SelectListItem> ClientOptions { get; set; } = [];

    public IEnumerable<SelectListItem> StatusOptions { get; set; } = [];

    public IEnumerable<SelectListItem> PriorityOptions { get; set; } = [];

    public IEnumerable<SelectListItem> ModuleOptions { get; set; } = [];

    public IEnumerable<SelectListItem> EngineerOptions { get; set; } = [];

    public IEnumerable<SelectListItem> SourceOptions { get; set; } = [];

    public static TicketFormViewModel FromEntity(ServiceTicket ticket)
    {
        return new TicketFormViewModel
        {
            Id = ticket.Id,
            ClientId = ticket.ClientId,
            Title = ticket.Title,
            Description = ticket.Description,
            Module = ticket.Module,
            Status = ticket.Status,
            Priority = ticket.Priority,
            AssignedEngineer = ticket.AssignedEngineer,
            PlannedHours = ticket.PlannedHours,
            SpentHours = ticket.SpentHours,
            SourceChannel = ticket.SourceChannel,
            AffectedVersion = ticket.AffectedVersion,
            DueAt = ticket.DueAt,
            RequiresDeployment = ticket.RequiresDeployment,
            IsBillable = ticket.IsBillable
        };
    }
}
