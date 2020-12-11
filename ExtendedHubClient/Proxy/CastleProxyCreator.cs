using Castle.DynamicProxy;
using ExtendedHubClient.Abstractions.Proxy;

namespace ExtendedHubClient.Proxy
{
    public class CastleProxyCreator<TInterceptor> : IProxyCreator
    where TInterceptor: class, IInterceptorWrapper, new()
    {
        protected readonly ProxyGenerator ProxyGenerator;

        public CastleProxyCreator(IProxyBuilder proxyBuilder = null)
        {
            ProxyGenerator = new ProxyGenerator(proxyBuilder ?? new DefaultProxyBuilder());
        }

        public TInterface CreateProxyForInterface<TInterface>(IMethodProxy holder)
            where TInterface : class
        {
            var interceptor = new TInterceptor();
            interceptor.AttachMethodHolder(holder);
            return ProxyGenerator.CreateInterfaceProxyWithoutTarget<TInterface>(interceptor);
        }
    }
}