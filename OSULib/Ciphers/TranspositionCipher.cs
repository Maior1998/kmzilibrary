using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using OSULib.Misc;

namespace OSULib.Ciphers
{
    /// <summary>
    ///     Класс, представляющий методы для работы с шифром перестановки алфавита.
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
                key.IndexOf((byte) Languages.CurrentLanguage.Alphabet.IndexOf(source))];
        }

        /// <summary>
        ///     Возвращает случайный ключ для шифра перестановки текущего алфавита.
        /// </summary>
        /// <returns></returns>
        public static TCK GetRandomKey()
        {
            byte[] Result = new byte[Languages.CurrentLanguage.Alphabet.Length].Select((val, ind) => (byte) ind)
                .ToArray();
            for (int i = Result.Length - 1; i >= 1; i--)
            {
                int j = RD.Rand.Next(i + 1);
                byte temp = Result[j];
                Result[j] = Result[i];
                Result[i] = temp;
            }

            return new TCK(Result);
        }

        /// <summary>
        ///     Осуществляет шифрование строки с использованием ключа в виде перестановочной функции, записанной в виде массив
        ///     <see cref="byte" />
        /// </summary>
        /// <param name="Source">Открытый текст, к которому необходимо применить алгоритм шифрования</param>
        /// <param name="EncryptKey">Массив, представляющий перестановочную функцию</param>
        /// <returns></returns>
        public static string Encrypt(string Source, TCK EncryptKey)
        {
            Source = Source.ToUpper();
            StringBuilder buffer = new StringBuilder();
            foreach (char symbol in Source)
                buffer.Append(Languages.CurrentLanguage.Alphabet.Contains(symbol)
                    ? GetEncryptedChar(symbol, EncryptKey.Key)
                    : symbol);
            return buffer.ToString();
        }

        /// <summary>
        ///     Осуществляет дешифрование строки с использованием ключа в виде перестановочной функции, записанной в виде массив
        ///     <see cref="byte" />
        /// </summary>
        /// <param name="Source">Шифртекст, который необходимо дешифровать</param>
        /// <param name="DecryptKey">Массив типа <see cref="byte" />, представляющий собой перестановочную функцию</param>
        /// <returns>Строка - результат дешифрования</returns>
        public static string Decrypt(string Source, TCK DecryptKey)
        {
            Source = Source.ToUpper();
            StringBuilder buffer = new StringBuilder();
            foreach (char symbol in Source)
                buffer.Append(Languages.CurrentLanguage.Alphabet.Contains(symbol)
                    ? GetDecryptedChar(symbol, DecryptKey.Key)
                    : symbol);
            return buffer.ToString();
        }

        /// <summary>
        ///     Представляет класс для обработки и хранения ключа шифра перестановки алфавита (Transposition Cipher Key).
        /// </summary>
        public class TCK
        {
            /// <summary>
            ///     Инициализация нового ключа по имеющемуся массиву байт.
            /// </summary>
            /// <param name="ArrKey"></param>
            public TCK(byte[] ArrKey)
            {
                if (ArrKey.Length != Languages.CurrentLanguage.Alphabet.Length)
                    throw new InvalidOperationException("Размер ключа не совпадает с длиной алфавита.");
                Key = new byte[ArrKey.Length];
                ArrKey.CopyTo(Key, 0);
            }

            /// <summary>
            ///     Инициализация нового ключа при помощи его строкового представления
            /// </summary>
            /// <param name="TextKey"></param>
            public TCK(string TextKey)
            {
                //TODO: сделать проверку на повторяющиеся буквы
                string[] Rows = TextKey.ToUpper().Split('\n');
                if (Rows.Length != Languages.CurrentLanguage.Alphabet.Length)
                    throw new InvalidOperationException("Размер ключа не совпадает с длиной алфавита.");
                Regex KeyRegex = new Regex(string.Concat(@"\s*([", Languages.CurrentLanguage.Alphabet, @"])\s*->\s*([",
                    Languages.CurrentLanguage.Alphabet, @"])\n?"));
                SortedList<byte, byte> TranspositionMatrixSorted = new SortedList<byte, byte>();
                foreach (string Row in Rows)
                {
                    if (!KeyRegex.IsMatch(Row))
                        throw new InvalidOperationException("Ошибка распознавания ключа.");
                    Match match = KeyRegex.Match(Row);
                    TranspositionMatrixSorted.Add(
                        (byte) Languages.CurrentLanguage.Alphabet.IndexOf(match.Groups[1].Value,
                            StringComparison.Ordinal),
                        (byte) Languages.CurrentLanguage.Alphabet.IndexOf(match.Groups[2].Value,
                            StringComparison.Ordinal));
                }

                Key = TranspositionMatrixSorted.Values.ToArray();
            }

            /// <summary>
            ///     Представление ключа в виде массива байт.
            /// </summary>
            public byte[] Key { get; }

            /// <summary>
            ///     Возвращает строковое представление данного ключа.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return string.Join("\n", Languages.CurrentLanguage.Alphabet
                    .Select((t, i) => $"{t} -> {Languages.CurrentLanguage.Alphabet[Key[i]]}"));
            }
        }
    }
}