using LibnfcSharp.Mifare;
using LibnfcSharp.Mifare.Enums;
using System;

namespace LibnfcSharp.CLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            do
            {
                using (var context = new NfcContext())
                using (var device = context.OpenDevice())
                {
                    var mfc = new MifareClassic(device);
                    mfc.RegisterKeyAProviderCallback(KeyAProviderCallback);
                    mfc.InitialDevice();
                    mfc.WaitForCard();
                    mfc.IdentifyMagicCardType();

                    Console.WriteLine($"Magic type: {mfc.MagicCardType}");

                    if (mfc.ReadManufacturerBlock(out byte[] manufacturerBlock))
                    {
                        Console.WriteLine("Manufacturer block:");
                        PrintHex(manufacturerBlock, MifareClassic.BLOCK_SIZE);
                        Console.WriteLine();
                    }

                    byte[] blockData;

                    mfc.ReadBlock(0, out blockData);
                    PrintHex(blockData, MifareClassic.BLOCK_SIZE);

                    mfc.ReadBlock(1, out blockData);
                    PrintHex(blockData, MifareClassic.BLOCK_SIZE);

                    mfc.ReadBlock(2, out blockData);
                    PrintHex(blockData, MifareClassic.BLOCK_SIZE);

                    mfc.ReadBlock(3, out blockData);
                    PrintHex(blockData, MifareClassic.BLOCK_SIZE);

                    if (mfc.MagicCardType == MifareMagicCardType.GEN_1 ||
                        mfc.MagicCardType == MifareMagicCardType.GEN_2)
                    {
                        if (mfc.Authenticate(0, MifareKeyType.KEY_A, MifareClassic.FACTORY_KEY))
                        {
                            Console.WriteLine("Authenticate successful");
                        }
                    }

                    if (mfc.WriteBlock(2, MifareClassic.EMPTY_BLOCK))
                    {
                        Console.WriteLine("Write successful");
                    }

                    mfc.ReadBlock(2, out blockData);
                    PrintHex(blockData, MifareClassic.BLOCK_SIZE);
                }
            }
            while (Console.ReadKey().Key == ConsoleKey.Enter);
        }

        private static byte[] KeyAProviderCallback(byte sector, byte[] uid)
        {
            return new byte[6] { 0x4B, 0x0B, 0x20, 0x10, 0x7C, 0xCB };
        }

        private static void PrintHex(byte[] pbtData, uint szBytes)
        {
            if (pbtData == null)
                return;

            for (uint i = 0; i < szBytes; i++)
                Console.Write("{0:x2}  ", pbtData[i]);
            Console.WriteLine();
        }
    }
}
