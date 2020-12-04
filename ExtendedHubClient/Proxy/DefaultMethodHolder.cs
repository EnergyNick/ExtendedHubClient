using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtendedHubClient.Abstractions.Methods;
using ExtendedHubClient.Abstractions.Proxy;
using Microsoft.AspNetCore.SignalR.Client;

namespace ExtendedHubClient.Proxy
{
    public class DefaultMethodHolder : IMethodHolder
    {
        private readonly HubConnection _hub;
        private readonly IMethodRegistrationManager _manager;

        public DefaultMethodHolder(HubConnection hub, IMethodRegistrationManager manager)
        {
            _hub = hub;
            _manager = manager;
        }

        public async Task OnMethodInvoke(string name, IEnumerable<object> arguments)
        {
            var args = arguments?.ToArray() ?? new object[0];
        
            if(!_manager.IsMethodContainsInRegistration(name, args, MethodType.Send, out var reason))
                throw new ArgumentException(reason);

            await _hub.SendCoreAsync(name, args).ConfigureAwait(false);
        }
    }
}