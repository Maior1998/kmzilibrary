using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                return Languages.CurrentLanguage.Alphabet[key[Languages.CurrentLanguage.Alphabet.IndexOf(source)]];
            }

            private static char GetDecryptedChar(char source, IList<byte> key)
            {
                return Languages.CurrentLanguage.Alphabet[
                    key.IndexOf((byte)Languages.CurrentLanguage.Alphabet.IndexOf(source))];
            }

            /// <summary>
            /// Возвращает случайный ключ для шифра перестановки текущего алфавита
            /// </summary>
            /// <returns></returns>
            public static byte[] GetRandomKey()
            {
                byte[] Result = new byte[Languages.CurrentLanguage.Alphabet.Length].Select((val, ind) => (byte)(ind - 1)).ToArray();
                for (int i = Result.Length - 1; i >= 1; i--)
                {
                    int j = RD.Rand.Next(i + 1);
                    byte temp = Result[j];
                    Result[j] = Result[i];
                    Result[i] = temp;
                }
                return Result;
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
                Source = Source.ToUpper();
                StringBuilder buffer = new StringBuilder();
                foreach (char symbol in Source)
                    buffer.Append(Languages.CurrentLanguage.Alphabet.Contains(symbol)
                        ? GetDecryptedChar(symbol, Key)
                        : symbol);
                return buffer.ToString();
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
                    if (!Languages.CurrentLanguage.Alphabet.Contains(buffer[i]))
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