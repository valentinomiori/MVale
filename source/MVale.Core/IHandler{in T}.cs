using System;

namespace MVale.Core
{
    public interface IHandler<in T>
    {
        void Handle(T instance);
    }
}