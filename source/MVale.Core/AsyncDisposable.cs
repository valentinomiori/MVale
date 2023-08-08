#if NETCOREAPP
using System;
using System.Threading.Tasks;

namespace MVale.Core
{
    public sealed class AsyncDisposable : AsyncDisposableBase
    {
        public Func<ValueTask> AsyncDisposeFunc { get; }

        public Action<bool> DisposeAction { get; }

        public AsyncDisposable(Func<ValueTask> asyncDisposeFunc) : this(asyncDisposeFunc, null)
        {
        }

        public AsyncDisposable(Func<ValueTask> asyncDisposeFunc, Action<bool> disposeAction)
        {
            this.AsyncDisposeFunc = asyncDisposeFunc ?? throw new ArgumentNullException(nameof(asyncDisposeFunc));
            this.DisposeAction = disposeAction;
        }

        public static AsyncDisposable Create(Func<Task> asyncDisposeFunc)
        {
            if (asyncDisposeFunc == null)
                throw new ArgumentNullException(nameof(asyncDisposeFunc));

            return Create(asyncDisposeFunc, null);
        }

        public static AsyncDisposable Create(Func<Task> asyncDisposeFunc, Action<bool> disposeAction)
        {
            if (asyncDisposeFunc == null)
                throw new ArgumentNullException(nameof(asyncDisposeFunc));

            return new AsyncDisposable(
                async () => await asyncDisposeFunc(),
                disposeAction);
        }

        public static AsyncDisposable Create(Func<Task> asyncDisposeFunc, Action disposeAction, bool? onDisposing = null)
        {
            if (disposeAction == null)
                throw new ArgumentNullException(nameof(disposeAction));

            return Create(asyncDisposeFunc, disposing =>
            {
                if (onDisposing == null || onDisposing.Value == disposing)
                {
                    disposeAction.Invoke();
                }
            });
        }

        public static AsyncDisposable From(IAsyncDisposable asyncDisposable)
        {
            if (asyncDisposable == null)
                return null;

            if (asyncDisposable is IDisposable disposable)
                return new AsyncDisposable(
                    asyncDisposable.DisposeAsync,
                    disposing =>
                    {
                        if (disposing)
                        {
                            disposable.Dispose();
                        }
                    });

            return new AsyncDisposable(asyncDisposable.DisposeAsync, null);
        }

        protected override ValueTask InternalDisposeAsync()
            => this.AsyncDisposeFunc.Invoke();

        protected override void InternalDispose(bool disposing)
            => this.DisposeAction?.Invoke(disposing);
    }
}
#endif