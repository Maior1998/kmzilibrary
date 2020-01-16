using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KMZILib
{
    public static partial class Ciphers
    {

        public static class AES
        {
            /// <summary>
            /// Generate a private key
            /// From : www.chapleau.info/blog/2011/01/06/usingsimplestringkeywithaes256encryptioninc.html
            /// </summary>
            public static string GenerateKey(int iKeySize)
            {
                RijndaelManaged aesEncryption = new RijndaelManaged();
                aesEncryption.KeySize = iKeySize;
                aesEncryption.BlockSize = 128;
                aesEncryption.Mode = CipherMode.CBC;
                aesEncryption.Padding = PaddingMode.PKCS7;
                aesEncryption.GenerateIV();
                string ivStr = Convert.ToBase64String(aesEncryption.IV);
                aesEncryption.GenerateKey();
                string keyStr = Convert.ToBase64String(aesEncryption.Key);
                string completeKey = ivStr + "," + keyStr;

                return Convert.ToBase64String(Encoding.UTF8.GetBytes(completeKey));
            }

            /// <summary>
            /// Encrypt
            /// From : www.chapleau.info/blog/2011/01/06/usingsimplestringkeywithaes256encryptioninc.html
            /// </summary>
            public static string Encrypt(string iPlainStr, string iCompleteEncodedKey, int iKeySize)
            {
                RijndaelManaged aesEncryption = new RijndaelManaged();
                aesEncryption.KeySize = iKeySize;
                aesEncryption.BlockSize = 128;
                aesEncryption.Mode = CipherMode.CBC;
                aesEncryption.Padding = PaddingMode.PKCS7;
                aesEncryption.IV = Convert.FromBase64String(Encoding.UTF8.GetString(Convert.FromBase64String(iCompleteEncodedKey)).Split(',')[0]);
                aesEncryption.Key = Convert.FromBase64String(Encoding.UTF8.GetString(Convert.FromBase64String(iCompleteEncodedKey)).Split(',')[1]);
                byte[] plainText = Encoding.UTF8.GetBytes(iPlainStr);
                ICryptoTransform crypto = aesEncryption.CreateEncryptor();
                byte[] cipherText = crypto.TransformFinalBlock(plainText, 0, plainText.Length);
                return Convert.ToBase64String(cipherText);
            }

            /// <summary>
            /// Decrypt
            /// From : www.chapleau.info/blog/2011/01/06/usingsimplestringkeywithaes256encryptioninc.html
            /// </summary>
            public static string Decrypt(string iEncryptedText, string iCompleteEncodedKey, int iKeySize)
            {
                RijndaelManaged aesEncryption = new RijndaelManaged
                {
                    KeySize = iKeySize,
                    BlockSize = 128,
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7,
                    IV = Convert.FromBase64String(Encoding.UTF8
                        .GetString(Convert.FromBase64String(iCompleteEncodedKey)).Split(',')[0]),
                    Key = Convert.FromBase64String(Encoding.UTF8
                        .GetString(Convert.FromBase64String(iCompleteEncodedKey)).Split(',')[1])
                };
                ICryptoTransform decrypto = aesEncryption.CreateDecryptor();
                byte[] encryptedBytes = Convert.FromBase64CharArray(iEncryptedText.ToCharArray(), 0, iEncryptedText.Length);
                return Encoding.UTF8.GetString(decrypto.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length));
            }
        }

    }
}
