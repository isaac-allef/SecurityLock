using SecurityLock;
using SecurityLock.Key;
using SecurityLock.KeyPair;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var engine1 = new SecurityLockBuilder("engine1")
            .ForKeyPair(b =>
            {
                b.UseCombinationLimitAWithB(1, TimeSpan.FromSeconds(3));
                b.UseCombinationLimitBWithA(1, TimeSpan.FromSeconds(3));
            })
            .ForUniqueKey(b =>
            {
                b.UseBlockList((MemoryBlockList)new List<string>() { "123", "B" });
                b.UseRateLimit(1, TimeSpan.FromSeconds(5));
            })
            .Build();

builder.Services.AddScoped(_ => engine1);

var app = builder.Build();

app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();
