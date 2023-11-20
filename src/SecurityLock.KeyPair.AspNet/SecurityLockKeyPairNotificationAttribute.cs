namespace SecurityLock.KeyPair.AspNet;

public sealed class SecurityLockKeyPairNotificationAttribute : SecurityLockKeyPairAttribute
{
    public SecurityLockKeyPairNotificationAttribute(string keyAFieldName, string keyBFieldName) : base(keyAFieldName, keyBFieldName)
    {
    }

    public SecurityLockKeyPairNotificationAttribute(string context, string keyAFieldName, string keyBFieldName) : base(context, keyAFieldName, keyBFieldName)
    {
    }
}
