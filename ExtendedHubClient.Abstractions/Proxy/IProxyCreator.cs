namespace ExtendedHubClient.Abstractions.Proxy
{
    public interface IProxyCreator
    {
        TInterface CreateProxyForInterface<TInterface>(IMethodHolder holder) where TInterface: class;
    }
}