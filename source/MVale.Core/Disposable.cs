using System;

namespace MVale.Core
{
    public sealed class Disposable : DisposableBase
    {
        public Action<bool> DisposeAction { get; }

        public Disposable(Action<bool> disposeAction)
        {
            DisposeAction = disposeAction ?? throw new ArgumentNullException(nameof(disposeAction));
        }

        public static Disposable Create(Action<bool> disposeAction)
        {
            return new Disposable(disposeAction);
        }

        public static Disposable Create(Action disposeAction, bool? onDisposing = null)
        {
            if (disposeAction == null)
                throw new ArgumentNullException(nameof(disposeAction));

            return new Disposable(disposing =>
            {
                if (onDisposing == null || onDisposing.Value == disposing)
                {
                    disposeAction.Invoke();
                }
            });
        }

        public static Disposable From(IDisposable disposable, bool? onDisposing = null)
        {
            if (disposable == null)
                return null;

            return new Disposable(disposing =>
            {
                if (onDisposing == null || onDisposing.Value == disposing)
                {
                    disposable.Dispose();
                }
            });
        }

        protected override void InternalDispose(bool disposing)
        {
            this.DisposeAction(disposing);
        }
    }
}