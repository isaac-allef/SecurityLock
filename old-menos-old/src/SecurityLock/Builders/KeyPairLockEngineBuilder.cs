using SecurityLock.KeyPair;

namespace SecurityLock;

public sealed class KeyPairLockEngineBuilder
{
    private string _context;
    private LockEngine _engine;

    internal KeyPairLockEngineBuilder(LockEngine engine, string context = "")
    {
        _engine = engine;
        _context = context;
    }

    // public LockEngine Build()
    // {
    //     return _engine;
    // }

    // public static implicit operator LockEngine(KeyPairLockEngineBuilder builder)
    //     => builder.Build();

    /*public*/private KeyPairLockEngineBuilder AddLock(LockEngine.UnLocking<(string partA, string partB)> _lock)
    {
        _engine.AddLock(_lock);
        return this;
    }

    public KeyPairLockEngineBuilder UseCombinationLimitAWithB(int limit, TimeSpan expiresIn, CombinationLimiter? combinationLimiter = null)
    {
        combinationLimiter ??= new MemoryCombinationLimiter(_context);
        return AddLock(new CombinationLimiterLock(combinationLimiter, limit, expiresIn).TryUnlock);
    }

    public KeyPairLockEngineBuilder UseCombinationLimitBWithA(int limit, TimeSpan expiresIn, CombinationLimiter? combinationLimiter = null)
    {
        combinationLimiter ??= new MemoryCombinationLimiter(_context);
        var l = new CombinationLimiterLock(combinationLimiter, limit, expiresIn);
        return AddLock(new KeysInverterDecorator(l).TryUnlock);
    }
}
