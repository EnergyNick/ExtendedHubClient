using System;
using System.Threading.Tasks;

namespace ExtendedHubClient.Abstractions.Methods
{
    public class MethodView : IEquatable<MethodView>
    {
        public string Name { get; }
        public Type[] Arguments { get; }
        public Type ReturnValue { get; }

        public MethodView(string name, Type[] arguments, Type returnValue = null)
        {
            Name = name;
            Arguments = arguments;
            ReturnValue = returnValue ?? typeof(Task);
        }

        public bool Equals(MethodView other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name && Equals(Arguments, other.Arguments);
        }

        public override bool Equals(object obj) => Equals(obj as MethodView);

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Arguments);
        }
    }
}