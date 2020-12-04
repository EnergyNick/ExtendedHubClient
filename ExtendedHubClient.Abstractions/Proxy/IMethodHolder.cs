using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExtendedHubClient.Abstractions.Proxy
{
    public interface IMethodHolder
    {
        public Task OnMethodInvoke(string name, IEnumerable<object> arguments);
    }
}