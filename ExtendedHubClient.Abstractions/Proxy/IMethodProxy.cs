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
        /// <summary>
        /// Universal realization for method proxying without returning value
        /// </summary>
        /// <param name="name">Method name</param>
        /// <param name="arguments">Arguments for calling method</param>
        public Task OnMethodInvoke(string name, IEnumerable<object> arguments);
        
        /// <summary>
        /// Universal realization for method proxying with returning value <see cref="TResult"/>
        /// </summary>
        /// <param name="name">Method name</param>
        /// <param name="arguments">Arguments for calling method</param>
        /// <typeparam name="TResult">Function return value</typeparam>
        /// <returns>Return <see cref="Task{TResult}"/> of calling server</returns>
        public Task<TResult> OnMethodInvokeWithReturnValue<TResult>(string name, IEnumerable<object> arguments);
    }
}