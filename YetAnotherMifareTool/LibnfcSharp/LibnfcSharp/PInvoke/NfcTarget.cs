using System.Runtime.InteropServices;

namespace LibnfcSharp.PInvoke
{
    #region Enums

    public enum NfcModulationType
    {
        NMT_UNDEFINED = 0,
        NMT_ISO14443A = 1,
        NMT_JEWEL,
        NMT_ISO14443B,
        NMT_ISO14443BI, // pre-ISO14443B aka ISO/IEC 14443 B' or Type B'
        NMT_ISO14443B2SR, // ISO14443-2B ST SRx
        NMT_ISO14443B2CT, // ISO14443-2B ASK CTx
        NMT_FELICA,
        NMT_DEP,
        NMT_BARCODE, // Thinfilm NFC Barcode
        NMT_ISO14443BICLASS, // HID iClass 14443B mode
        NMT_END_ENUM = NMT_ISO14443BICLASS, // dummy for sizing - always should alias last
    }

    public enum NfcBaudRate
    {
        NBR_UNDEFINED = 0,
        NBR_106 = 1,
        NBR_212,
        NBR_424,
        NBR_847,
    }

    #endregion Enums

    #region Structs

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NfcTarget
    {
        public NfcTargetInfo TargetInfo;
        public NfcModulation Modulation;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NfcTargetInfo
    {
        public NfcIso14443aInfo Iso14443aInfo;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NfcIso14443aInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] abtAtqa;//[2];
        public byte btSak;
        public uint szUidLen;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] abtUid;//[10];
        public uint szAtsLen;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 254)]
        public byte[] abtAts;//[254]; // Maximal theoretical ATS is FSD-2, FSD=256 for FSDI=8 in RATS
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NfcModulation
    {
        public NfcModulationType ModulationType;
        public NfcBaudRate BaudRate;
    }

    #endregion Structs
}
