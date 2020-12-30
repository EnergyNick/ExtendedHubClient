using Castle.DynamicProxy;
using ExtendedHubClient.Abstractions.Proxy;
using ExtendedHubClient.Proxy.Interceptors;
using ExtendedHubClient.Proxy.Interceptors.Factory;

namespace ExtendedHubClient.Proxy
{
    public class CastleProxyCreator : IProxyCreator
    {
        protected readonly IInterceptorFactory InterceptorFactory;
        protected readonly IProxyGenerator ProxyGenerator;

        public CastleProxyCreator(IInterceptorFactory interceptorFactory, IProxyBuilder proxyBuilder = null)
        {
            ProxyGenerator = new ProxyGenerator(proxyBuilder ?? new DefaultProxyBuilder());
            InterceptorFactory = interceptorFactory;
        }

        public TInterface CreateProxyForInterface<TInterface>(IMethodProxy holder)
            where TInterface : class
        {
            var interceptor = InterceptorFactory.CreateInterceptorWrapper(holder);
            return ProxyGenerator.CreateInterfaceProxyWithoutTarget<TInterface>(interceptor.ToInterceptor());
        }
    }
}