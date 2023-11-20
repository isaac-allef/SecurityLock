using SecurityLock.KeyPair;

namespace SecurityLock;

public sealed class KeyPairLockEngineBuilder
{
    private string _context;
    private KeyPairLockEngine _engine;

    public KeyPairLockEngineBuilder(string context = "")
    {
        _engine = new KeyPairLockEngine(context);
        _context = context;
    }

    public KeyPairLockEngine Build()
    {
        return _engine;
    }

    public static implicit operator KeyPairLockEngine(KeyPairLockEngineBuilder builder)
        => builder.Build();

    /*public*/private KeyPairLockEngineBuilder AddLock(KeyPairLockEngine.UnLocking _lock)
    {
        _engine.AddLock(_lock);
        return this;
    }

    public KeyPairLockEngineBuilder UseCombinationLimitAWithB(int limit, TimeSpan expiresIn, KeyValueAccess<ISet<(string, DateTime)>?>? combinationLimiter = null)
    {
        combinationLimiter ??= new MemoryKeyValueAccess<ISet<(string, DateTime)>?>(_context+"-"+"combination");
        return AddLock(new CombinationLimiterLock(combinationLimiter, limit, expiresIn).TryUnlock);
    }

    public KeyPairLockEngineBuilder UseCombinationLimitBWithA(int limit, TimeSpan expiresIn, KeyValueAccess<ISet<(string, DateTime)>?>? combinationLimiter = null)
    {
        combinationLimiter ??= new MemoryKeyValueAccess<ISet<(string, DateTime)>?>(_context+"-"+"combination");
        var combinationLimiterLock = new CombinationLimiterLock(combinationLimiter, limit, expiresIn);
        return AddLock(new KeysInverterDecorator(combinationLimiterLock.TryUnlock).TryUnlock);
    }

    public KeyPairLockEngineBuilder UseBlockListToA(KeyListReader blockList) // por exemplo um txt
    {
        return AddLock(new BlockListLock(blockList).TryUnlock);
    }

    public KeyPairLockEngineBuilder UseBlockListToB(KeyListReader blockList) // por exemplo um txt
    {
        var blockListLock = new BlockListLock(blockList);
        return AddLock(new KeysInverterDecorator(blockListLock.TryUnlock).TryUnlock);
    }

    public KeyPairLockEngineBuilder UseRateLimitToA(int limit, TimeSpan period, KeyValueAccess<(int, DateTime)?>? rateLimiter = null)
    {
        rateLimiter ??= new MemoryKeyValueAccess<(int, DateTime)?>(_context+"-"+"rateLimite");
        return AddLock(new RateLimiterLock(rateLimiter, limit, period).TryUnlock);
    }

    public KeyPairLockEngineBuilder UseRateLimitToB(int limit, TimeSpan period, KeyValueAccess<(int, DateTime)?>? rateLimiter = null)
    {
        rateLimiter ??= new MemoryKeyValueAccess<(int, DateTime)?>(_context+"-"+"rateLimite");
        var rateLimiterLock = new RateLimiterLock(rateLimiter, limit, period);
        return AddLock(new KeysInverterDecorator(rateLimiterLock.TryUnlock).TryUnlock);
    }
}
