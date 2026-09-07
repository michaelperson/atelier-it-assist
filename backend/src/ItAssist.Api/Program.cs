using ItAssist.Api.Infrastructure;
using ItAssist.Domain.Commun;
using ItAssist.Domain.Tickets;

var builder = WebApplication.CreateBuilder(args);

var cheminTickets = builder.Configuration["Tickets:Chemin"]
    ?? Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "..", "data", "tickets.jsonl");

builder.Services.AddControllers();
builder.Services.AddSingleton<IHorloge, HorlogeSysteme>();
builder.Services.AddSingleton<IJournal, JournalConsole>();
builder.Services.AddSingleton<IDepotTickets>(sp =>
    new DepotTicketsFichier(Path.GetFullPath(cheminTickets), sp.GetRequiredService<IJournal>()));
builder.Services.AddSingleton<RoutageService>();
builder.Services.AddSingleton<TicketService>();
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p
    .WithOrigins("http://localhost:4200")
    .AllowAnyHeader()
    .AllowAnyMethod()));

var app = builder.Build();

app.UseCors();
app.MapControllers();
app.MapGet("/sante", () => Results.Ok(new { statut = "ok" }));

app.Run();
