using SecurityLock.KeyPair;
using static SecurityLock.KeyPair.KeyPairLockEngine;

namespace SecurityLock.KeyPair.Tests;

public class KeyPairLockEngineBuilderTests
{
    [Theory]
    [InlineData("A", "B")]
    public void Test1(string keyA, string keyB)
    {
        var engine = new KeyPairLockEngineBuilder("testContext")
            .UseBlockList(ForKey.A, (MemoryBlockList) new List<string>() { "A1" })
            .UseBlockList(ForKey.B, (MemoryBlockList) new List<string>() { "B1" })
            .UseCombinationLimit(ForKey.A, limit: 2, expiresIn: TimeSpan.FromDays(30))
            .UseCombinationLimit(ForKey.B, limit: 2, expiresIn: TimeSpan.FromDays(30))
            .UseRateLimit(ForKey.A, limit: 1, period: TimeSpan.FromMinutes(2))
            .UseRateLimit(ForKey.B, limit: 1, period: TimeSpan.FromMinutes(2))
            .UsePairOfKeysRateLimit(limit: 1, period: TimeSpan.FromMinutes(2))
            .Build();
        
        var result = engine.TryUnlock((keyA, keyB));

        Assert.True(result.IsUnlocked);
    }
}
