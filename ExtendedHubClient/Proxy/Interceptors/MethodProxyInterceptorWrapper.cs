﻿using System;
using Castle.DynamicProxy;
using ExtendedHubClient.Abstractions.Proxy;

namespace ExtendedHubClient.Proxy.Interceptors
{
    internal class MethodProxyInterceptorWrapper : IInterceptorWrapper
    {
        private IMethodHolder _holder;

        public void Intercept(IInvocation invocation)
        {
            if(invocation == null)
                throw new ArgumentNullException(nameof(invocation));

            if (invocation.Arguments.Length != 2
                || invocation.Arguments[0] is string
                || invocation.Arguments[1] is object[])
                throw new InvalidOperationException(
                    $"{nameof(MethodProxyInterceptorWrapper)} can only work with {nameof(ISendMethodProxy)}");
            
            lock (_holder)
            {
                var name = invocation.Arguments[0] as string;
                var arguments = invocation.Arguments[1] as object[];
                _holder.OnMethodInvoke(name, arguments).GetAwaiter().GetResult();
            }
        }

        public void AttachMethodHolder(IMethodHolder holder)
        {
            lock (_holder)
            {
                _holder = holder ?? throw new ArgumentNullException(nameof(holder));
            }
        }
    }
}