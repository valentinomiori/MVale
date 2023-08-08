using System;

namespace MVale.Core
{
    public sealed class FuncServiceProviderAdapter : IServiceProvider
    {
        public Func<Type, object> Callback { get; }

        public FuncServiceProviderAdapter(Func<Type, object> callback)
        {
            Callback = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        public object GetService(Type serviceType)
        {
            return this.Callback(serviceType);
        }
    }
}