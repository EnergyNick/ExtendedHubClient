using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtendedHubClient.Abstractions.Methods;
using ExtendedHubClient.Abstractions.Proxy;
using Microsoft.AspNetCore.SignalR.Client;

namespace ExtendedHubClient.Proxy
{
    public class TypedMethodProxy : IMethodProxy
    {
        private readonly HubConnection _hub;
        private readonly IMethodsManager _manager;

        public TypedMethodProxy(HubConnection hub, IMethodsManager manager)
        {
            _hub = hub;
            _manager = manager;
        }

        public async Task OnMethodInvoke(string name, IEnumerable<object> arguments)
        {
            var args = arguments?.ToArray() ?? new object[0];

            if (!_manager.CanArgumentsCallMethodFromRegistration(MethodType.Send, name, args))
                throw new ArgumentException(
                    $"Can't call method {name} with arguments: {string.Join(", ", args.Select(x => x?.ToString()))}");

            await _hub.SendCoreAsync(name, args).ConfigureAwait(false);
        }
        
        public async Task<object> OnMethodInvokeWithReturnValue(string name, IEnumerable<object> arguments, Type returnType)
        {
            var args = arguments?.ToArray() ?? new object[0];

            if (!_manager.CanArgumentsCallMethodFromRegistration(MethodType.Send, name, args))
                throw new ArgumentException(
                    $"Can't call method {name} with arguments: {string.Join(", ", args.Select(x => x?.ToString()))}");

            return await _hub.InvokeCoreAsync(name, returnType, args).ConfigureAwait(false);
        }
    }
}