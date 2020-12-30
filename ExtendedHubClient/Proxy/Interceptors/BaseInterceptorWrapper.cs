using System;
using Castle.DynamicProxy;
using ExtendedHubClient.Abstractions.Proxy;

namespace ExtendedHubClient.Proxy.Interceptors
{
    public abstract class BaseInterceptorWrapper : AsyncInterceptorBase
    {
        protected readonly IMethodProxy MethodProxy;

        protected BaseInterceptorWrapper(IMethodProxy methodProxy)
        {
            MethodProxy = methodProxy ?? throw new ArgumentNullException(nameof(methodProxy));
        }
    }
}