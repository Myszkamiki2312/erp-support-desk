using Microsoft.EntityFrameworkCore;
using Palickaj.Models;

namespace Palickaj.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await db.Database.EnsureCreatedAsync();

        if (await db.Clients.AnyAsync())
        {
            await RenameAuthorAsync(db);
            return;
        }

        var today = DateTime.Today;

        var clients = new List<Client>
        {
            new()
            {
                Name = "Northwind Manufacturing",
                Industry = "Produkcja",
                ErpEnvironment = "Comarch ERP XL 2024.1",
                ContactPerson = "Anna Krol",
                ContactEmail = "anna.krol@northwind.pl",
                SupportPlan = "Enterprise",
                ActiveUsers = 118,
                DatabaseName = "XL_NORTHWIND_PRD",
                LastReviewAt = today.AddDays(-6)
            },
            new()
            {
                Name = "Vista Retail Group",
                Industry = "Handel detaliczny",
                ErpEnvironment = "Comarch ERP XL 2023.4",
                ContactPerson = "Marek Banas",
                ContactEmail = "marek.banas@vistaretail.pl",
                SupportPlan = "Business",
                ActiveUsers = 74,
                DatabaseName = "XL_VISTA_CORE",
                LastReviewAt = today.AddDays(-13)
            },
            new()
            {
                Name = "Helios Logistics",
                Industry = "Logistyka",
                ErpEnvironment = "Comarch ERP XL 2024.0",
                ContactPerson = "Kinga Stelmach",
                ContactEmail = "kinga.stelmach@helioslog.pl",
                SupportPlan = "Enterprise",
                ActiveUsers = 93,
                DatabaseName = "XL_HELIOS_WMS",
                LastReviewAt = today.AddDays(-4)
            },
            new()
            {
                Name = "MediCore Pharma",
                Industry = "Farmacja",
                ErpEnvironment = "Comarch ERP Optima + XL bridge",
                ContactPerson = "Pawel Ostrowski",
                ContactEmail = "pawel.ostrowski@medicore.pl",
                SupportPlan = "Business",
                ActiveUsers = 51,
                DatabaseName = "OPTIMA_MEDICORE",
                LastReviewAt = today.AddDays(-10)
            },
            new()
            {
                Name = "Baltic Components",
                Industry = "Automotive",
                ErpEnvironment = "Comarch ERP XL 2024.1",
                ContactPerson = "Dorota Zych",
                ContactEmail = "dorota.zych@balticcomponents.pl",
                SupportPlan = "Enterprise",
                ActiveUsers = 128,
                DatabaseName = "XL_BALTIC_PROD",
                LastReviewAt = today.AddDays(-3)
            },
            new()
            {
                Name = "Urban Foods",
                Industry = "FMCG",
                ErpEnvironment = "Comarch ERP XL 2023.5",
                ContactPerson = "Jakub Kania",
                ContactEmail = "jakub.kania@urbanfoods.pl",
                SupportPlan = "Start",
                ActiveUsers = 29,
                DatabaseName = "XL_URBAN_FOODS",
                LastReviewAt = today.AddDays(-17)
            }
        };

        await db.Clients.AddRangeAsync(clients);
        await db.SaveChangesAsync();

        var tickets = new List<ServiceTicket>
        {
            new()
            {
                Number = "SRV-2603-001",
                ClientId = clients[0].Id,
                Title = "Blad walidacji dokumentu WZ po aktualizacji",
                Description = "Operatorzy magazynu otrzymuja komunikat przy zatwierdzaniu WZ dla zlecen mieszanych.",
                Module = "Logistyka",
                Status = "W realizacji",
                Priority = "Krytyczny",
                AssignedEngineer = "Bartłomiej Przybycień",
                PlannedHours = 8,
                SpentHours = 5.5m,
                SourceChannel = "Telefon",
                AffectedVersion = "ERP XL 2024.1",
                CreatedAt = today.AddDays(-4).AddHours(8),
                UpdatedAt = today.AddDays(-1).AddHours(15),
                DueAt = today.AddHours(16),
                RequiresDeployment = true,
                IsBillable = false
            },
            new()
            {
                Number = "SRV-2603-002",
                ClientId = clients[1].Id,
                Title = "Rozszerzenie eksportu zamowien do sklepu B2B",
                Description = "Klient potrzebuje dodatkowego pola z limitem kredytowym w integracji web service.",
                Module = "Integracje",
                Status = "Zaplanowane",
                Priority = "Wysoki",
                AssignedEngineer = "Bartłomiej Przybycień",
                PlannedHours = 12,
                SpentHours = 2,
                SourceChannel = "Email",
                AffectedVersion = "ERP XL 2023.4",
                CreatedAt = today.AddDays(-7).AddHours(11),
                UpdatedAt = today.AddDays(-2).AddHours(10),
                DueAt = today.AddDays(3).AddHours(12),
                RequiresDeployment = true,
                IsBillable = true
            },
            new()
            {
                Number = "SRV-2603-003",
                ClientId = clients[2].Id,
                Title = "Niezgodnosc stanow magazynowych po imporcie skanera",
                Description = "Po nocnym imporcie dane skanera terminalowego nie bilansuja sie z ruchem magazynowym.",
                Module = "Magazyn",
                Status = "Nowe",
                Priority = "Krytyczny",
                AssignedEngineer = "Marta Kozak",
                PlannedHours = 6,
                SpentHours = 1,
                SourceChannel = "Portal",
                AffectedVersion = "ERP XL 2024.0",
                CreatedAt = today.AddDays(-1).AddHours(7),
                UpdatedAt = today.AddDays(-1).AddHours(8),
                DueAt = today.AddHours(11),
                RequiresDeployment = false,
                IsBillable = false
            },
            new()
            {
                Number = "SRV-2603-004",
                ClientId = clients[3].Id,
                Title = "Nowy raport marzy dla kontrolingu",
                Description = "Dzial finansowy potrzebuje raportu z marza per producent i grupe towarowa.",
                Module = "Finanse",
                Status = "W realizacji",
                Priority = "Sredni",
                AssignedEngineer = "Katarzyna Wrobel",
                PlannedHours = 18,
                SpentHours = 9,
                SourceChannel = "Spotkanie",
                AffectedVersion = "ERP Optima bridge",
                CreatedAt = today.AddDays(-10).AddHours(9),
                UpdatedAt = today.AddDays(-2).AddHours(14),
                DueAt = today.AddDays(5).AddHours(15),
                RequiresDeployment = true,
                IsBillable = true
            },
            new()
            {
                Number = "SRV-2603-005",
                ClientId = clients[4].Id,
                Title = "Poprawa wydajnosci generatora zlecen produkcyjnych",
                Description = "Proces planowania trwa powyzej 7 minut dla wsadow dziennych i wymaga optymalizacji.",
                Module = "Produkcja",
                Status = "Czeka na klienta",
                Priority = "Wysoki",
                AssignedEngineer = "Kamil Pluta",
                PlannedHours = 16,
                SpentHours = 11.5m,
                SourceChannel = "Email",
                AffectedVersion = "ERP XL 2024.1",
                CreatedAt = today.AddDays(-12).AddHours(10),
                UpdatedAt = today.AddDays(-1).AddHours(16),
                DueAt = today.AddDays(-1).AddHours(12),
                RequiresDeployment = false,
                IsBillable = true
            },
            new()
            {
                Number = "SRV-2603-006",
                ClientId = clients[5].Id,
                Title = "Konfiguracja nowego operatora i uprawnien",
                Description = "Trzeba przygotowac role dla brygadzisty oraz szablon uprawnien do dokumentow handlowych.",
                Module = "Administracja",
                Status = "Zamkniete",
                Priority = "Niski",
                AssignedEngineer = "Marta Kozak",
                PlannedHours = 2,
                SpentHours = 1.5m,
                SourceChannel = "Telefon",
                AffectedVersion = "ERP XL 2023.5",
                CreatedAt = today.AddDays(-8).AddHours(13),
                UpdatedAt = today.AddDays(-6).AddHours(9),
                DueAt = today.AddDays(-6).AddHours(12),
                RequiresDeployment = false,
                IsBillable = true
            },
            new()
            {
                Number = "SRV-2603-007",
                ClientId = clients[0].Id,
                Title = "Integracja faktur z systemem obiegu dokumentow",
                Description = "Do wdrozenia pozostaje mapowanie nowych statusow akceptacji i logowanie bledow transportu.",
                Module = "Integracje",
                Status = "W realizacji",
                Priority = "Wysoki",
                AssignedEngineer = "Katarzyna Wrobel",
                PlannedHours = 20,
                SpentHours = 13,
                SourceChannel = "Spotkanie",
                AffectedVersion = "ERP XL 2024.1",
                CreatedAt = today.AddDays(-14).AddHours(9),
                UpdatedAt = today.AddDays(-1).AddHours(11),
                DueAt = today.AddDays(2).AddHours(15),
                RequiresDeployment = true,
                IsBillable = true
            },
            new()
            {
                Number = "SRV-2603-008",
                ClientId = clients[1].Id,
                Title = "Korekta numeracji dokumentow zwrotu",
                Description = "Po zamknieciu miesiaca klient wymaga odseparowania schematu numeracji dla kanalow outlet.",
                Module = "Handel",
                Status = "Nowe",
                Priority = "Sredni",
                AssignedEngineer = "Bartłomiej Przybycień",
                PlannedHours = 5,
                SpentHours = 0,
                SourceChannel = "Portal",
                AffectedVersion = "ERP XL 2023.4",
                CreatedAt = today.AddDays(-2).AddHours(15),
                UpdatedAt = today.AddDays(-2).AddHours(15),
                DueAt = today.AddDays(4).AddHours(10),
                RequiresDeployment = false,
                IsBillable = true
            },
            new()
            {
                Number = "SRV-2603-009",
                ClientId = clients[2].Id,
                Title = "Monitoring bledow synchronizacji kurierow",
                Description = "Potrzebny jest raport dzienny z nieudanych przesylek oraz liczba ponowien.",
                Module = "Integracje",
                Status = "Zaplanowane",
                Priority = "Sredni",
                AssignedEngineer = "Kamil Pluta",
                PlannedHours = 9,
                SpentHours = 1.5m,
                SourceChannel = "Email",
                AffectedVersion = "ERP XL 2024.0",
                CreatedAt = today.AddDays(-5).AddHours(12),
                UpdatedAt = today.AddDays(-3).AddHours(10),
                DueAt = today.AddDays(6).AddHours(14),
                RequiresDeployment = true,
                IsBillable = true
            },
            new()
            {
                Number = "SRV-2603-010",
                ClientId = clients[4].Id,
                Title = "Walidacja marszruty dla gniazd produkcyjnych",
                Description = "Przy zamianie gniazda system nie przelicza czasu przezbrojenia na operacji koncowej.",
                Module = "Produkcja",
                Status = "Nowe",
                Priority = "Wysoki",
                AssignedEngineer = "Marta Kozak",
                PlannedHours = 7,
                SpentHours = 0.5m,
                SourceChannel = "Telefon",
                AffectedVersion = "ERP XL 2024.1",
                CreatedAt = today.AddDays(-1).AddHours(9),
                UpdatedAt = today.AddDays(-1).AddHours(9),
                DueAt = today.AddDays(1).AddHours(9),
                RequiresDeployment = false,
                IsBillable = false
            }
        };

        var integrations = new List<IntegrationFlow>
        {
            new()
            {
                ClientId = clients[0].Id,
                Name = "XL -> DMS Faktury",
                SourceSystem = "Comarch ERP XL",
                TargetSystem = "VSoft DMS",
                Status = "Monitorowana",
                Owner = "Bartłomiej Przybycień",
                Criticality = "Wysoka",
                SuccessRate = 97.8m,
                ErrorCount = 2,
                LastRunAt = today.AddHours(-2),
                NextRunAt = today.AddHours(2)
            },
            new()
            {
                ClientId = clients[1].Id,
                Name = "XL -> B2B API",
                SourceSystem = "Comarch ERP XL",
                TargetSystem = "Portal B2B",
                Status = "Wymaga reakcji",
                Owner = "Katarzyna Wrobel",
                Criticality = "Wysoka",
                SuccessRate = 91.4m,
                ErrorCount = 6,
                LastRunAt = today.AddMinutes(-35),
                NextRunAt = today.AddMinutes(25)
            },
            new()
            {
                ClientId = clients[2].Id,
                Name = "WMS Scanner Import",
                SourceSystem = "Terminale Zebra",
                TargetSystem = "Comarch ERP XL",
                Status = "Monitorowana",
                Owner = "Marta Kozak",
                Criticality = "Wysoka",
                SuccessRate = 95.1m,
                ErrorCount = 3,
                LastRunAt = today.AddMinutes(-18),
                NextRunAt = today.AddMinutes(42)
            },
            new()
            {
                ClientId = clients[3].Id,
                Name = "Optima -> Hurtownia danych",
                SourceSystem = "Comarch Optima",
                TargetSystem = "SQL Warehouse",
                Status = "Stabilna",
                Owner = "Kamil Pluta",
                Criticality = "Srednia",
                SuccessRate = 99.2m,
                ErrorCount = 0,
                LastRunAt = today.AddHours(-6),
                NextRunAt = today.AddHours(6)
            },
            new()
            {
                ClientId = clients[4].Id,
                Name = "MES Feedback Loop",
                SourceSystem = "MES",
                TargetSystem = "Comarch ERP XL",
                Status = "Wymaga reakcji",
                Owner = "Bartłomiej Przybycień",
                Criticality = "Wysoka",
                SuccessRate = 88.7m,
                ErrorCount = 8,
                LastRunAt = today.AddHours(-1),
                NextRunAt = today.AddHours(1)
            },
            new()
            {
                ClientId = clients[5].Id,
                Name = "XL -> Kurier API",
                SourceSystem = "Comarch ERP XL",
                TargetSystem = "InPost API",
                Status = "Stabilna",
                Owner = "Marta Kozak",
                Criticality = "Srednia",
                SuccessRate = 98.6m,
                ErrorCount = 1,
                LastRunAt = today.AddHours(-3),
                NextRunAt = today.AddHours(3)
            }
        };

        await db.ServiceTickets.AddRangeAsync(tickets);
        await db.IntegrationFlows.AddRangeAsync(integrations);
        await db.SaveChangesAsync();
    }

    private static async Task RenameAuthorAsync(AppDbContext db)
    {
        var hasChanges = false;

        var tickets = await db.ServiceTickets
            .Where(ticket => ticket.AssignedEngineer == "Bartosz Przybycien" || ticket.AssignedEngineer == "Bartlomiej Przybycien")
            .ToListAsync();

        foreach (var ticket in tickets)
        {
            ticket.AssignedEngineer = "Bartłomiej Przybycień";
            hasChanges = true;
        }

        var integrations = await db.IntegrationFlows
            .Where(integration => integration.Owner == "Bartosz Przybycien" || integration.Owner == "Bartlomiej Przybycien")
            .ToListAsync();

        foreach (var integration in integrations)
        {
            integration.Owner = "Bartłomiej Przybycień";
            hasChanges = true;
        }

        if (hasChanges)
        {
            await db.SaveChangesAsync();
        }
    }
}
