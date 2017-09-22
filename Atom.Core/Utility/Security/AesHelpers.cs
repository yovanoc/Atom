using System;
using System.IO;
using System.Security.Cryptography;

namespace Atom.Core.Utility.Security
{
    public static class AesHelpers
    {
        // Fields
        private static readonly Aes Aes;

        private static readonly byte[] Iv =
            {115, 100, 102, 111, 98, 110, 101, 54, 122, 102, 108, 100, 59, 115, 103, 53};

        private static readonly byte[] Key =
        {
            56, 113, 115, 100, 102, 53, 52, 55, 115, 54, 103, 118, 102, 115, 118, 54, 115, 53, 52, 114, 103, 104, 114,
            54, 122, 101, 53, 103, 52, 49, 54, 53
        };


        static AesHelpers()
        {
            Aes = Aes.Create();
            Aes.Padding = PaddingMode.PKCS7;
            Aes.Key = Key;
            Aes.IV = Iv;
        }

        public static byte[] Encrypt(byte[] data)
        {
            if (data == null || data.Length <= 0)
                throw new ArgumentNullException(nameof(data));

            byte[] encrypted;

            using (var msEncrypt = new MemoryStream())
            {
                using (var encryptor = Aes.CreateEncryptor())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(data, 0, data.Length);
                        csEncrypt.FlushFinalBlock();
                    }

                    encrypted = msEncrypt.ToArray();
                }
            }

            return encrypted;
        }

        public static byte[] Decrypt(byte[] data)
        {
            if (data == null || data.Length <= 0)
                throw new ArgumentNullException(nameof(data));

            byte[] decrypted;

            using (var msEncrypt = new MemoryStream())
            {
                using (var decryptor = Aes.CreateDecryptor())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, decryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(data, 0, data.Length);
                        csEncrypt.FlushFinalBlock();
                    }

                    decrypted = msEncrypt.ToArray();
                }
            }

            return decrypted;
        }

        public static byte[] Encrypt(byte[] data, byte[] key, byte[] iv)
        {
            if (data == null || data.Length <= 0)
                throw new ArgumentNullException(nameof(data));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));

            byte[] encrypted;

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(data, 0, data.Length);
                    }

                    encrypted = msEncrypt.ToArray();
                }
            }

            return encrypted;
        }

        public static byte[] Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            if (data == null || data.Length <= 0)
                throw new ArgumentNullException(nameof(data));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));

            byte[] decrypted;

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, decryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(data, 0, data.Length);
                    }

                    decrypted = msEncrypt.ToArray();
                }
            }

            return decrypted;
        }

        public static byte[] EncryptStringToBytes(string plainText)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));

            byte[] encrypted;

            using (var msEncrypt = new MemoryStream())
            {
                using (var encryptor = Aes.CreateEncryptor())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;
        }

        public static string DecryptStringFromBytes(byte[] cipherText)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException(nameof(cipherText));

            string plaintext;

            using (var msDecrypt = new MemoryStream(cipherText))
            {
                using (var decryptor = Aes.CreateDecryptor())
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        public static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));
            byte[] encrypted;
            // Create an Aes object
            // with the specified key and IV.
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        public static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException(nameof(cipherText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));

            // Declare the string used to hold
            // the decrypted text.
            string plaintext;

            // Create an Aes object
            // with the specified key and IV.
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}