using System;
using System.Threading;
using System.Threading.Tasks;
using ExtendedHubClient.Abstractions;
using ExtendedHubClient.Abstractions.Methods;
using ExtendedHubClient.Abstractions.Proxy;
using ExtendedHubClient.Methods;
using ExtendedHubClient.Proxy;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ExtendedHubClient
{
    /// <summary>
    /// A base class for a strongly typed SignalR client.
    /// </summary>
    /// <typeparam name="T">The type of client.</typeparam>
    public class HubClient<T> : IHubClient<T>
        where T: class
    {
        public bool IsActive { get; private set; }

        public virtual T Server => ProxyCreator.CreateProxyForInterface<T>(MethodHolder);
        public IMethodRegistrationManager Methods { get; }

        protected readonly ILogger Logger;
        protected readonly HubConnection Hub;
        protected readonly IProxyCreator ProxyCreator;
        protected readonly IMethodHolder MethodHolder;

        public HubClient(string url,
            Action<HttpConnectionOptions> connectionConfiguration = null,
            Action<IHubConnectionBuilder> additionalHubConfiguration = null, 
            ILogger logger = null, 
            IProxyCreator proxyCreator = null
            )
        {
            Logger = logger ?? NullLogger.Instance;
            ProxyCreator = proxyCreator ?? new DefaultProxyCreator();
            Methods = new MethodRegistrationManager(Hub, CommandReceived); 
            
            MethodHolder = new DefaultMethodHolder(Hub, Methods);

            Hub = CreateAndConfigureHub(url, connectionConfiguration, additionalHubConfiguration);
        }

        public async Task<bool> TryOpenConnection(CancellationToken cancellationToken = default)
        {
            var currentState = Hub.State;
            switch (currentState)
            {
                case HubConnectionState.Connecting:
                case HubConnectionState.Reconnecting:
                {
                    await Task.Delay(Hub.ServerTimeout, cancellationToken).ConfigureAwait(false);
                    break;
                }
                case HubConnectionState.Disconnected:
                    try
                    {
                        Logger.LogInformation($"Start attempt to create connection with hub");
                        await Hub.StartAsync(cancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        ChangeManagerState(false, "Can't start connection with Hub", e);
                    }
                    break;
            }

            if (Hub.State == HubConnectionState.Connected && currentState != Hub.State)
                ChangeManagerState(true,
                    $"Manager successfully connected. The ConnectionId is now: {Hub.ConnectionId}");

            return IsActive;
        }

        public async Task CloseConnection(CancellationToken cancellationToken = default)
        {
            ChangeManagerState(false, $"Manager close connection with Hub");
            await Hub.StopAsync(cancellationToken).ConfigureAwait(false);
        }

        public event OnHubReceiveDelegate OnCommandReceive;

        public virtual async ValueTask DisposeAsync()
        {
            if(Hub.State != HubConnectionState.Disconnected)
                await CloseConnection().ConfigureAwait(false);
            
            await Hub.DisposeAsync().ConfigureAwait(false);
        }

        protected void ChangeManagerState(bool isActive, string reason, Exception exception = null)
        {
            if (exception == null)
                Logger.LogInformation(reason);
            else
                Logger.LogWarning(exception, reason);
            
            IsActive = isActive;
        }

        private async Task CommandReceived(string methodName, object[] methodArgs)
        {
            if (OnCommandReceive != null) 
                await OnCommandReceive.Invoke(methodName, methodArgs);
        }

        private HubConnection CreateAndConfigureHub(string hubUrl,
            Action<HttpConnectionOptions> connectionConfiguration,
            Action<IHubConnectionBuilder> additionalHubConfiguration)
        {
            var builder = new HubConnectionBuilder()
                .WithUrl(new Uri(hubUrl), connectionConfiguration);
            additionalHubConfiguration?.Invoke(builder);

            var hub = builder.Build();
            
            hub.Reconnecting += exception =>
            {
                ChangeManagerState(false, "Hub client try reconnect to Hub", exception);
                return Task.CompletedTask;
            };

            hub.Reconnected += connectionId =>
            {
                ChangeManagerState(true, $"Hub client successfully reconnected. The ConnectionId is now: {connectionId}");
                return Task.CompletedTask;
            };

            hub.Closed += exception =>
            {
                if (exception != null)
                    ChangeManagerState(false, "Hub client close connection with Hub", exception);
                return Task.CompletedTask;
            };

            return hub;
        }
    }
}