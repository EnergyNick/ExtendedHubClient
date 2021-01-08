using System;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using ExtendedHubClient.Abstractions.Proxy;

namespace ExtendedHubClient.Proxy.Interceptors
{
    public class InterceptorWrapper : BaseInterceptorWrapper
    {
        public InterceptorWrapper(IMethodProxy methodProxy) 
            : base(methodProxy)
        { }

        protected override Task InterceptAsync(IInvocation invocation, Func<IInvocation, Task> proceed)
        {
            if(invocation == null)
                throw new ArgumentNullException(nameof(invocation));

            var name = invocation?.Method.Name;
            var arguments = invocation.Arguments;
            return MethodProxy.OnMethodInvoke(name, arguments);
        }

        protected override Task<TResult> InterceptAsync<TResult>(IInvocation invocation, Func<IInvocation, Task<TResult>> proceed)
        {
            if(invocation == null)
                throw new ArgumentNullException(nameof(invocation));

            if(MethodProxy == null)
                throw new NullReferenceException($"Can't invoke without attached {nameof(IMethodProxy)}");
            
            var name = invocation?.Method.Name;
            var arguments = invocation.Arguments;
            return MethodProxy.OnMethodInvokeWithReturnValue<TResult>(name, arguments);
        }
    }
}