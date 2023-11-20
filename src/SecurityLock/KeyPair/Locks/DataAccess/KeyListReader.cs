namespace SecurityLock.KeyPair;

public abstract class KeyListReader
{
    public abstract Task<bool> ListContains(string key);
}
