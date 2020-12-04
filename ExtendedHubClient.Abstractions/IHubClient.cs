using System;
using System.Threading;
using System.Threading.Tasks;
using ExtendedHubClient.Abstractions.Methods;

namespace ExtendedHubClient.Abstractions
{
    /// <summary>
    /// Interface of strongly typed SignalR client.
    /// </summary>
    /// <typeparam name="T">The type of client.</typeparam>
    public interface IHubClient<out T> : IAsyncDisposable
        where T: class
    {
        bool IsActive { get; }
        
        T Server { get; }
        IMethodRegistrationManager Methods { get; }
        
        Task<bool> TryOpenConnection(CancellationToken cancellationToken = default);
        Task CloseConnection(CancellationToken cancellationToken = default);
        
        event OnHubReceiveDelegate OnCommandReceive;
    }
}