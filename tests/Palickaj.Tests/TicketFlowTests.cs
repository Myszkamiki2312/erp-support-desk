using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Palickaj.Data;

namespace Palickaj.Tests;

public sealed class TicketFlowTests
{
    [Fact]
    public async Task CreateForm_ShouldRenderCleanPolishLabels()
    {
        using var factory = new PalickajWebApplicationFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/Tickets/Create");
        var html = WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Dodaj nowe zgloszenie", html);
        Assert.Contains("Osoba odpowiedzialna", html);
        Assert.Contains("Przepracowane godziny", html);
        Assert.Contains("Wymaga wdrozenia na produkcje", html);
        Assert.Contains("Rozlicz jako prace platna", html);
        Assert.Contains("Bart\u0142omiej Przybycie\u0144", html);
        Assert.DoesNotContain("ownership", html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Rozliczaj jako prace billable", html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("deploymentu produkcyjnego", html, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateTicket_ShouldPersistAndBeVisibleOnDetailsAndList()
    {
        using var factory = new PalickajWebApplicationFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });

        var createPage = await client.GetAsync("/Tickets/Create");
        var createHtml = await createPage.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, createPage.StatusCode);

        var token = ExtractRequired(createHtml, "__RequestVerificationToken\" type=\"hidden\" value=\"([^\"]+)\"");
        var uniqueTitle = $"Test integracyjny {Guid.NewGuid():N}";
        string clientId;

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            clientId = await db.Clients
                .AsNoTracking()
                .OrderBy(client => client.Id)
                .Select(client => client.Id.ToString())
                .FirstAsync();
        }

        var formData = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["ClientId"] = clientId,
            ["Title"] = uniqueTitle,
            ["Description"] = "Automatyczny test dodawania zgloszenia i sprawdzenia widokow.",
            ["Module"] = "Integracje",
            ["Status"] = "Nowe",
            ["Priority"] = "Wysoki",
            ["AssignedEngineer"] = "Bart\u0142omiej Przybycie\u0144",
            ["PlannedHours"] = "4",
            ["SpentHours"] = "1",
            ["SourceChannel"] = "Portal",
            ["AffectedVersion"] = "Comarch ERP XL 2024.1",
            ["DueAt"] = "2026-03-25T12:30"
        };

        var postResponse = await client.PostAsync("/Tickets/Create", new FormUrlEncodedContent(formData));

        Assert.Equal(HttpStatusCode.Redirect, postResponse.StatusCode);
        Assert.NotNull(postResponse.Headers.Location);
        Assert.StartsWith("/Tickets/Details/", postResponse.Headers.Location!.OriginalString, StringComparison.Ordinal);

        var detailsResponse = await client.GetAsync(postResponse.Headers.Location);
        var detailsHtml = WebUtility.HtmlDecode(await detailsResponse.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, detailsResponse.StatusCode);
        Assert.Contains(uniqueTitle, detailsHtml);
        Assert.Contains("Bart\u0142omiej Przybycie\u0144", detailsHtml);
        Assert.Contains("Comarch ERP XL 2024.1", detailsHtml);

        var listResponse = await client.GetAsync($"/Tickets?search={Uri.EscapeDataString(uniqueTitle)}");
        var listHtml = WebUtility.HtmlDecode(await listResponse.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        Assert.Contains(uniqueTitle, listHtml);
        Assert.Contains("Wysoki", listHtml);
    }

    private static string ExtractRequired(string html, string pattern)
    {
        var match = Regex.Match(html, pattern, RegexOptions.IgnoreCase);
        Assert.True(match.Success, $"Pattern not found: {pattern}");
        return match.Groups[1].Value;
    }
}
