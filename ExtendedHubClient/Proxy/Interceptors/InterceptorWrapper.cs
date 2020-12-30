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

        protected override async Task InterceptAsync(IInvocation invocation, Func<IInvocation, Task> proceed)
        {
            if(invocation == null)
                throw new ArgumentNullException(nameof(invocation));

            var name = invocation?.Method.Name;
            var arguments = invocation.Arguments;
            await MethodProxy.OnMethodInvoke(name, arguments).ConfigureAwait(false);
        }

        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, Func<IInvocation, Task<TResult>> proceed)
        {
            if(invocation == null)
                throw new ArgumentNullException(nameof(invocation));

            if(MethodProxy == null)
                throw new NullReferenceException($"Can't invoke without attached {nameof(IMethodProxy)}");
            
            var name = invocation?.Method.Name;
            var arguments = invocation.Arguments;
            var returnType = invocation.Method.ReturnType;
            return (TResult) await MethodProxy.OnMethodInvokeWithReturnValue(name, arguments, returnType)
                .ConfigureAwait(false);
        }
    }
}