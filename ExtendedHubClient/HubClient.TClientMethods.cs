using System;
using Castle.Core.Internal;
using ExtendedHubClient.Abstractions;
using ExtendedHubClient.Abstractions.Methods;
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
    /// Client methods from <see cref="TClientMethods"/> can only return a Task.
    /// </summary>
    /// <typeparam name="TClientMethods">Interface that provides a view similar to the client's methods</typeparam>
    public class HubClient<TClientMethods> : BaseHubClient
        where TClientMethods: class
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
                    new DefaultMethodManager(hub, hubMethod, typeof(ISendMethodProxy), typeof(TClientMethods)),
                connectionConfiguration,
                additionalHubConfiguration,
                logger)
        {
            MethodProxy = new DefaultMethodProxy(Hub, Methods);
            ProxyCreator = new DefaultTypedProxyCreator();
        }
    }
}