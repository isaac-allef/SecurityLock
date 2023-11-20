namespace SecurityLock;

public sealed class SecurityLockBuilder
{
    private string _context;
    private LockEngine _engine;

    public SecurityLockBuilder(string context = "")
    {
        _context = context;
        _engine = new LockEngine();
    }

    public LockEngine Build()
    {
        return _engine;
    }

    public static implicit operator LockEngine(SecurityLockBuilder builder)
        => builder.Build();

    public SecurityLockBuilder ForKeyPair(Action<KeyPairLockEngineBuilder> b)
    {
        b(new KeyPairLockEngineBuilder(_engine));
        return this;
    }

    public SecurityLockBuilder ForUniqueKey(Action<KeyLockEngineBuilder> b)
    {
        b(new KeyLockEngineBuilder(_engine));
        return this;
    }

    public void Teste()
    {
        ForKeyPair(b => {
            b.UseCombinationLimitAWithB(1, TimeSpan.FromSeconds(5));
            b.UseCombinationLimitBWithA(1, TimeSpan.FromSeconds(5));
        });
    }
}
