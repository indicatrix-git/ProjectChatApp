using ChatApi.Data;
using ChatApi.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- Database (Entity Framework Core + MySQL via Pomelo) ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ChatContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// --- Web API + SignalR ---
builder.Services.AddControllers();
builder.Services.AddSignalR();

// --- CORS (handy if you ever connect from a browser; harmless for WPF) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(_ => true)
              .AllowCredentials());
});

var app = builder.Build();

// --- Create the database and tables automatically on first run ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ChatContext>();
    db.Database.EnsureCreated();
}

app.UseCors("AllowAll");

app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.Run();
