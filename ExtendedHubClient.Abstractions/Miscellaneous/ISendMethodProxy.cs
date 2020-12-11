using System.Threading;
using System.Threading.Tasks;

namespace ExtendedHubClient.Abstractions
{
    /// <summary>
    /// Basic interface for interacting with the server that provides access to a generic method
    /// </summary>
    public interface ISendMethodProxy
    {
        public Task SendCoreAsync(string methodName, object[] methodArgs, CancellationToken token = default);
    }
}