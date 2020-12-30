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
        /// Universal realization for method proxying with returning value <see cref="Task"/>
        /// </summary>
        /// <param name="name">Method name</param>
        /// <param name="arguments">Arguments for calling method</param>
        public Task OnMethodInvoke(string name, IEnumerable<object> arguments);
        
        /// <summary>
        /// Universal realization for method proxying with returning value <see cref="Task{TResult}"/>
        /// </summary>
        /// <param name="name">Method name</param>
        /// <param name="arguments">Arguments for calling method</param>
        /// <param name="returnType">Type of returning TResult from method</param>
        /// <returns>Return <see cref="Task{TResult}"/> with a generic object with the real type <see cref="returnType"/></returns>
        public Task<object> OnMethodInvokeWithReturnValue(string name, IEnumerable<object> arguments, Type returnType);
    }
}