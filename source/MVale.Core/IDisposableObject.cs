using System;

namespace MVale.Core
{
    /// <summary>
    /// An interface that enhances the inherited <see cref="IDisposable"/>.
    /// </summary>
    public interface IDisposableObject : IDisposableInfo, IDisposable
    {
    }
}