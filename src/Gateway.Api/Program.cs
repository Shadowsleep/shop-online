using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("ocelot.json", false, true);

builder.Services.AddOcelot()
    .AddConsul();

var app = builder.Build();

// Usar Ocelot
await app.UseOcelot();

app.Run();
