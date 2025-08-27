# Yet Another Mifare Tool (YAMT)

## V1.4.0 - Update pending...
The current instructions are for V1.3.0

## How To Use

1. Download the correct **[Yet-Another-Mifare-Tool-XXXX.zip](https://github.com/skylandersNFC/Yet-Another-Mifare-Tool/releases)** archive for you device  and **extract it**.
2. Place a **[Mifare S50 1K](https://skylandersnfc.github.io/Docs/Skylanders_Buying_List/Skylanders_NFC_Cards/)** card onto your **[ACR122U](https://skylandersnfc.github.io/Docs/Skylanders_Buying_List/Skylanders_NFC_Devices/#acr122u-all-skylanders)** or **[PN532 V2.0](https://skylandersnfc.github.io/Docs/Skylanders_Buying_List/Skylanders_NFC_Devices/#pn532-v20-all-skylanders)** NFC device.
3. Run "**Yet-Another-Mifare-Tool-XXXX.exe**" as **Admin** and use "**Select dumps...**" to select a Skylander dump from the **[Ultimate NFC Pack](https://skylandersnfc.github.io/Skylanders-Ultimate-NFC-Pack/)**.
4. Simply click "**WRITE DUMP**" and wait for it to finish.
5. **That's it!** The software manages all the technical details, so you don’t have to worry about a thing.

## Overview

**Yet Another Mifare Tool (YAMT)** supports **[ACR122U](https://skylandersnfc.github.io/Docs/Skylanders_Buying_List/Skylanders_NFC_Devices/#acr122u-all-skylanders)** and **[PN532 V2.0](https://skylandersnfc.github.io/Docs/Skylanders_Buying_List/Skylanders_NFC_Devices/#pn532-v20-all-skylanders)** NFC devices.

You can write to **[Mifare S50 1K](https://skylandersnfc.github.io/Docs/Skylanders_Buying_List/Skylanders_NFC_Cards/)** **Gen1 UID Re-Writable**, **Gen1 UID Locked** and **Gen2 CUID** cards.

**Forget** the old days when you had to use the ***[Mifare Windows Tool (MWT)](https://github.com/ElDavoo/Mifare-Windows-Tool-Reborn)***, checking countless options and ensuring the card was decrypted.

**No more extra tools** like ***[TheSkyLib](https://github.com/skylandersNFC/TheSkyLib)*** or ***[SkyUID](https://github.com/skylandersNFC/SkyUID-Generator/)*** to prepare the dump's UID.

**No more driver issues**, **wrong access conditions** or **unnecessary** complications.

**Yet Another Mifare Tool (YAMT)** handles everything automatically for you.

![todd-howard-it-just-works](https://media1.tenor.com/m/rkI1a8s2Z6QAAAAC/todd-howard-it-just-works.gif)

## Is Yet Another Mifare Tool (YAMT) better than Mifare Windows Tool (MWT) for Skylanders?

![Much Better](https://media1.tenor.com/m/fvCLHApzwu4AAAAC/much-better-guy.gif)

## Screenshots
![Gen1A_Start](./screenshots/YAMT_Gen1A_1.png)
![Gen1A_End](./screenshots/YAMT_Gen1A_2.png)

## Technical Details

- For **Gen1a** and **Gen1b** **UID Re-Writable** cards, Yet Another Mifare Tool (**YAMT**) uses backdoor commands to write the exact data from the dump onto the card.
     - You can use those cards **for all Skylanders**, including **Imaginators**.
     - Such cards can also be **re-written with any type of Skylanders using backdoor commands**, which completely resets the card.

- For **Gen1 UID Locked** cards that are **empty**, YAMT modifies the Skylanders dump to unlock access conditions and matches the card's original locked UID.
     - You **cannot write Imaginators to Gen1 UID Locked** cards. They require **Gen1 UID Re-Writable** or **Gen2 CUID** cards instead.
     - For **Gen1 UID Locked** cards that have **already been written**, YAMT can calculate Keys A based on the UID and rewrite data while adjusting the dump for the existing UID on the card, as long as the original access conditions aren’t locked.

- For **Gen2 CUID** cards that are **empty**, YAMT operates similarly to Gen1 UID Locked cards, but includes the ability to **change the UID** (which allows them to be **used for Imaginators**).
     - However, Gen2 cards **cannot be re-written with another Imaginator** (_once **written**_), but can be **rewritten** with **any other Skylander**.

## Errors Explained

 - **YAMT crashed before writing the dump, closing the application automatically**
> [!NOTE]
> This issue occurs **randomly**, usually with the **[PN532 V2.0](https://skylandersnfc.github.io/Docs/Skylanders_Buying_List/Skylanders_NFC_Devices/#pn532-v20-all-skylanders)** and roughly once every **10-20 writes**. Occasionally, the driver misbehaves, which seems to stem from the original libnfc implementation.
>
> However, it's **far less frequent** compared to how often the **MWT crashed**. Simply **relaunching** the **YAMT** application should **resolve** this issue.
>
> If the problem persists, you can try the initial release of **[YAMT 1.0.0](https://github.com/skylandersNFC/Yet-Another-Mifare-Tool/releases/tag/v1.0.0)**, as several users have reported more stable behavior with this version, when encountering such crashes.

---

 - **YAMT is showing "Error: Writing block XX failed!"**
> [!NOTE]
> Try using the "**Reset**" option first before writing to the card. This can help with certain problematic cards, hence avoiding this issue.

---

 - **System.Exception: Failed to connect to NFC reader.**
> [!NOTE]
> Please **connect** your **ACR122U** or **PN532 V2.0** NFC device. Ensure that you've downloaded the correct archive for your NFC device.
> 
> Also, make sure you have the proper drivers installed for your device.
> 
> For **[ACR122U](https://skylandersnfc.github.io/Docs/Skylanders_Buying_List/Skylanders_NFC_Devices/#acr122u-all-skylanders)** you need **[ACS Unified Driver MSI Win 4280.zip](https://skylandersnfc.github.io/Docs/Skylanders_Buying_List/Skylanders_NFC_Devices/ACR122U/drivers/ACS_Unified_Driver_MSI_Win_4280.zip)** driver.
> 
> For **[PN532 V2.0](https://skylandersnfc.github.io/Docs/Skylanders_Buying_List/Skylanders_NFC_Devices/#pn532-v20-all-skylanders)** you need **[CH341SER.zip](https://skylandersnfc.github.io/Docs/Skylanders_Buying_List/Skylanders_NFC_Devices/PN532/drivers/CH341SER.zip)** driver.

> [!TIP]
> For **[PN532 V2.0](https://skylandersnfc.github.io/Docs/Skylanders_Buying_List/Skylanders_NFC_Devices/#pn532-v20-all-skylanders)**, if you've followed **all the steps above** and it **still doesn't work**, I have one final **trick** for you.
> 
> Since this is a **COM port device**, typically it will use port **COM3**. However, if your machine is already connected to **another COM device** or some other _**black magic**_ was involved, the **[PN532 V2.0](https://skylandersnfc.github.io/Docs/Skylanders_Buying_List/Skylanders_NFC_Devices/#pn532-v20-all-skylanders)** might instead be assigned to **COM4** or even different port.
> 
> This **won't work** with the existing **`libnfc.conf`** configuration file, since it is set to look for a **`pn532_uart`** device on **COM3**.
> 
> You can easily check your **COM port** by opening the **Command Prompt** and typing "**`chgport`**".
> 
> In an ideal scenario, you **should see** something like this:
> 
> ```
> AUX = \DosDevices\COM1
> COM1 = \Device\Serial0
> COM3 = \Device\Serial2
> ```
> 
> However, in reality, the last row will **likely show COM4** or **another COM** port, hence your problem. You need to **note this COM port number**, then open the **`libnfc.conf`** file with **Notepad**.
> 
> **Change** the **line**:
> 
> ```ini
> device.connstring = "pn532_uart:COM3:115200"
> ```
> 
> to reflect the **correct COM port you're using**.

---

 - **System.DllNotFoundException: Unable to load DLL 'libnfc' or one of its dependencies: The specified module could not be found. (0x8007007E)**
> [!NOTE]
> **Extract** the "**Yet-Another-Mifare-Tool-XXXX.zip**" file and **run** the "**Yet-Another-Mifare-Tool-XXXX.exe**". Ensure it's **not run directly from the zip archive**.

---

 - **Error: Unable to write toys with signature to Gen1 UID LOCKED cards. Use a toy without signature or another card...**
> [!NOTE]
> You **cannot** write **Imaginators** to **Gen1 UID Locked** cards. Please use **Gen1A UID Unlocked/Re-Writable** or **blank Gen2** cards instead.

---

 - **Error: Unable to write toys with signature to used Gen2 CUID cards. Use a toy without signature or another card...**
> [!NOTE]
> Gen2 cards **cannot** be **rewritten** with a different **Imaginator**. They can only be **rewritten** with **regular Skylanders**.

---

 - **Error: Sector 0 is locked (by access conditions). Use another card...**
 - **Error: Uid is locked and sector 0 is locked (by access conditions)! Use another card...**
> [!NOTE]
> You've used Skylanders dumps **different** from those in the **Ultimate NFC Pack V7**, causing **locked access conditions** for **Sector 0**.
> 
> To avoid this, use a **new blank card** and the **Ultimate NFC Pack V7** to ensure Sector 0 remains **unlocked**.

---

 - **Error: Unknown magic card type. Use another card...**
> [!NOTE]
> You need **Mifare S50 1k** cards. **NTAG** or other cards are **not compatible**.

---

 - **Error: Failed to read manufacturer block!**
> [!NOTE]
> If you're using a **Mifare S50 1k** cards, there may be an issue with the **current card**. Try another one from the batch.

---

 - **Error: Authenticating sector 1 failed!**
> [!NOTE]
> It seems you have a **Mifare Classic S50 1K UID Locked** card and previously used another software to write data to it.
> 
> Since this data **wasn't aligned** with the card's fixed UID, it's **incorrect** now.
> 
> As a result, from Sector 1 onward, the data **cannot be decoded** due to **mismatched Access Control keys**.
> 
> You’ll need to **use a new, untouched card**.

---

 - **Error: No card found!**
> [!NOTE]
> Place a **Mifare S50 1k** card on the reader.

---

 - **Dump is not valid!**
> [!NOTE]
> Use the **[Ultimate NFC Pack](https://skylandersnfc.github.io/Skylanders-Ultimate-NFC-Pack/)**.

---

 - **No dump selected!**
> [!NOTE]
> Select a dump file from the **[Ultimate NFC Pack](https://skylandersnfc.github.io/Skylanders-Ultimate-NFC-Pack/)**.

---

 - **You must install .NET Runtime to run this application.**
> [!NOTE]
> Download and install **[.NET Runtime 6.0.36](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)**.
> 
> You can also use these direct links: **[.NET Runtime 6.0.36 x64](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-6.0.36-windows-x64-installer)** and **[.NET Runtime 6.0.36 x86](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-6.0.36-windows-x86-installer)**. Install both just in case.
> 
> If you are still having problems, install this as well: **[Visual C++ Redistributable Runtimes All-in-One](https://www.techpowerup.com/download/visual-c-redistributable-runtime-package-all-in-one/)**
>
> Extract the zip archive and run the included "install_all.bat" with admin privileges (right click, "Run as administrator").
