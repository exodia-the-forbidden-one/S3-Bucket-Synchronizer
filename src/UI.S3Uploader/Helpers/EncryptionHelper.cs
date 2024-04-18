using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Management;

namespace Uploader_UI.Helpers
{
    internal class EncryptionHelper
    {
        public static string Encrypt(string text)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(GetHwid());
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(text);
                        }
                    }
                    array = ms.ToArray();
                }
            }
            return Convert.ToBase64String(array);
        }

        public static string Decrypt(string text)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(text);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(GetHwid());
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cs))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
        }

        private static string GetHwid()
        {
            string hwid = string.Empty;
            ManagementClass mc = new("Win32_ComputerSystemProduct");
            using (ManagementObjectCollection moc = mc.GetInstances())
            {
                foreach (var mo in moc)
                {
                    hwid = (string)mo.Properties["UUID"].Value;
                    break;
                }
            }

            return hwid.Replace("-", "");
        }
    }
}
