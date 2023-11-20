using SecurityLock.KeyPair;
using SecurityLock.KeyPair.AspNet;
using static SecurityLock.KeyPair.KeyPairLockEngine;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var engine = new KeyPairLockEngineBuilder("engine1")
                .UseBlockList(ForKey.A, (MemoryBlockList)new List<string>() { "A1" })
                .UseBlockList(ForKey.B, (MemoryBlockList)new List<string>() { "B1" })
                .UseRateLimit(ForKey.A, 1, TimeSpan.FromSeconds(5))
                .Build();
builder.Services.AddScoped<KeyPairLockEngine>(_ => engine);

var app = builder.Build();

// app.Map("", a => a.UseMiddleware<SecurityLockKeyPairMiddleware>());
app.UseMiddleware<SecurityLockKeyPairMiddleware>();

app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();
