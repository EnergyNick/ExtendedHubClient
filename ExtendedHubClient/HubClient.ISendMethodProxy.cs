using System;
using ExtendedHubClient.Proxy;
using ExtendedHubClient.Proxy.Interceptors;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace ExtendedHubClient
{
    public class HubClient : HubClient<ISendMethodProxy>
    {
        public override ISendMethodProxy Server => ProxyCreator.CreateProxyForInterface<ISendMethodProxy>(MethodHolder);

        public HubClient(string url,
            Action<HttpConnectionOptions> connectionConfiguration = null,
            Action<IHubConnectionBuilder> additionalHubConfiguration = null,
            ILogger logger = null)
            : base(url,
                connectionConfiguration,
                additionalHubConfiguration,
                logger,
                new CastleProxyCreator<MethodProxyInterceptorWrapper>())
        { }
    }
}