using System;

namespace MVale.Core
{
    /// <summary>
    /// A delegate representing a typed event handler.
    /// </summary>
    /// <typeparam name="TSender">The type of the sender for this event.</typeparam>
    /// <typeparam name="TEventArgs">The type of the arguments for this event.</typeparam>
    /// <param name="sender">The instance that raised this event.</param>
    /// <param name="e">The arguments this event was raised with.</param>
    public delegate void EventHandler<in TSender, in TEventArgs>(TSender sender, TEventArgs e);
}