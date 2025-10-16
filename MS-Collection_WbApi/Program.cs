using Microsoft.EntityFrameworkCore;
using MS_Collection_WbApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ?? Conexión a la base de datos
builder.Services.AddDbContext<JoyeriaMsBdContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("connectionDB")));

// ?? Servicio de pagos
builder.Services.AddScoped<IPaymentService, StripePaymentService>();

// ?? CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:44345") // <-- URL del frontend Blazor
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ?? HttpClient base para consumo de API
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:44300/") // <- Asegúrate que esta sea tu API
});

// ?? Controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ?? Redirigir raíz a Swagger
app.MapGet("/", (HttpContext context) =>
{
    context.Response.Redirect("/swagger/index.html", permanent: false);
});

// ?? Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting(); // ?? NECESARIO para que funcione CORS
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();
app.Run();
