using System;
using System.Threading;
using System.Threading.Tasks;
using ExtendedHubClient.Abstractions;
using ExtendedHubClient.Abstractions.Methods;
using ExtendedHubClient.Abstractions.Proxy;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ExtendedHubClient
{
    public abstract class BaseHubClient : IHubClient
    {
        public bool IsActive { get; private set; }

        public ISendMethodProxy Server => ProxyCreator.CreateProxyForInterface<ISendMethodProxy>(MethodProxy);
        public IMethodsManager Methods { get; }

        protected abstract IProxyCreator ProxyCreator { get; }
        protected abstract IMethodProxy MethodProxy { get; }
        
        protected readonly ILogger Logger;
        protected readonly HubConnection Hub;

        public event OnHubReceiveDelegate OnCommandReceive;

        protected BaseHubClient(string url,
            Func<HubConnection, OnHubReceiveDelegate, IMethodsManager> methodRegistration,
            Action<HttpConnectionOptions> connectionConfiguration = null,
            Action<IHubConnectionBuilder> additionalHubConfiguration = null, 
            ILogger logger = null
        )
        {
            Logger = logger ?? NullLogger.Instance;

            Hub = CreateAndConfigureHub(url, connectionConfiguration, additionalHubConfiguration);
            Methods = methodRegistration(Hub, CommandReceived);
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
        
        protected virtual async Task CommandReceived(string methodName, object[] methodArgs)
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