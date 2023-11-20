using SecurityLock.Key;

namespace SecurityLock;


public sealed class KeyLockEngineBuilder
{
    private string _context;
    private LockEngine _engine;

    internal KeyLockEngineBuilder(LockEngine engine, string context = "")
    {
        _engine = engine;
        _context = context;
    }

    // public LockEngine Build()
    // {
    //     return _engine;
    // }

    // public static implicit operator LockEngine(KeyLockEngineBuilder builder)
    //     => builder.Build();

    /*public*/private KeyLockEngineBuilder AddLock(LockEngine.UnLocking<string> _lock)
    {
        _engine.AddLock(_lock);
        return this;
    }

    // fazer uma AllowList -> travar para quem não está na lista

    public KeyLockEngineBuilder UseBlockList(BlockList blockList) // por exemplo um txt
    {
        return AddLock(new BlockListLock(blockList).TryUnlock);
    }

    public KeyLockEngineBuilder UseRateLimit(int limit, TimeSpan period, RateLimiter? rateLimiter = null)
    {
        rateLimiter ??= new MemoryTokenBucketRateLimiter(_context);
        return AddLock(new RateLimiterLock(rateLimiter, limit, period).TryUnlock);
    }
}
