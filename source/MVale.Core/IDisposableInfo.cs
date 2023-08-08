using System;

namespace MVale.Core
{
    /// <summary>
    /// Provides informations on the current disposable state.
    /// </summary>
    public interface IDisposableInfo
    {
        /// <summary>
        /// <see langword="true"/> if the current object has been disposed, <see langword="false"/> otherwise.
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Raised when the current object has been disposed.
        /// </summary>
        event EventHandler<IDisposableInfo> Disposed;
    }
}