namespace ExtendedHubClient.Abstractions.Proxy
{
    /// <summary>
    /// Provides an interface for creating proxy objects for calling server methods.
    /// </summary>
    public interface IProxyCreator
    {
        TInterface CreateProxyForInterface<TInterface>(IMethodHolder holder) where TInterface: class;
    }
}