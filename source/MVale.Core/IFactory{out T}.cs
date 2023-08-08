using System;

namespace MVale.Core
{
    public interface IFactory<out T>
    {
        T Create();
    }
}