using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace ExtendedHubClient.Abstractions.Proxy
{
    /// <summary>
    /// Represents a simplification of the called method for sending a message via <see cref="HubConnection"/>
    /// </summary>
    public interface IMethodProxy
    {
        public Task OnMethodInvoke(string name, IEnumerable<object> arguments);
        public Task<object> OnMethodInvokeWithReturnValue(string name, IEnumerable<object> arguments, Type returnType);
    }
}