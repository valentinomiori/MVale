#if NETCOREAPP
using System;

namespace MVale.Core
{
    /// <summary>
    /// An interface that enhances the inherited <see cref="IAsyncDisposable"/>.
    /// </summary>
    public interface IAsyncDisposableObject : IDisposableInfo, IAsyncDisposable
    {
    }
}
#endif