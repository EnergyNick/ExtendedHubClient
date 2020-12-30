using Castle.DynamicProxy;
using ExtendedHubClient.Proxy.Interceptors.Factory;

namespace ExtendedHubClient.Proxy
{
    public class DefaultProxyCreator : CastleProxyCreator
    {
        public DefaultProxyCreator(IProxyBuilder proxyBuilder = null) 
            : base(new DefaultInterceptorFactory(), proxyBuilder)
        { }
    }
}