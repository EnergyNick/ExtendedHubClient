using ExtendedHubClient.Abstractions.Proxy;

namespace ExtendedHubClient.Proxy.Interceptors.Factory
{
    public class DefaultInterceptorFactory : IInterceptorFactory
    {
        public BaseInterceptorWrapper CreateInterceptorWrapper(IMethodProxy methodProxy)
        {
            return new InterceptorWrapper(methodProxy);
        }
    }
}