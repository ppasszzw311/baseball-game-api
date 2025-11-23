using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using simulator_console.Services.PlayerPackage;
using simulator_console.Services.Simulator;

// Set up DI
var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddScoped<PlayerSerivce>();
builder.Services.AddScoped<GameSimulatorSerivce>();

var host = builder.Build();

// Get the service and run the simulation
var gameSimulator = host.Services.GetRequiredService<GameSimulatorSerivce>();

Console.WriteLine("Starting simulation...");
gameSimulator.RunSimulation();
Console.WriteLine("Simulation finished.");
