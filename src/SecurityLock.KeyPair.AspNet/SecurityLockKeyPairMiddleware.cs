using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using SecurityLock.KeyPair;

namespace SecurityLock.KeyPair.AspNet;

/// <summary>
/// 
/// </summary>
public class SecurityLockKeyPairMiddleware
{
#pragma warning disable CS0109 // Member does not hide an inherited member; new keyword is not required
    private readonly RequestDelegate _next;
#pragma warning restore CS0109 // Member does not hide an inherited member; new keyword is not required

    /// <summary>
    /// 
    /// </summary>
    /// <param name="next"></param>
    public SecurityLockKeyPairMiddleware(RequestDelegate next)
    {
        _next = next;
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public async Task InvokeAsync(HttpContext context, KeyPairLockEngine engine)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        var attribute = GetSecurityLockKeyPairAttribute(context);
        if (attribute is null || !engine.Context.Equals(attribute.Context))
        {
            await _next(context);
            return;
        }

        var keyA = GetKeyValue(context.Request, attribute.KeyAFieldName);
        var keyB = GetKeyValue(context.Request, attribute.KeyBFieldName);

        if (attribute.GetType().Equals(typeof(SecurityLockKeyPairThrowsAttribute)))
        {
            engine.TryUnlockAndThrows((keyA, keyB));
        }
        else if (attribute.GetType().Equals(typeof(SecurityLockKeyPairNotificationAttribute)))
        {
            var result = await engine.TryUnlockAndNotify((keyA, keyB));
            if (result.IsUnlocked.Equals(false))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(result.Errors);
                return;
            }
        }
        else
        {
            var result = await engine.TryUnlock((keyA, keyB));
            if (result.IsUnlocked.Equals(false))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync(result.Message);
                return;
            }
        }

        await _next(context);
    }

    private SecurityLockKeyPairAttribute? GetSecurityLockKeyPairAttribute(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint is null)
        {
            return null;
        }

        var action = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
        if (action is null)
        {
            return null;
        }

        var attribute = action.MethodInfo.GetCustomAttribute<SecurityLockKeyPairAttribute>();
        return attribute;
    }

    private string GetKeyValue(HttpRequest request, string keyName)
    {
        var keyValue = request.Headers[keyName];

        if (string.IsNullOrWhiteSpace(keyValue))
        {
            keyValue = request.Query[keyName];
        }

        if (string.IsNullOrWhiteSpace(keyValue) 
        && request.RouteValues.TryGetValue(keyName, out var routeValue))
        {
            keyValue = routeValue?.ToString();
        }

        return keyValue.ToString();
    }
}
