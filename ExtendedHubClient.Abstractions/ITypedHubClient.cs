namespace ExtendedHubClient.Abstractions
{
    /// <summary>
    /// Interface of strongly typed SignalR client.
    /// </summary>
    /// <typeparam name="TServerMethods">The type of client.</typeparam>
    public interface ITypedHubClient<out TServerMethods> : IHubClient
        where TServerMethods: class
    {
        /// <summary>
        /// Interface for calling server methods.
        /// </summary>
        new TServerMethods Server { get; }
    }
}