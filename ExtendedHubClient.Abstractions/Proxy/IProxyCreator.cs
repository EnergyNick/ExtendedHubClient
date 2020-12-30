namespace ExtendedHubClient.Abstractions.Proxy
{
    /// <summary>
    /// Provides an interface for creating proxy objects for calling server methods.
    /// </summary>
    public interface IProxyCreator
    {
        /// <summary>
        /// Create a proxy for the <see cref="TInterface"/> that handles <see cref="IMethodProxy"/>
        /// </summary>
        /// <param name="methodProxy"></param>
        /// <typeparam name="TInterface">Interface for proxying</typeparam>
        /// <returns>Proxy class with type <see cref="TInterface"/></returns>
        TInterface CreateProxyForInterface<TInterface>(IMethodProxy methodProxy) where TInterface: class;
    }
}