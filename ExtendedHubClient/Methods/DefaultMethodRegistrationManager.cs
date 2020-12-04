using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ExtendedHubClient.Abstractions;
using ExtendedHubClient.Abstractions.Methods;
using Microsoft.AspNetCore.SignalR.Client;

namespace ExtendedHubClient.Methods
{
    public class MethodRegistrationManager : IMethodRegistrationManager
    {
        private readonly HubConnection _hubConnection;
        private readonly OnHubReceiveDelegate _onHubReceiveMethod;

        public MethodRegistrationManager(
            HubConnection hubConnection,
            OnHubReceiveDelegate onHubReceiveMethod
            )
        {
            _hubConnection = hubConnection;
            _onHubReceiveMethod = onHubReceiveMethod;
            
            _sendMethods = new ConcurrentDictionary<string, MethodView>();
            _receiveMethods = new ConcurrentDictionary<string, MethodView>();
        }

        private readonly ConcurrentDictionary<string, MethodView> _sendMethods;
        public IReadOnlyDictionary<string, MethodView> SendMethods => _sendMethods;

        private readonly ConcurrentDictionary<string, MethodView> _receiveMethods;
        public IReadOnlyDictionary<string, MethodView> ReceiveMethods => _receiveMethods;
        
        
        public void RegisterMethods(MethodType type, Type methodsInterfaceType)
        {
            foreach (var method in methodsInterfaceType.GetMethods())
            {
                VerifyMethod(type, methodsInterfaceType, method);

                var arguments = method.GetParameters().Select(x => x.ParameterType).ToArray();
                var methodView = new MethodView(method.Name, arguments, method.ReturnType);
                RegisterMethod(type, methodView);
            }
        }

        public void RegisterMethods(MethodType type, params MethodViewDto[] methods)
        {
            foreach (var method in methods)
            {
                VerifyMethodView(type, new MethodView(method));
                RegisterMethod(type, new MethodView(method));
            }
        }

        protected void RegisterMethod(MethodType type, MethodView methodView)
        {
            var name = methodView.Name;
            switch (type)
            {
                case MethodType.Send:
                {
                    _sendMethods.TryAdd(name, methodView);
                    break;
                }
                case MethodType.Receive:
                {
                    _receiveMethods.TryAdd(name, methodView);
                    _hubConnection.On(name, methodView.Arguments, response => _onHubReceiveMethod(name, response));
                    break;
                }
            }
        }

        public bool IsMethodContainsInRegistration(string methodName, 
            IReadOnlyList<object> methodArgs, 
            MethodType type,
            out string errorReason)
        {
            var methodProvider = type == MethodType.Receive ? _receiveMethods : _sendMethods;
            if (!methodProvider.TryGetValue(methodName, out var view))
            {
                errorReason = $"Can't find registered method \"{methodName}\"";
                return true;
            }

            if (view.Arguments.Length != methodArgs.Count)
            {
                errorReason = $"Mismatch in the number of input arguments for the method \"{methodName}\"";
                return true;
            }
            
            for (var i = 0; i < methodArgs.Count; i++)
            {
                var argType = view.Arguments[i];
                var dtoType = methodArgs[i].GetType();

                if (dtoType == argType) continue;
                
                errorReason = $"Type mismatch on {i + 1} argument when try to call method \"{methodName}\"";
                return true;
            }

            errorReason = null;
            return false;
        }

        private static void VerifyMethodView(MethodType type, MethodView methodView)
        {
            if(methodView.Arguments == null || methodView.Name == null || methodView.ReturnValue == null)
                throw new ArgumentNullException($"Can't register method with null fields");
            
            if (type == MethodType.Receive && methodView.ReturnValue != typeof(Task))
            {
                throw new InvalidOperationException(
                    $"Cannot generate proxy implementation for '{methodView.Name}'. " +
                    $"All receive methods must return '{typeof(Task).FullName}'.");
            }

            foreach (var parameter in methodView.Arguments)
            {
                if (parameter.IsByRef)
                {
                    throw new InvalidOperationException(
                        $"Cannot generate proxy implementation for method'{methodView.Name} with parameter {parameter.Name}'. " +
                        "Methods must not have 'ref' parameters.");
                }
            }
        }
        
        private static void VerifyMethod(MethodType type, Type interfaceType, MethodInfo interfaceMethod)
        {
            if (type == MethodType.Receive && interfaceMethod.ReturnType != typeof(Task))
            {
                throw new InvalidOperationException(
                    $"Cannot generate proxy implementation for '{interfaceType.FullName}.{interfaceMethod.Name}'. " +
                    $"All receive methods must return '{typeof(Task).FullName}'.");
            }

            foreach (var parameter in interfaceMethod.GetParameters())
            {
                if (parameter.IsOut)
                {
                    throw new InvalidOperationException(
                        $"Cannot generate proxy implementation for '{interfaceType.FullName}.{interfaceMethod.Name}'. " +
                        "Methods must not have 'out' parameters.");
                }

                if (parameter.ParameterType.IsByRef)
                {
                    throw new InvalidOperationException(
                        $"Cannot generate proxy implementation for '{interfaceType.FullName}.{interfaceMethod.Name}'. " +
                        "Methods must not have 'ref' parameters.");
                }
            }
        }
    }
}