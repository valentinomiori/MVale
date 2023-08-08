#if NETCOREAPP
using System;
using System.Threading.Tasks;

namespace MVale.Core
{
    /// <summary>
    /// A base class to implement the asynchronous disposable pattern.
    /// </summary>
    public abstract class AsyncDisposableBase :
        DisposableBase,
        IAsyncDisposableObject,
        IDisposableInfo,
        IAsyncDisposable,
        IDisposableObject,
        IDisposable
    {
        /// <summary>
        /// <para>
        /// This method is called when the current object is being asynchronously disposed.
        /// </para>
        /// <para>
        /// Does nothing if not overridden.
        /// </para>
        /// </summary>
        /// <returns>A value task representing the asynchronous operation.</returns>
        /// <remarks> Always call the <see langword="base"/> method. </remarks>
        protected abstract ValueTask InternalDisposeAsync();

        public async ValueTask DisposeAsync()
        {
            if (!this.IsDisposed)
            {
                await this.InternalDisposeAsync().ConfigureAwait(false);
                this.InternalDispose(disposing: false);
                this.IsDisposed = true;
                this.RaiseDisposed();
            }

            GC.SuppressFinalize(this);
        }
    }
}
#endif