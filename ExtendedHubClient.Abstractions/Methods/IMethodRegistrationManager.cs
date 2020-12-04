using System;
using System.Collections.Generic;

namespace ExtendedHubClient.Abstractions.Methods
{
    public interface IMethodRegistrationManager
    {
        IReadOnlyDictionary<string, MethodView> SendMethods { get; }
        IReadOnlyDictionary<string, MethodView> ReceiveMethods { get; }
        
        void RegisterMethods(MethodType type, Type methodsInterfaceType);
        void RegisterMethods(MethodType type, params MethodViewDto[] methods);

        bool IsMethodContainsInRegistration(string methodName, 
            IReadOnlyList<object> methodArgs, 
            MethodType type,
            out string errorReason);
    }
}