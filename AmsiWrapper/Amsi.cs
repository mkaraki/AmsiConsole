using System;
using System.Runtime.InteropServices;

namespace AmsiWrapper
{
    static public class Amsi
    {
        [DllImport("amsi.dll", CharSet = CharSet.Unicode)]
        public static extern int AmsiInitialize(string appName, ref IntPtr amsiContext);

        [DllImport("amsi.dll", CharSet = CharSet.Unicode)]
        public static extern int AmsiUninitialize(object amsiContext);

        [DllImport("amsi.dll", CharSet = CharSet.Unicode)]
        public static extern int AmsiScanBuffer(
            IntPtr amsiContext,
            IntPtr buffer,
            uint length,
            [MarshalAs(UnmanagedType.LPWStr)] string contentName,
            IntPtr amsiSession,
            out AMSI_RESULT result
        );

        [DllImport("amsi.dll", CharSet = CharSet.Unicode)]
        public static extern int AmsiScanString(
            IntPtr amsiContext,
            [MarshalAs(UnmanagedType.LPWStr)] string str,
            [MarshalAs(UnmanagedType.LPWStr)] string contentName,
            IntPtr amsiSession,
            out AMSI_RESULT result
        );
    }

    public enum AMSI_RESULT
    {
        AMSI_RESULT_CLEAN = 0,
        AMSI_RESULT_NOT_DETECTED = 1,
        AMSI_RESULT_BLOCKED_BY_ADMIN_START = 16384,
        AMSI_RESULT_BLOCKED_BY_ADMIN_END = 20479,
        AMSI_RESULT_DETECTED = 32768,
    }
}
