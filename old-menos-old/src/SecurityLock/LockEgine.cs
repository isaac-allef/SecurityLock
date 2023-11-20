namespace SecurityLock;

public sealed class LockEngine
{
    public bool IsDisabled { get; private set; } = false;
    public delegate LockResponse UnLocking<T>(T key);


    private IList<UnLocking<(string partA, string partB)>> _locksForKeyPair;
    private IList<UnLocking<string>> _locksForUniqueKey;
    private IList<UnLocking<object?>> _locksKeyless;

    internal LockEngine()
    {
        _locksForKeyPair = new List<UnLocking<(string partA, string partB)>>();
        _locksForUniqueKey = new List<UnLocking<string>>();
        _locksKeyless = new List<UnLocking<object?>>();
    }

    public void Disable()
        => IsDisabled = true;
    
    public void Enable()
        => IsDisabled = false;
    

    internal void AddLock(UnLocking<(string partA, string partB)> l)
        => _locksForKeyPair.Add(l);

    public LockResponse TryUnlock((string partA, string partB) key)
        => RunUnlock(_locksForKeyPair, key);

    public void TryUnlockAndThrows((string partA, string partB) key)
        => RunUnlock(_locksForKeyPair, key, (response) => throw new LockException(response.Message));

    public LockNotification TryUnlockAndNotify((string partA, string partB) key)
    {
        var notification = new LockNotification();
        RunUnlock(_locksForKeyPair, key, notification);
        return notification;
    }

    
    internal void AddLock(UnLocking<string> l)
        => _locksForUniqueKey.Add(l);

    public LockResponse TryUnlock(string key)
        => RunUnlock(_locksForUniqueKey , key);

    public void TryUnlockAndThrows(string key)
        => RunUnlock(_locksForUniqueKey, key, (response) => throw new LockException(response.Message));

    public LockNotification TryUnlockAndNotify(string key)
    {
        var notification = new LockNotification();
        RunUnlock(_locksForUniqueKey, key, notification);
        return notification;
    }

    
    internal void AddLock(UnLocking<object?> l)
        => _locksKeyless.Add(l);

    public LockResponse TryUnlock()
        => RunUnlock(_locksKeyless, null);

    public void TryUnlockAndThrows()
        => RunUnlock(_locksKeyless, null, (response) => throw new LockException(response.Message));
        
    public LockNotification TryUnlockAndNotify()
    {
        var notification = new LockNotification();
        RunUnlock(_locksKeyless, null, notification);
        return notification;
    }

    private LockResponse RunUnlock<T>(IList<UnLocking<T>> unLockingList, T key, LockNotification notification)
    => RunUnlock<T>(unLockingList, key, (response) => notification.AddError(response.Message));

    private LockResponse RunUnlock<T>(IList<UnLocking<T>> unLockingList, T key, Action<LockResponse>? execute = null)
    {
        if (unLockingList.Count.Equals(0)) throw new Exception("Not implemented");
        
        if (IsDisabled) return LockResponse.Unlocked();

        foreach(var unLocking in unLockingList)
        {
            var response = unLocking(key);
            if (response.IsUnlocked.Equals(false))
            {
                if (execute is not null) execute(response);
                else return response;
            }
        }

        return LockResponse.Unlocked();
    }
}
