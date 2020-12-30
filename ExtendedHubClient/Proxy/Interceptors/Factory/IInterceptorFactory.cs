using ExtendedHubClient.Abstractions.Proxy;

namespace ExtendedHubClient.Proxy.Interceptors.Factory
{
    public interface IInterceptorFactory
    {
        public BaseInterceptorWrapper CreateInterceptorWrapper(IMethodProxy methodProxy);
    }
}