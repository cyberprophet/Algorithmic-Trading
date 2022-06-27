using Microsoft.Win32;

using System.Collections;
using System.Runtime.Versioning;

namespace ShareInvest
{
    enum DigitalProductIdVersion
    {
        UpToWindows7,
        Windows8AndUp
    }
    [SupportedOSPlatform("windows8.0")]
    class KeyDecoder
    {
        static string GetWindowsProductKeyFromDigitalProductId(byte[] digitalProductId, DigitalProductIdVersion digitalProductIdVersion) => digitalProductIdVersion == DigitalProductIdVersion.Windows8AndUp ? DecodeProductKeyWin8AndUp(digitalProductId) : DecodeProductKey(digitalProductId);
        static string DecodeProductKey(byte[] digitalProductId)
        {
            char[] digits = new[] { 'B', 'C', 'D', 'F', 'G', 'H', 'J', 'K', 'M', 'P', 'Q', 'R', 'T', 'V', 'W', 'X', 'Y', '2', '3', '4', '6', '7', '8', '9' }, decodedChars = new char[decodeLength];
            var hexPid = new ArrayList();

            for (var i = keyStartIndex; i <= keyEndIndex; i++)
                hexPid.Add(digitalProductId[i]);

            for (var i = decodeLength - 1; i >= 0; i--)
            {
                if ((i + 1) % 6 == 0)
                    decodedChars[i] = '-';

                else
                {
                    var digitMapIndex = 0;

                    for (var j = decodeStringLength - 1; j >= 0; j--)
                    {
                        var byteValue = (digitMapIndex << 8) | (byte)hexPid[j]!;
                        hexPid[j] = (byte)(byteValue / 0x18);
                        digitMapIndex = byteValue % 0x18;
                        decodedChars[i] = digits[digitMapIndex];
                    }
                }
            }
            return new string(decodedChars);
        }
        static string DecodeProductKeyWin8AndUp(byte[] digitalProductId)
        {
            var key = string.Empty;
            var isWin8 = (byte)((digitalProductId[0x42] / 6) & 1);
            var last = 0;
            digitalProductId[0x42] = (byte)((digitalProductId[0x42] & 0xf7) | (isWin8 & 2) * 4);

            for (var i = 0x18; i >= 0; i--)
            {
                var current = 0;

                for (var j = 0xE; j >= 0; j--)
                {
                    current *= 0x100;
                    current = digitalProductId[j + keyOffset] + current;
                    digitalProductId[j + keyOffset] = (byte)(current / 0x18);
                    current %= 0x18;
                    last = current;
                }
                key = digits[current] + key;
            }
            string keypart1 = key.Substring(1, last), keypart2 = key.Substring(1 + last, key.Length - (last + 1));
            key = string.Concat(keypart1, "N", keypart2);

            for (var i = 5; i < key.Length; i += 6)
                key = key.Insert(i, "-");

            return key;
        }
        internal static string? ProductKeyFromRegistry
        {
            get
            {
                using (var localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32))
                {
                    var registryKeyValue = localKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion")?.GetValue("DigitalProductId");

                    if (registryKeyValue != null)
                    {
                        var digitalProductId = (byte[])registryKeyValue;
                        var isWin8OrUp = Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor >= 2 || Environment.OSVersion.Version.Major > 6;
                        localKey.Close();

                        return GetWindowsProductKeyFromDigitalProductId(digitalProductId, isWin8OrUp ? DigitalProductIdVersion.Windows8AndUp : DigitalProductIdVersion.UpToWindows7);
                    }
                }
                return null;
            }
        }
        const int decodeLength = 0x1D;
        const int decodeStringLength = 0xF;
        const int keyOffset = 0x34;
        const int keyStartIndex = 0x34;
        const int keyEndIndex = keyStartIndex + 0xF;
        const string digits = "BCDFGHJKMPQRTVWXY2346789";
    }
}