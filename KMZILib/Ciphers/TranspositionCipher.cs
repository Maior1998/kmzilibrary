using System.Collections.Generic;
using System.Linq;
using System.Text;
using static KMZILib.Ciphers.Alphabets;

namespace KMZILib
{
    public static partial class Ciphers
    {
        /// <summary>
        ///     Шифр перестановки
        /// </summary>
        public static class TranspositionCipher
        {
            private static char GetEncryptedChar(char source, IReadOnlyList<byte> key)
            {
                return CurrentLanguage == Language.Russian
                    ? RussianAlphabet[key[RussianAlphabet.IndexOf(source)]]
                    : EnglishAlphabet[key[EnglishAlphabet.IndexOf(source)]];
            }

            private static char GetDecryptedChar(char source, IList<byte> key)
            {
                return CurrentLanguage == Language.Russian
                    ? RussianAlphabet[key.IndexOf((byte) RussianAlphabet.IndexOf(source))]
                    : EnglishAlphabet[key.IndexOf((byte) EnglishAlphabet.IndexOf(source))];
            }

            /// <summary>
            ///     Осуществляет шифрование строки с использованием ключа в виде перестановочной функции, записанной в виде массив
            ///     <see cref="byte" />
            /// </summary>
            /// <param name="Source">Открытый текст, к которому необходимо применить алгоритм шифрования</param>
            /// <param name="Key">Массив, представляющий перестановочную функцию</param>
            /// <returns></returns>
            public static string Encrypt(string Source, byte[] Key)
            {
                StringBuilder buffer = new StringBuilder(Source.ToUpper());
                for (int i = 0; i < buffer.Length; i++)
                {
                    if (char.IsWhiteSpace(buffer[i])) continue;
                    if (CurrentLanguage == Language.Russian &&
                        !RussianAlphabet.Contains(buffer[i]) ||
                        CurrentLanguage == Language.English && !EnglishAlphabet.Contains(buffer[i]))
                    {
                        buffer[i] = '.';
                        continue;
                    }

                    buffer[i] = GetEncryptedChar(buffer[i], Key);
                }

                buffer = buffer.Replace('.', ' ');
                string answer = buffer.ToString();
                while (answer.Contains("  "))
                    answer = answer.Replace("  ", " ");
                return answer;
            }

            /// <summary>
            ///     Осуществляет дешифрование строки с использованием ключа в виде перестановочной функции, записанной в виде массив
            ///     <see cref="byte" />
            /// </summary>
            /// <param name="Source">Шифртекст, который необходимо дешифровать</param>
            /// <param name="Key">Массив типа <see cref="byte" />, представляющий собой перестановочную функцию</param>
            /// <returns>Строка - результат дешифрования</returns>
            public static string Decrypt(string Source, byte[] Key)
            {
                StringBuilder buffer = new StringBuilder(Source.ToUpper());
                for (int i = 0; i < buffer.Length; i++)
                {
                    if (!CurrentAlphabet.Contains(buffer[i]))
                    {
                        buffer[i] = '.';
                        continue;
                    }

                    buffer[i] = GetDecryptedChar(buffer[i], Key);
                }

                buffer = buffer.Replace('.', ' ');
                string answer = buffer.ToString();
                while (answer.Contains("  "))
                    answer = answer.Replace("  ", " ");
                return answer;
            }
        }
    }
}