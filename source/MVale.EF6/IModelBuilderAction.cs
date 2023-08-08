using System;
using System.Data.Entity;

namespace MVale.EntityFramework
{
    public interface IModelBuilderAction
    {
        public delegate void Callback(DbModelBuilder modelBuilder);

        public sealed class CallbackObject : IModelBuilderAction
        {
            public Callback Callback { get; }

            public CallbackObject(Callback callback)
            {
                this.Callback = callback ?? throw new ArgumentNullException(nameof(callback));
            }

            public void OnModelCreating(DbModelBuilder modelBuilder)
                => this.Callback.Invoke(modelBuilder);
        }
        
        void OnModelCreating(DbModelBuilder modelBuilder);
    }
}