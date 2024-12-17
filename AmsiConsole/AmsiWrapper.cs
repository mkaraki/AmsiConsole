using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AmsiConsole
{
    internal class AmsiWrapper : IDisposable
    {
        private IntPtr amsiContext = IntPtr.Zero;

        public AmsiWrapper(string appName) 
        { 
            int ecode = Amsi.AmsiInitialize(appName, ref amsiContext);
            if (ecode != 0)
            {
                throw new Exception("Failed to initialize AMSI");
            }
        }

        public Amsi.AMSI_RESULT ScanBuffer(byte[] buffer, string contentName)
        {
            IntPtr bufferPtr = Marshal.AllocHGlobal(buffer.Length);
            Amsi.AMSI_RESULT result = Amsi.AMSI_RESULT.AMSI_RESULT_NOT_DETECTED;
            try
            {
                Marshal.Copy(buffer, 0, bufferPtr, buffer.Length);
                int ecode = Amsi.AmsiScanBuffer(amsiContext, bufferPtr, (uint)buffer.Length, contentName, IntPtr.Zero, out result);
                if (ecode != 0)
                {
                    throw new Exception("Failed to scan buffer");
                }
            }
            finally
            {
                Marshal.FreeHGlobal(bufferPtr);
            }
            return result;
        }

        public Amsi.AMSI_RESULT ScanString(string content, string contentName)
        {
            int ecode = Amsi.AmsiScanString(amsiContext, content, contentName, IntPtr.Zero, out Amsi.AMSI_RESULT result);
            if (ecode != 0)
            {
                throw new Exception("Failed to scan string");
            }
            return result;
        }

        public void Dispose()
        {
            // This occurs access violation exception.
            /*int ecode = Amsi.AmsiUninitialize(amsiContext);
            if (ecode != 0)
            {
                throw new Exception("Failed to uninitialize AMSI");
            }*/
        }
    }
}
