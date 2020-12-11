using System.Collections.Generic;

namespace ExtendedHubClient.Abstractions.Methods
{
    /// <summary>
    /// Provides an interface for easy display of registration results and check invoke for any method.
    /// </summary>
    public interface IMethodsManager
    {
        IReadOnlyCollection<MethodView> SendMethods { get; }
        
        IReadOnlyCollection<MethodView> ReceiveMethods { get; }

        /// <summary>
        /// Checks whether any method will be called for the given arguments and name.
        /// </summary>
        /// <param name="type">Method type in registration</param>
        /// <param name="name">Method name</param>
        /// <param name="arguments">Method arguments</param>
        bool CanArgumentsCallMethodFromRegistration(MethodType type, string name, IReadOnlyList<object> arguments);
        
        /// <summary>Returns the method that will be called for the given arguments and method name.</summary>
        /// <param name="type">Method type in registration</param>
        /// <param name="name">Method name</param>
        /// <param name="arguments">Method arguments</param>
        /// <param name="methodView">Method if the search is successful</param>
        /// <returns></returns>
        bool TryGetMethodForMethodArguments(MethodType type, 
            string name, 
            IReadOnlyList<object> arguments,
            out MethodView methodView);
    }
}