namespace SecurityLock;

public sealed class KeyPairLockEngine : LockEngine<(string partA, string partB)>
{
    public KeyPairLockEngine(string context) : base(context)
    {
    }
}
