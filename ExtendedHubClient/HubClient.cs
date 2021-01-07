using System;
using ExtendedHubClient.Abstractions;
using ExtendedHubClient.Abstractions.Proxy;
using ExtendedHubClient.Methods;
using ExtendedHubClient.Proxy;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace ExtendedHubClient
{
    /// <summary>
    /// Provide default realization of <see cref="IHubClient"/>.
    /// </summary>
    public class HubClient : BaseHubClient
    {
        protected override IProxyCreator ProxyCreator { get; }
        protected override IMethodProxy MethodProxy { get; }

        public HubClient(string url,
            Action<HttpConnectionOptions> connectionConfiguration = null,
            Action<IHubConnectionBuilder> additionalHubConfiguration = null,
            ILogger logger = null
            )
            : base(url,
                (hub, hubMethod) => 
                    new DefaultMethodManager(hub, hubMethod, typeof(ISendMethodProxy), null),
                connectionConfiguration,
                additionalHubConfiguration,
                logger)
        {
            MethodProxy = new UntypedMethodProxy(Hub, Methods);
            ProxyCreator = new DefaultProxyCreator();
        }
    }
}