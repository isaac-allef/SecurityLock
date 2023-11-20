namespace SecurityLock.KeyPair;

public sealed partial class KeyPairLockEngine
{
    public string Context { get; } = string.Empty;
    public bool IsDisabled { get; private set; } = false;
    private IList<ILock> _locks;

    private KeyPairLockEngine(IList<ILock> locks)
    {
        _locks = locks;
    }

    private KeyPairLockEngine(string context, IList<ILock> locks)
    {
        Context = context;
        _locks = locks;
    }

    private void AddLock(ILock _lock)
        => _locks.Add(_lock);
    
    public void Disable()
        => IsDisabled = true;
    
    public void Enable()
        => IsDisabled = false;
    
    public LockResponse TryUnlock((string keyA, string keyB) keys)
        => RunUnlock(keys);
        
    public void TryUnlockAndThrows((string keyA, string keyB) keys)
        => RunUnlock(keys, (response) => throw new LockException(response.Message));

    public LockNotification TryUnlockAndNotify((string keyA, string keyB) keys)
    {
        var notification = new LockNotification();
        RunUnlock(keys, (response) => notification.AddError(response.Message));
        return notification;
    }

    private LockResponse RunUnlock((string keyA, string keyB) keys, Action<LockResponse>? execute = null)
    {
        if (IsDisabled) return LockResponse.Unlocked();

        if (string.IsNullOrWhiteSpace(keys.keyA) 
        || string.IsNullOrWhiteSpace(keys.keyB))
        {
            return LockResponse.Locked("No key can be empty");
        }

        foreach(var _lock in _locks)
        {
            var response = _lock.TryUnlock(keys);
            if (response.IsUnlocked.Equals(false))
            {
                if (execute is not null) execute(response);
                else return response;
            }
        }

        return LockResponse.Unlocked();
    }
}
