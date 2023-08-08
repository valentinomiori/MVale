using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MVale.Core.Test
{
    [TestFixture(TestOf = typeof(AsyncDisposable))]
    public class AsyncDisposable_Test
    {
        [Test]
        public void TestDisposeAsync()
        {
            bool wasDisposed = false;
            bool wasDisposedAsync = false;
            bool? disposingValue = null;
            bool wasDisposedRaised = false;

            var subject = new AsyncDisposable(
                async () =>
                {
                    Assert.AreEqual(false, wasDisposed);
                    Assert.AreEqual(false, wasDisposedAsync);
                    Assert.AreEqual(null, disposingValue);
                    Assert.AreEqual(false, wasDisposedRaised);
                    await Task.Delay(TimeSpan.FromSeconds(2));
                    wasDisposedAsync = true;
                },
                disposing =>
                {
                    Assert.AreEqual(false, wasDisposed);
                    Assert.AreEqual(true, wasDisposedAsync);
                    Assert.AreEqual(null, disposingValue);
                    Assert.AreEqual(false, wasDisposedRaised);
                    wasDisposed = true;
                    disposingValue = disposing;
                });

            subject.Disposed += (d) =>
            {
                wasDisposedRaised = true;
            };

            Assert.AreEqual(false, wasDisposed);
            Assert.AreEqual(false, wasDisposedAsync);
            Assert.AreEqual(null, disposingValue);
            Assert.AreEqual(false, wasDisposedRaised);
            Assert.AreEqual(false, subject.IsDisposed);

            Assert.DoesNotThrowAsync(async () =>
            {
                await using var disposable = subject;
            });

            Assert.AreEqual(true, wasDisposed);
            Assert.AreEqual(true, wasDisposedAsync);
            Assert.AreEqual(false, disposingValue);
            Assert.AreEqual(true, wasDisposedRaised);
            Assert.AreEqual(true, subject.IsDisposed);

            wasDisposed = false;
            wasDisposedAsync = false;
            disposingValue = null;
            wasDisposedRaised = false;

            Assert.DoesNotThrow(() => subject.Dispose());

            Assert.AreEqual(false, wasDisposed);
            Assert.AreEqual(false, wasDisposedAsync);
            Assert.AreEqual(null, disposingValue);
            Assert.AreEqual(false, wasDisposedRaised);
            Assert.AreEqual(true, subject.IsDisposed);
        }

        [Test]
        public void TestDisposeAsyncThrow()
        {
            bool wasDisposed = false;
            bool wasDisposedAsync = false;
            bool? disposingValue = null;
            bool wasDisposedRaised = false;

            AsyncDisposable subject = null;
            subject = new AsyncDisposable(
                async () =>
                {
                    Assert.AreEqual(false, wasDisposed);
                    Assert.AreEqual(false, wasDisposedAsync);
                    Assert.AreEqual(null, disposingValue);
                    Assert.AreEqual(false, wasDisposedRaised);
                    await Task.Delay(TimeSpan.FromSeconds(2));
                    throw new TestException();
                },
                disposing =>
                {
                    if (subject != null)
                    {
                        Assert.Fail();
                    }
                });

            subject.Disposed += (d) =>
            {
                wasDisposedRaised = true;
            };

            Assert.AreEqual(false, wasDisposed);
            Assert.AreEqual(false, wasDisposedAsync);
            Assert.AreEqual(null, disposingValue);
            Assert.AreEqual(false, wasDisposedRaised);
            Assert.AreEqual(false, subject.IsDisposed);

            Assert.ThrowsAsync<TestException>(async () =>
            {
                await subject.DisposeAsync();
            });

            Assert.AreEqual(false, wasDisposed);
            Assert.AreEqual(false, wasDisposedAsync);
            Assert.AreEqual(null, disposingValue);
            Assert.AreEqual(false, wasDisposedRaised);
            Assert.AreEqual(false, subject.IsDisposed);

            subject = null;
        }

        [Test]
        public void TestDisposeThrow()
        {
            bool wasDisposed = false;
            bool wasDisposedAsync = false;
            bool? disposingValue = null;
            bool wasDisposedRaised = false;

            var subject = new AsyncDisposable(
                () => 
                {
                    Assert.Fail();
                    return new ValueTask(Task.CompletedTask);
                },
                disposing =>
                {
                    Assert.AreEqual(false, wasDisposed);
                    Assert.AreEqual(false, wasDisposedAsync);
                    Assert.AreEqual(null, disposingValue);
                    Assert.AreEqual(false, wasDisposedRaised);
                    wasDisposed = true;
                    disposingValue = disposing;
                });

            subject.Disposed += (d) =>
            {
                wasDisposedRaised = true;
            };

            Assert.DoesNotThrow(() => subject.Dispose());

            Assert.AreEqual(true, wasDisposed);
            Assert.AreEqual(false, wasDisposedAsync);
            Assert.AreEqual(true, disposingValue);
            Assert.AreEqual(true, wasDisposedRaised);
            Assert.AreEqual(true, subject.IsDisposed);
        }
    }
}