using NEGOCIO.DATAACCESS;
using NEGOCIO.MODELO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BackendReact.Server.HUB;

var builder = WebApplication.CreateBuilder(args);

// Agregar SignalR a los servicios
builder.Services.AddSignalR();

// Configuraci�n de la cadena de conexi�n
builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));

// Registro de la clase DAL
builder.Services.AddScoped<DAL>();

// Resto de la configuraci�n
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddJsonOptions(options => {
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("http://localhost:1420", "tauri://localhost") // A�adir origen Tauri
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // Importante para SignalR
        });
});
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseCors("AllowSpecificOrigin");

// Mapear los controladores de la API
app.MapControllers();

// Mapear el hub de SignalR
app.MapHub<EstacionHUB>("/EsatcionHUB");

// Configurar fallback para la aplicaci�n SPA
app.MapFallbackToFile("/index.html");

app.Run();
