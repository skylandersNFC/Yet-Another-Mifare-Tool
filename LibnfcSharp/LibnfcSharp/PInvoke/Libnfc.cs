using System;
using System.Runtime.InteropServices;

namespace LibnfcSharp.PInvoke
{
    public static class Libnfc
    {
        public const string LibraryName = "libnfc";
        public const int DeviceNameLength = 256;
        public const int DevicePortLength = 64;
        public const int MaxUserDefinedDevices = 4;
        public const int ConnectStringBufferSize = 1024;

        /// <summary>
        /// Initialize libnfc.
        /// This function must be called before calling any other libnfc function
        /// </summary>
        /// <param name="context">Output location for NfcContext</param>
        [DllImport(LibraryName, EntryPoint = "nfc_init", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Init(ref IntPtr context);

        /// <summary>
        /// Deinitialize libnfc.
        /// Should be called after closing all open devices and before your application terminates.
        /// </summary>
        /// <param name="context">The context to deinitialize</param>
        [DllImport(LibraryName, EntryPoint = "nfc_exit", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exit(IntPtr context);

        /// <summary>
        /// Open a NFC device.
        /// If <paramref name="connstring"/> is null, the first available device from NfcListDevices function is used.
        ///
        /// If <paramref name="connstring"/> is set, this function will try to claim the right device using information provided by <paramref name="connstring"/>.
        ///
        /// When it has successfully claimed a NFC device, memory is allocated to save the device information.
        /// It will return a pointer to a NfcDevice struct.
        /// This pointer should be supplied by every next functions of libnfc that should perform an action with this device.
        /// </summary>
        /// <remarks>
        /// Depending on the desired operation mode, the device needs to be configured by using NfcInitiatorInit() or NfcTargetInit(),
        /// optionally followed by manual tuning of the parameters if the default parameters are not suiting your goals.
        /// </remarks>
        /// <param name="context">The context to operate on</param>
        /// <param name="connstring">The device connection string if specific device is wanted, null otherwise</param>
        /// <returns>Returns pointer to a NfcDevice struct if successfull; otherwise returns null value.</returns>
        [DllImport(LibraryName, EntryPoint = "nfc_open", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Open(IntPtr context, string connstring);

        /// <summary>
        /// Close from a NFC device.
        /// Initiator's selected tag is closed and the device, including allocated NfcDevice struct, is released.
        /// </summary>
        /// <param name="pnd">NfcDevice struct pointer that represent currently used device</param>
        [DllImport(LibraryName, EntryPoint = "nfc_close", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Close(IntPtr pnd);

        [DllImport(LibraryName, EntryPoint = "nfc_list_devices", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ListDevices(IntPtr context, IntPtr connstrings, uint connstrings_len);

        [DllImport(LibraryName, EntryPoint = "nfc_abort_command", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AbortCommand(IntPtr pnd);

        [DllImport(LibraryName, EntryPoint = "nfc_initiator_init", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitiatorInit(IntPtr pnd);

        [DllImport(LibraryName, EntryPoint = "nfc_initiator_select_passive_target", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitiatorSelectPassiveTarget(IntPtr pnd, NfcModulation nm, byte[] pbtInitData, uint szInitData, ref NfcTarget pnt);

        [DllImport(LibraryName, EntryPoint = "nfc_perror", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Perror(IntPtr pnd, string s);

        [DllImport(LibraryName, EntryPoint = "nfc_device_get_name", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr _DeviceGetName(IntPtr pnd);

        public static string DeviceGetName(IntPtr pnd) => Marshal.PtrToStringAnsi(_DeviceGetName(pnd));

        [DllImport(LibraryName, EntryPoint = "nfc_device_set_property_bool", CallingConvention = CallingConvention.Cdecl)]
        public static extern int DeviceSetPropertyBool(IntPtr pnd, NfcProperty property, bool bEnable);

        [DllImport(LibraryName, EntryPoint = "nfc_initiator_transceive_bytes", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitiatorTransceiveBytes(IntPtr pnd, byte[] pbtTx, uint szTx, byte[] pbtRx, uint szRx, int timeout);

        [DllImport(LibraryName, EntryPoint = "nfc_initiator_transceive_bits", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitiatorTransceiveBits(IntPtr pnd, byte[] pbtTx, uint szTxBits, byte[] pbtTxPar, byte[] pbtRx, uint szRx, byte[] pbtRxPar);

        [DllImport(LibraryName, EntryPoint = "nfc_initiator_transceive_bytes_timed", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitiatorTransceiveBytesTimed(IntPtr pnd, byte[] pbtTx, uint szTx, byte[] pbtRx, uint szRx, ref uint cycles);

        [DllImport(LibraryName, EntryPoint = "nfc_initiator_transceive_bits_timed", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitiatorTransceiveBitsTimed(IntPtr pnd, byte[] pbtTx, uint szTxBits, byte[] pbtTxPar, byte[] pbtRx, uint szRx, byte[] pbtRxPar, ref uint cycles);

        [DllImport(LibraryName, EntryPoint = "iso14443a_crc_append", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Iso14443aCrcAppend(byte[] pbtData, uint szLen);

        [DllImport(LibraryName, EntryPoint = "nfc_version", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr _Version();

        /// <summary>
        /// Returns the library version
        /// </summary>
        /// <returns>Returns a string with the library version</returns>
        public static string Version => Marshal.PtrToStringAnsi(_Version());
    }
}
