using System;
using Castle.DynamicProxy;
using ExtendedHubClient.Abstractions.Proxy;

namespace ExtendedHubClient.Proxy.Interceptors
{
    public class InterceptorWrapper : IInterceptorWrapper
    {
        private IMethodHolder _holder;

        public void Intercept(IInvocation invocation)
        {
            lock (_holder)
            {
                var name = invocation?.Method.Name ?? throw new ArgumentNullException(nameof(invocation));
                var arguments = invocation.Arguments;
                _holder.OnMethodInvoke(name, arguments).GetAwaiter().GetResult();
            }
        }

        public void AttachMethodHolder(IMethodHolder holder)
        {
            lock (_holder)
            {
                _holder = holder ?? throw new ArgumentNullException(nameof(holder));
            }
        }
    }
}