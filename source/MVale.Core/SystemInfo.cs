using System;

namespace MVale.Core
{
    /// <summary>
    /// This class is a collection of properties regarding the current system.
    /// </summary>
    public static class SystemInfo
    {
        /// <summary>
        /// This property is <see langword="true"/> if the default behavior for arithmetic
        /// calculations (those not enclosed in a <see langword="checked"/> or <see langword="unchecked"/> block)
        /// is checked (an <see cref="OverflowException"/> is thrown if overflow occours).
        /// </summary>
        public static bool DefaultIsChecked { get; }

        public static bool IsNetCoreApp
        {
            get
            {
#if NETCOREAPP
                return true;
#else
                return false;
#endif
            }
        }

        static SystemInfo()
        {
            DefaultIsChecked = GetDefaultIsChecked();
        }

        private static bool GetDefaultIsChecked()
        {
            bool isSignedChecked;
            bool isUnsignedChecked;

            try
            {
                int i = int.MaxValue;
                i++;
                isSignedChecked = false;
            }
            catch (OverflowException)
            {
                isSignedChecked = true;
            }

            try
            {
                uint i = uint.MaxValue;
                i++;
                isUnsignedChecked = false;
            }
            catch (OverflowException)
            {
                isUnsignedChecked = true;
            }

            return isSignedChecked == isUnsignedChecked
                ? isSignedChecked && isUnsignedChecked
                : throw new InvalidOperationException();
        }
    }
}