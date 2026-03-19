namespace Palickaj.Models;

public static class SupportCatalog
{
    public static readonly string[] TicketStatuses =
    [
        "Nowe",
        "W realizacji",
        "Czeka na klienta",
        "Zaplanowane",
        "Zamkniete"
    ];

    public static readonly string[] TicketPriorities =
    [
        "Krytyczny",
        "Wysoki",
        "Sredni",
        "Niski"
    ];

    public static readonly string[] Modules =
    [
        "Administracja",
        "Finanse",
        "Handel",
        "Integracje",
        "Logistyka",
        "Magazyn",
        "Produkcja"
    ];

    public static readonly string[] SourceChannels =
    [
        "Email",
        "Telefon",
        "Portal",
        "Spotkanie"
    ];

    public static readonly string[] Engineers =
    [
        "Bartłomiej Przybycień",
        "Katarzyna Wrobel",
        "Marta Kozak",
        "Kamil Pluta"
    ];

    public static readonly string[] SupportPlans =
    [
        "Start",
        "Business",
        "Enterprise"
    ];

    public static readonly string[] IntegrationStatuses =
    [
        "Stabilna",
        "Monitorowana",
        "Wymaga reakcji"
    ];

    public static readonly string[] CriticalityLevels =
    [
        "Niska",
        "Srednia",
        "Wysoka"
    ];
}
