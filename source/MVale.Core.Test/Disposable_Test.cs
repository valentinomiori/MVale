using System;
using NUnit.Framework;

namespace MVale.Core.Test
{
    [TestFixture(TestOf = typeof(Disposable))]
    public class Disposable_Test
    {
        [Test]
        public void Test()
        {
            bool wasDisposed = false;
            bool wasDisposedRaised = false;

            Disposable subject = null;

            subject = Disposable.Create(
                () =>
                {
                    wasDisposed = true;
                    Assert.AreEqual(false, wasDisposedRaised);
                    Assert.AreEqual(false, subject.IsDisposed);
                },
                onDisposing: true);

            subject.Disposed += (s) =>
            {
                wasDisposedRaised = true;
                Assert.AreEqual(true, wasDisposed);
                Assert.AreEqual(true, s.IsDisposed);
            };

            Assert.DoesNotThrow(() =>
            {
                using var disposable = subject;
            });

            Assert.AreEqual(true, wasDisposed);
            Assert.AreEqual(true, wasDisposedRaised);
            Assert.AreEqual(true, subject.IsDisposed);

            wasDisposed = false;
            wasDisposedRaised = false;

            Assert.DoesNotThrow(() => subject.Dispose());

            Assert.AreEqual(false, wasDisposed);
            Assert.AreEqual(false, wasDisposedRaised);
            Assert.AreEqual(true, subject.IsDisposed);

            subject = Disposable.Create(
                () =>
                {
                    wasDisposed = true;
                    Assert.AreEqual(false, wasDisposedRaised);
                    Assert.AreEqual(false, subject.IsDisposed);
                    throw new TestException();
                },
                onDisposing: true);

            wasDisposed = false;
            wasDisposedRaised = false;

            Assert.Throws<TestException>(() => subject.Dispose());

            Assert.AreEqual(true, wasDisposed);
            Assert.AreEqual(false, wasDisposedRaised);
            Assert.AreEqual(false, subject.IsDisposed);

            Assert.Throws<TestException>(() => subject.Dispose());
        }
    }
}