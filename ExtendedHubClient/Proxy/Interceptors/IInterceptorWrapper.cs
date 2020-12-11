using Castle.DynamicProxy;
using ExtendedHubClient.Abstractions.Proxy;

namespace ExtendedHubClient.Proxy
{
    public interface IInterceptorWrapper : IInterceptor
    {
        public void AttachMethodHolder(IMethodProxy holder);
    }
}