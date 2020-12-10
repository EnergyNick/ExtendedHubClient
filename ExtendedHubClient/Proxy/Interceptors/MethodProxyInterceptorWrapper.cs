using System;
using Castle.DynamicProxy;
using ExtendedHubClient.Abstractions;
using ExtendedHubClient.Abstractions.Proxy;

namespace ExtendedHubClient.Proxy.Interceptors
{
    public class MethodProxyInterceptorWrapper : IInterceptorWrapper
    {
        private IMethodProxy _methodProxy;
        
        private readonly object _locker = new object();

        public void Intercept(IInvocation invocation)
        {
            if(invocation == null)
                throw new ArgumentNullException(nameof(invocation));
            
            if(_methodProxy == null)
                throw new NullReferenceException($"Can't invoke without attached {nameof(IMethodProxy)}");

            if (invocation.Arguments.Length != 2
                || !(invocation.Arguments[0] is string)
                || !(invocation.Arguments[1] is object[]))
                throw new InvalidOperationException(
                    $"{nameof(MethodProxyInterceptorWrapper)} can only work with {nameof(IHubClient)}");
            
            lock (_locker)
            {
                var name = invocation.Arguments[0] as string;
                var arguments = invocation.Arguments[1] as object[];
                invocation.ReturnValue = _methodProxy.OnMethodInvoke(name, arguments);
            }
        }

        public void AttachMethodHolder(IMethodProxy holder)
        {
            lock (_locker)
            {
                _methodProxy = holder ?? throw new ArgumentNullException(nameof(holder));
            }
        }
    }
}