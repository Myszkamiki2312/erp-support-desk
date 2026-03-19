using Microsoft.EntityFrameworkCore;
using ErpSupportDesk.Data;
using ErpSupportDesk.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddCors(options =>
{
    options.AddPolicy("spa", policy =>
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod());
});
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var provider = builder.Configuration["Database:Provider"] ?? "Sqlite";

    if (string.Equals(provider, "SqlServer", StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection"));
    }
    else
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<TicketNumberGenerator>();

var app = builder.Build();

await DataSeeder.SeedAsync(app.Services);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("spa");
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public partial class Program
{
}
