# LibnfcSharp

[![NuGet Package](https://img.shields.io/nuget/v/LibnfcSharp.svg)](https://www.nuget.org/packages/LibnfcSharp/)
[![Build Status](https://img.shields.io/appveyor/ci/MirakelDev/LibnfcSharp.svg)](https://ci.appveyor.com/project/MirakelDev/LibnfcSharp)

**LibnfcSharp** is a .NET wrapper for the `libnfc` library (version 1.8.0, x86 build), providing a managed interface for developers to easily interact with NFC (Near Field Communication) devices in their .NET applications. This wrapper simplifies the integration of NFC functionalities, allowing for seamless communication with various NFC hardware.

## Features

- Easy-to-use API for accessing NFC functionalities.
- Support for reading and writing NFC tags.
- Compatible with a wide range of NFC devices, specifically:
  - **ACR122U**
  - **PN532 V2**
- **Dependency:** Uses [libusb-win32](https://github.com/mcuee/libusb-win32/releases/tag/release_1.4.0.0) (x86 build) for USB communication.

## Native `libnfc` Repository

For more information about the underlying library, visit the [libnfc GitHub Repository](https://github.com/nfc-tools/libnfc).

## Installation

You can install the library via NuGet Package Manager or the .NET CLI.

### Using NuGet Package Manager

```bash
Install-Package LibnfcSharp
```

### Using .NET CLI

```bash
dotnet add package LibnfcSharp
```

### Getting Started

To get started with LibnfcSharp, follow these steps:

1. Create a new .NET project or open an existing one.


2. Install the LibnfcSharp package using one of the installation methods mentioned above.


3. Use the following example code to interact with NFC devices:

```csharp
using System;
using LibnfcSharp;
using LibnfcSharp.Mifare;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            using (var nfcContext = new NfcContext())
            using (var nfcDevice = nfcContext.OpenDevice())
            {

                var mfc = new MifareClassic(nfcDevice);
                mfc.InitialDevice();
                mfc.WaitForCard();

                byte[] data;
                mfc.ReadCard(out data);

                mfc.WriteDump(data);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
```

### Contributing

Contributions are welcome! If you would like to contribute to the project, please follow these steps:

1. Fork the repository.

2. Create a new feature branch (git checkout -b feature/YourFeature).

3. Make your changes and commit them (git commit -m 'Add some feature').

4. Push to the branch (git push origin feature/YourFeature).

5. Create a pull request.


### License

This project is licensed under the LGPLv3 License.

### Acknowledgments

- [libnfc](https://github.com/nfc-tools/libnfc) - The original library that this wrapper interfaces with (version 1.8.0, x86 build).

- [libusb-win32](https://github.com/mcuee/libusb-win32) - Used for USB communication (x86 build).
