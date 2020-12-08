using System.Collections.Generic;

namespace ExtendedHubClient.Abstractions.Methods
{
    /// <summary>
    /// Provides an interface for easy display of registration results and checking for any method.
    /// </summary>
    public interface IMethodsManager
    {
        IReadOnlyCollection<MethodView> SendMethods { get; }
        
        IReadOnlyCollection<MethodView> ReceiveMethods { get; }

        /// <summary>
        /// Allows you to find out if a similar method exists in registration.
        /// </summary>
        /// <param name="type">Method type in registration</param>
        /// <param name="name">Method name</param>
        /// <param name="arguments">Method arguments</param>
        /// <param name="errorReason">Reason for the negative result</param>
        bool IsMethodContainsInRegistration(MethodType type, 
            string name, 
            IReadOnlyList<object> arguments,
            out string errorReason);
        
        
        /// <param name="type">Method type in registration</param>
        /// <param name="name">Method name</param>
        /// <param name="arguments">Method arguments</param>
        /// <param name="methodView">Method if the search is successful</param>
        /// <returns></returns>
        bool TryGetMethodFromRegistration(MethodType type, 
            string name, 
            IReadOnlyList<object> arguments,
            out MethodView methodView);
    }
}