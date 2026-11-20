using Microsoft.Extensions.Options;
using RepairIT.Shared.Configuration;

// Squelette du micro-service SalesService (ventes et statistiques).
// Créé en V1 pour poser l'architecture cible ; ses fonctionnalités arrivent en V3.
var builder = WebApplication.CreateBuilder(args);

// Configuration : la section "Application" est exposée via IOptions<ApplicationOptions>
builder.Services.Configure<ApplicationOptions>(builder.Configuration.GetSection(ApplicationOptions.Section));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger : http://localhost:5004/swagger (environnement de développement uniquement)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

// GET api/info : permet de vérifier que le service tourne et quelle version est déployée
app.MapGet("/api/info", (IOptions<ApplicationOptions> options) => options.Value);

app.Run();
