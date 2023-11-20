using SecurityLock;
using SecurityLock.KeyPair;
using SecurityLock.KeyPair.AspNet;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var engine1 = new KeyPairLockEngineBuilder("engine1")
            .UseCombinationLimitAWithB(1, TimeSpan.FromSeconds(10))
            // .UseCombinationLimitBWithA(1, TimeSpan.FromSeconds(60))
            .UseBlockListToA((MemoryListReader)new List<string>() { "A1" })
            // .UseBlockListToB((MemoryBlockList)new List<string>() { "B1" })
            .UseRateLimitToA(1, TimeSpan.FromSeconds(10))
            // .UseRateLimitToB(1, TimeSpan.FromSeconds(60))
            .Build();

builder.Services.AddScoped(_ => engine1);

var app = builder.Build();

app.UseMiddleware<SecurityLockKeyPairMiddleware>();

app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();
