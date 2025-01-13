using System.ComponentModel;

namespace LibnfcSharp.PInvoke
{
    public enum NfcError
    {
        [DefaultValue("Success (no error)")]
        NFC_SUCCESS = 0,

        [DefaultValue("Input / output error, device may not be usable anymore without re-open it")]
        NFC_EIO = -1,

        [DefaultValue("Invalid argument(s)")]
        NFC_EINVARG = -2,

        [DefaultValue("Operation not supported by device")]
        NFC_EDEVNOTSUPP = -3,
        [DefaultValue("No such device")]
        NFC_ENOTSUCHDEV = -4,

        [DefaultValue("Buffer overflow")]
        NFC_EOVFLOW = -5,

        [DefaultValue("Operation timed out")]
        NFC_ETIMEOUT = -6,

        [DefaultValue("Operation aborted (by user)")]
        NFC_EOPABORTED = -7,

        [DefaultValue("Not (yet) implemented")]
        NFC_ENOTIMPL = -8,

        [DefaultValue("Target released")]
        NFC_ETGRELEASED = -10,

        [DefaultValue("Error while RF transmission")]
        NFC_ERFTRANS = -20,

        [DefaultValue("MIFARE Classic: authentication failed")]
        NFC_EMFCAUTHFAIL = -30,

        [DefaultValue("Software error (allocation, file/pipe creation, etc.)")]
        NFC_ESOFT = -80,

        [DefaultValue("Device's internal chip error")]
        NFC_ECHIP = -90
    }
}
