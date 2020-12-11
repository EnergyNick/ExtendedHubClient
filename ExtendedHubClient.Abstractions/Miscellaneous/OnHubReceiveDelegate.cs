using System.Threading.Tasks;

namespace ExtendedHubClient.Abstractions
{
    /// <summary>
    /// Represents an abstraction of a method called from the server.
    /// </summary>
    public delegate Task OnHubReceiveDelegate(string methodName, object[] methodArgs);
}