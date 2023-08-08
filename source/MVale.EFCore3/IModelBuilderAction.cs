using System;
using Microsoft.EntityFrameworkCore;

namespace MVale.EntityFramework
{
    public interface IModelBuilderAction
    {
        public delegate void Callback(ModelBuilder modelBuilder);

        public sealed class CallbackObject : IModelBuilderAction
        {
            public Callback Callback { get; }

            public CallbackObject(Callback callback)
            {
                this.Callback = callback ?? throw new ArgumentNullException(nameof(callback));
            }

            public void OnModelCreating(ModelBuilder modelBuilder)
                => this.Callback.Invoke(modelBuilder);
        }

// Target runtime may not support default interface implementation
#if NETCOREAPP || NET
        public static IModelBuilderAction From(Callback callback)
            => new CallbackObject(callback);
#endif

        void OnModelCreating(ModelBuilder modelBuilder);
    }
}