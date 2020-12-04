using System.Threading.Tasks;

namespace ExtendedHubClient.Abstractions
{
    public delegate Task OnHubReceiveDelegate(string methodName, object[] methodArgs);
}