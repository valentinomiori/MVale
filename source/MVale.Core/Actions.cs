using System;

namespace MVale.Core
{
    public static class Actions
    {
        private static class Internal
        {
            public static void NoOpMethod()
            {
            }
        }

        public static Action NoOp => Internal.NoOpMethod;
    }
}