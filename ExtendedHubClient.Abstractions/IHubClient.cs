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
        bool IsActive { get; }
        
        ISendMethodProxy Server { get; }
        
        IMethodsManager Methods { get; }
        
        Task<bool> TryOpenConnection(CancellationToken cancellationToken = default);
        Task CloseConnection(CancellationToken cancellationToken = default);
        
        event OnHubReceiveDelegate OnCommandReceive;
    }
}