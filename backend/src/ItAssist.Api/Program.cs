using ItAssist.Api.Infrastructure;
using ItAssist.Domain.Commun;
using ItAssist.Domain.Tickets;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;


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

// Configuration Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ItAssist API",
        Version = "v1",
        Description = "API pour la gestion des tickets IT",
        Contact = new OpenApiContact
        {
            Name = "Support ItAssist",
            Email = "support@itassist.local"
        }
    });
});


var app = builder.Build();

// Configuration middleware Swagger et Scalar
if (app.Environment.IsDevelopment())
{
    // Swagger génère le JSON à /openapi/v1.json pour compatibilité avec Scalar
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "openapi/{documentName}.json";
    });

    // Scalar UI utilisera automatiquement /openapi/v1.json
    app.MapScalarApiReference();
}

app.UseCors();
app.MapControllers();
app.MapGet("/sante", () => Results.Ok(new { statut = "ok" }))
    .WithName("Sante")
    .WithDescription("Endpoint de vérification de l'état de santé de l'API")
    .Produces(StatusCodes.Status200OK);

app.Run();

