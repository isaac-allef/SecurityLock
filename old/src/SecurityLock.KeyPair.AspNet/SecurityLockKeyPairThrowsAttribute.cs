namespace SecurityLock.KeyPair.AspNet;

public sealed class SecurityLockKeyPairThrowsAttribute : SecurityLockKeyPairAttribute
{
    public SecurityLockKeyPairThrowsAttribute(string keyAFieldName, string keyBFieldName) : base(keyAFieldName, keyBFieldName)
    {
    }

    public SecurityLockKeyPairThrowsAttribute(string context, string keyAFieldName, string keyBFieldName) : base(context, keyAFieldName, keyBFieldName)
    {
    }
}
