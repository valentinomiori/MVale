using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MVale.Core.Utils.Test
{
    [TestOf(typeof(TaskUtil))]
    public class TaskUtil_Test
    {
        private async Task<T> CreateDelayedTask<T>(T value)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(50));
            return value;
        }

        private ImplicitConvertible<T> CreateImplicitlyConvertible<T>(T value) => new ImplicitConvertible<T>() { Value = value };
        private ExplicitConvertible<T> CreateExplicitlyConvertible<T>(T value) => new ExplicitConvertible<T>() { Value = value };

        [Test]
        public async Task Test()
        {
            Assert.AreEqual(TaskUtil.GetResult<object>(null), null);

            Assert.AreEqual(await TaskUtil.GetResult<object>(Task.FromResult(12)), 12);
            Assert.AreEqual(await TaskUtil.GetResult<int>(Task.FromResult<object>(12)), 12);

            Assert.AreEqual(await TaskUtil.GetResult<long>(Task.FromResult(12)), 12);
            Assert.AreEqual(await TaskUtil.GetResult<long>(Task.FromResult(this.CreateImplicitlyConvertible(12L))), 12);
            Assert.AreEqual(await TaskUtil.GetResult<long>(Task.FromResult(this.CreateExplicitlyConvertible(12L))), 12);
            Assert.AreEqual(await TaskUtil.GetResult<long>(this.CreateDelayedTask(12)), 12);
            Assert.AreEqual(await TaskUtil.GetResult<long>(this.CreateDelayedTask(this.CreateImplicitlyConvertible(12L))), 12);
            Assert.AreEqual(await TaskUtil.GetResult<long>(this.CreateDelayedTask(this.CreateExplicitlyConvertible(12L))), 12);

            Assert.ThrowsAsync<InvalidOperationException>(() => TaskUtil.GetResult<string>(Task.FromResult(12)));
            Assert.ThrowsAsync<InvalidOperationException>(() => TaskUtil.GetResult<string>(this.CreateDelayedTask(12)));

            Assert.ThrowsAsync<InvalidCastException>(() => TaskUtil.GetResult<ValueType>(Task.FromResult<object>("T")));
        }
    }
}