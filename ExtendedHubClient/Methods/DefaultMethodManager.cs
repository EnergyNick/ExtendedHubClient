using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Castle.DynamicProxy.Internal;
using ExtendedHubClient.Abstractions;
using ExtendedHubClient.Abstractions.Methods;
using Microsoft.AspNetCore.SignalR.Client;

namespace ExtendedHubClient.Methods
{
    /// <summary>
    /// Default realization of <see cref="IMethodsManager"/>.
    /// Can register methods based on interfaces.
    /// </summary>
    public class DefaultMethodManager : IMethodsManager
    {
        private readonly HubConnection _hubConnection;
        private readonly OnHubReceiveDelegate _onHubReceiveMethod;
        
        private readonly List<MethodView> _sendMethods;
        public IReadOnlyCollection<MethodView> SendMethods => _sendMethods;

        private readonly List<MethodView> _receiveMethods;
        public IReadOnlyCollection<MethodView> ReceiveMethods => _receiveMethods;

        public DefaultMethodManager(HubConnection hubConnection, 
            OnHubReceiveDelegate onHubReceiveMethod,
            Type sendInterface, 
            Type receiveInterface)
        {
            _hubConnection = hubConnection;
            _onHubReceiveMethod = onHubReceiveMethod;
            
            _sendMethods = new List<MethodView>();
            _receiveMethods = new List<MethodView>();
            
            RegisterMethods(MethodType.Receive, receiveInterface);
            RegisterMethods(MethodType.Send, sendInterface);
        }

        public bool CanArgumentsCallMethodFromRegistration(MethodType type, string name, IReadOnlyList<object> arguments)
        {
            return GetEqualMethod(type, name, arguments) != null;
        }

        public bool TryGetMethodForMethodArguments(MethodType type, 
            string name, 
            IReadOnlyList<object> arguments,
            out MethodView methodView)
        {
            methodView = GetEqualMethod(type, name, arguments);
            return methodView != null;
        }

        protected virtual bool IsArgumentEqualToType(Type methodType, object argument)
        {
            return argument == null
                ? methodType.IsClass || methodType.IsNullableType()
                : methodType == argument.GetType();
        }

        protected virtual void VerifyMethod(MethodType type, Type interfaceType, MethodInfo interfaceMethod)
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

        protected MethodView GetEqualMethod(MethodType type, string name, IReadOnlyList<object> arguments)
        {
            var methodProvider = type == MethodType.Receive ? _receiveMethods : _sendMethods;
            var equalNameMethods = methodProvider.FindAll(x => x.Name == name);
            if (equalNameMethods.IsNullOrEmpty())
                return null;

            var equalMethodView = equalNameMethods
                .Where(method => method.Arguments.Length == arguments.Count)
                .Where(method =>
                {
                    for (var i = 0; i < arguments.Count; i++)
                        if (IsArgumentEqualToType(method.Arguments[i], arguments[i]))
                            return false;

                    return true;
                })
                .FirstOrDefault();
            return equalMethodView;
        }

        private void RegisterMethods(MethodType type, Type methodsInterfaceType)
        {
            if (methodsInterfaceType == null) return;
            
            foreach (var method in methodsInterfaceType.GetMethods())
            {
                VerifyMethod(type, methodsInterfaceType, method);

                var arguments = method.GetParameters().Select(x => x.ParameterType).ToArray();
                var methodView = new MethodView(method.Name, arguments, method.ReturnType);
                RegisterMethod(type, methodView);
            }
        }

        private void RegisterMethod(MethodType type, MethodView methodView)
        {
            switch (type)
            {
                case MethodType.Send:
                {
                    _sendMethods.Add(methodView);
                    break;
                }
                case MethodType.Receive:
                {
                    _receiveMethods.Add(methodView);
                    _hubConnection.On(methodView.Name, methodView.Arguments, response => _onHubReceiveMethod(methodView.Name, response));
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}