using Microsoft.EntityFrameworkCore;
using SitiosApi.Application.Interfaces;
using SitiosApi.Application.Services;
using SitiosApi.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// --- Servicios ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Base de datos SQLite (archivo local sitios.db)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Data Source=sitios.db"));

// Inyección de dependencias: interfaz -> implementación (Clean Architecture)
builder.Services.AddScoped<ISitioRepository, SitioRepository>();
builder.Services.AddScoped<ISitioService, SitioService>();

// CORS abierto para que la app MAUI (Android/iOS/Windows) pueda consumir la API
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Permitir subir/recibir JSON de fotos/audio en Base64 (pueden pesar bastante)
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // 50 MB
});
builder.WebHost.ConfigureKestrel(o => o.Limits.MaxRequestBodySize = 50 * 1024 * 1024);

var app = builder.Build();

// Crear la base de datos automáticamente si no existe (útil para el examen: cero configuración)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("PermitirTodo");
app.UseAuthorization();
app.MapControllers();

app.Run();
