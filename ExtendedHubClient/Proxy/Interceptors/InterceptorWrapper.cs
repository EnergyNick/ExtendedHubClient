using System;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using ExtendedHubClient.Abstractions.Proxy;

namespace ExtendedHubClient.Proxy.Interceptors
{
    public class InterceptorWrapper : IInterceptorWrapper
    {
        private IMethodProxy _methodProxy;
        
        private readonly object _locker = new object();

        public void Intercept(IInvocation invocation)
        {
            if(invocation == null)
                throw new ArgumentNullException(nameof(invocation));

            if(_methodProxy == null)
                throw new NullReferenceException($"Can't invoke without attached {nameof(IMethodProxy)}");
            
            lock (_locker)
            {
                var name = invocation?.Method.Name;
                var arguments = invocation.Arguments;
                var returnType = invocation.MethodInvocationTarget.ReturnType;
                invocation.ReturnValue = returnType == typeof(Task)
                    ? _methodProxy.OnMethodInvoke(name, arguments)
                    : _methodProxy.OnMethodInvokeWithReturnValue(name, arguments, returnType);
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