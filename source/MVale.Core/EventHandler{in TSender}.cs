using System;

namespace MVale.Core
{
    /// <inheritdoc cref="EventHandler{TSender, TEventArgs}"/>
    public delegate void EventHandler<in TSender>(TSender sender);
}