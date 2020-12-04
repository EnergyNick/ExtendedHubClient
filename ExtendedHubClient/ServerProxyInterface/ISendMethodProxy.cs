using System.Threading.Tasks;

namespace ExtendedHubClient
{
    public interface ISendMethodProxy
    {
        public Task SendCoreAsync(string methodName, object[] methodArgs);
    }
}