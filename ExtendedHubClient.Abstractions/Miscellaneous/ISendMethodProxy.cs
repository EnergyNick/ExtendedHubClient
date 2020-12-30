using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExtendedHubClient.Abstractions
{
    /// <summary>
    /// Basic interface for interacting with the server that provides access to a generic method
    /// </summary>
    public interface ISendMethodProxy
    {
        /// <summary>
        /// Call method in server without return value
        /// </summary>
        public Task SendCoreAsync(string methodName, object[] methodArgs, CancellationToken token = default);

        /// <summary>
        /// Call method in server with return value
        /// </summary>
        public Task InvokeCoreAsync(string methodName, object[] methodArgs, Type returnType,
            CancellationToken token = default);
    }
}