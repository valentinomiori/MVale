using System;

namespace MVale.Core
{
    /// <summary>
    /// A base class to implement the disposable pattern.
    /// </summary>
    public abstract class DisposableBase : IDisposableObject, IDisposable
    {
        public bool IsDisposed { get; private protected set; }

        public event EventHandler<IDisposableInfo> Disposed;

        ~DisposableBase()
        {
            this.Dispose(disposing: false);
        }

        private void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                this.InternalDispose(disposing);
                this.IsDisposed = true;
                this.RaiseDisposed();
            }
        }

        private protected void RaiseDisposed()
        {
            this.Disposed?.Invoke(this);
        }

        /// <summary>
        /// <para>
        /// This method is called when the current object is being disposed.
        /// </para>
        /// <ul>
        /// <li>
        /// If <paramref name="disposing"/> is <see langword="true"/>: dispose managed state (managed objects).
        /// </li>
        /// <li>
        /// If <paramref name="disposing"/> is <see langword="false"/>: free unmanaged resources (unmanaged objects) and override finalizer,
        /// set large fields to null.
        /// </li>
        /// </ul>
        /// <para>
        /// Does nothing if not overridden.
        /// </para>
        /// </summary>
        /// <remarks> Always call the <see langword="base"/> method. </remarks>
        protected abstract void InternalDispose(bool disposing);

        /// <summary>
        /// Throws a <see cref="ObjectDisposedException"/> in case the current object is disposed,
        /// use the <see cref="IsDisposed"/> property to obtain the actual state.
        /// </summary>
        /// <exception cref="ObjectDisposedException"> If this instance is disposed.</exception>
        protected void ThrowIfDisposed()
        {
            if (this.IsDisposed)
                throw new ObjectDisposedException($"{this.GetType()}");
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
