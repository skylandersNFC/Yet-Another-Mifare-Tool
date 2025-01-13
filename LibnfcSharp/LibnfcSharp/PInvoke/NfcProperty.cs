namespace LibnfcSharp.PInvoke
{
    public enum NfcProperty
    {
        /// <summary>
        /// Default command processing timeout
        /// Property value's (duration) unit is ms and 0 means no timeout (infinite).
        /// Default value is set by driver layer
        /// </summary>
        TimeoutCommand,

        /// <summary>
        /// Timeout between ATR_REQ and ATR_RES
        /// When the device is in initiator mode, a target is considered as mute if no
        /// valid ATR_RES is received within this timeout value.
        /// Default value for this property is 103 ms on PN53x based devices.
        /// </summary>
        TimeoutAtr,

        /// <summary>
        /// Timeout value to give up reception from the target in case of no answer.
        /// Default value for this property is 52 ms).
        /// </summary>
        TimeoutCom,

        /// <summary>
        /// Let the PN53X chip handle the CRC bytes. This means that the chip appends
        /// the CRC bytes to the frames that are transmitted. It will parse the last
        /// bytes from received frames as incoming CRC bytes. They will be verified
        /// against the used modulation and protocol. If an frame is expected with
        /// incorrect CRC bytes this option should be disabled. Example frames where
        /// this is useful are the ATQA and UID+BCC that are transmitted without CRC
        /// bytes during the anti-collision phase of the ISO14443-A protocol.
        /// </summary>
        HandleCrc,

        /// <summary>
        /// Parity bits in the network layer of ISO14443-A are by default generated and
        /// validated in the PN53X chip. This is a very convenient feature. On certain
        /// times though it is useful to get full control of the transmitted data. The
        /// proprietary MIFARE Classic protocol uses for example custom (encrypted)
        /// parity bits. For interoperability it is required to be completely
        /// compatible, including the arbitrary parity bits. When this option is
        /// disabled, the functions to communicating bits should be used.
        /// </summary>
        HandleParity,

        /// <summary>
        /// This option can be used to enable or disable the electronic field of the
        /// NFC device.
        /// </summary>
        ActivateField,

        /// <summary>
        /// The internal CRYPTO1 co-processor can be used to transmit messages
        /// encrypted. This option is automatically activated after a successful MIFARE
        /// Classic authentication.
        /// </summary>
        ActivateCrypto1,

        /// <summary>
        /// The default configuration defines that the PN53X chip will try indefinitely
        /// to invite a tag in the field to respond. This could be desired when it is
        /// certain a tag will enter the field. On the other hand, when this is
        /// uncertain, it will block the application. This option could best be compared
        /// to the (NON)BLOCKING option used by (socket)network programming.
        /// </summary>
        InfiniteSelect,

        /// <summary>
        /// If this option is enabled, frames that carry less than 4 bits are allowed.
        /// According to the standards these frames should normally be handles as
        /// invalid frames.
        /// </summary>
        AcceptInvalidFrames,

        /// <summary>
        /// If the NFC device should only listen to frames, it could be useful to let
        /// it gather multiple frames in a sequence. They will be stored in the internal
        /// FIFO of the PN53X chip. This could be retrieved by using the receive data
        /// functions. Note that if the chip runs out of bytes (FIFO = 64 bytes long),
        /// it will overwrite the first received frames, so quick retrieving of the
        /// received data is desirable.
        /// </summary>
        AcceptMultipleFrames,

        /// <summary>
        /// This option can be used to enable or disable the auto-switching mode to
        /// ISO14443-4 is device is compliant.
        /// In initiator mode, it means that NFC chip will send RATS automatically when
        /// select and it will automatically poll for ISO14443-4 card when ISO14443A is
        /// requested.
        /// In target mode, with a NFC chip compliant (ie. PN532), the chip will
        /// emulate a 14443-4 PICC using hardware capability
        /// </summary>
        AutoIso14443_4,

        /// <summary>
        /// Use automatic frames encapsulation and chaining.
        /// </summary>
        EasyFraming,

        /// <summary>
        /// Force the chip to switch in ISO14443-A
        /// </summary>
        ForceIso14443_A,

        /// <summary>
        /// Force the chip to switch in ISO14443-B
        /// </summary>
        ForceIso14443_B,

        /// <summary>
        /// Force the chip to run at 106 kbps
        /// </summary>
        ForceSpeed106
    }
}
