using System;

namespace ExtendedHubClient.Abstractions.Methods
{
    public class MethodViewDto
    {
        public string Name { get; set; }
        public Type[] Arguments { get; set; }
        public Type ReturnValue { get; set; }
    }
}