using System;
using Castle.DynamicProxy;
using ExtendedHubClient.Abstractions.Proxy;

namespace ExtendedHubClient.Proxy.Interceptors
{
    public class InterceptorWrapper : IInterceptorWrapper
    {
        private IMethodHolder _holder;
        
        private readonly object _locker = new object();

        public void Intercept(IInvocation invocation)
        {
            if(invocation == null)
                throw new ArgumentNullException(nameof(invocation));

            if(_holder == null)
                throw new NullReferenceException($"Can't invoke without attached {nameof(IMethodHolder)}");
            
            lock (_locker)
            {
                var name = invocation?.Method.Name;
                var arguments = invocation.Arguments;
                _holder.OnMethodInvoke(name, arguments).GetAwaiter().GetResult();
            }
        }

        public void AttachMethodHolder(IMethodHolder holder)
        {
            lock (_locker)
            {
                _holder = holder ?? throw new ArgumentNullException(nameof(holder));
            }
        }
    }
}