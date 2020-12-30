using System;
using System.Threading;
using System.Threading.Tasks;
using ExtendedHubClient.Abstractions.Methods;

namespace ExtendedHubClient.Abstractions
{
    /// <summary>
    /// Interface of extended SignalR client.
    /// </summary>
    public interface IHubClient: IAsyncDisposable
    {
        /// <summary>
        /// Shows the server connection status.
        /// </summary>
        bool IsActive { get; }
        
        /// <summary>
        /// Interface for calling server methods.
        /// </summary>
        ISendMethodProxy Server { get; }
        
        IMethodsManager Methods { get; }
        
        Task<bool> TryOpenConnection(CancellationToken cancellationToken = default);
        
        Task CloseConnection(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// The event, which receives the call based on the registered methods.
        /// </summary>
        event OnHubReceiveDelegate OnCommandReceive;
    }
}