using simulator_console.Services.PlayerPackage;
using simulator_console.Services.Simulator;
using simulator_console.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddSingleton<PlayerSerivce>();
builder.Services.AddSingleton<GameSimulatorSerivce>();

builder.Services.AddDbContext<simulator_console.Data.GameDbContext>(options =>
    options.UseSqlite("Data Source=baseball.db"), ServiceLifetime.Singleton); 
// Note: Using Singleton DbContext because GameSimulatorService is Singleton. 
// Ideally GameSimulatorService should be Scoped, but for this simulation loop it's Singleton.
// We must ensure DbContext is also Singleton or use a ScopeFactory.
// Given the simplicity, Singleton DbContext with SQLite is acceptable for this single-user demo.

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:5173", "http://127.0.0.1:5500") // Common frontend ports
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();
app.MapHub<GameHub>("/gameHub");

app.Run();