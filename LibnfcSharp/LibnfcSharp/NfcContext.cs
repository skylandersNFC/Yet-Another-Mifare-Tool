using LibnfcSharp.PInvoke;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LibnfcSharp
{
    public class NfcContext : IDisposable
    {
        protected IntPtr contextPointer;
        private bool disposed = false;

        public NfcContext()
        {
            var version = Libnfc.Version;

            Libnfc.Init(ref contextPointer);
            if (contextPointer == IntPtr.Zero)
                throw new Exception("Unable to init libnfc (malloc)");
        }

        public List<string> ListDevices(int count = Libnfc.MaxUserDefinedDevices + 6)
        {
            IntPtr connectionStringsPointer = Marshal.AllocHGlobal(Libnfc.ConnectStringBufferSize * count);
            var devicesCount = Libnfc.ListDevices(contextPointer, connectionStringsPointer, (uint)count);

            var devices = new List<string>(devicesCount);
            for (int i = 0; i < devicesCount; i++)
                devices.Add(Marshal.PtrToStringAnsi(connectionStringsPointer + i * count));

            Marshal.FreeHGlobal(connectionStringsPointer);
            return devices;
        }

        public virtual NfcDevice OpenDevice(string name = null)
        {
            IntPtr devicePointer;
            try
            {
                devicePointer = Libnfc.Open(contextPointer, name);
                if (devicePointer == IntPtr.Zero)
                    throw new Exception();
            }
            catch (Exception)
            {
                throw new Exception("Error opening NFC reader");
            }

            return new NfcDevice(devicePointer);
        }

        public string Version => Libnfc.Version;

        public void Exit() => Dispose();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    // component.Dispose();
                }
                Libnfc.Exit(contextPointer);
                contextPointer = IntPtr.Zero;
                disposed = true;
            }
        }

        ~NfcContext()
        {
            Dispose(false);
        }
    }
}
