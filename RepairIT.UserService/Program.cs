using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RepairIT.UserService.Data;
using RepairIT.UserService.Services;
using RepairIT.Shared.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Configuration : la section "Application" est exposée via IOptions<ApplicationOptions>
builder.Services.Configure<ApplicationOptions>(builder.Configuration.GetSection(ApplicationOptions.Section));

// EF Core + SQLite : la chaîne de connexion vient de appsettings.json (pas codée en dur)
builder.Services.AddDbContext<UtilisateurContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("UtilisateursDb")));

// Injection de dépendances : Scoped, comme le DbContext dont le service dépend
builder.Services.AddScoped<IUtilisateurService, UtilisateurService>();

// Les énumérations (RoleUtilisateur) circulent en texte dans le JSON : "Gestionnaire" plutôt que 0
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Initialisation de la base (migrations + données de départ) avant de servir les requêtes
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UtilisateurContext>();
    UtilisateurDbInitializer.Initialiser(context, app.Logger);
}

// Swagger : http://localhost:5002/swagger (environnement de développement uniquement)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

// GET api/info : nom et version du service, lus dans la configuration
app.MapGet("/api/info", (IOptions<ApplicationOptions> options) => options.Value);

app.Run();
