using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMZILib.Ciphers
{
    /// <summary>
    ///     Шифр простой перестановки по строковому ключу.
    /// </summary>
    public static class SKS
    {
        /// <summary>
        ///     Осуществляет кодирование шифром простой перестановки по строковому ключу.
        /// </summary>
        /// <param name="source">Открытый текст.</param>
        /// <param name="key">Строка-ключ шифрования.</param>
        /// <returns>Строка - зашифрованное сообщение.</returns>
        public static string Encode(string source, string key)
        {
            source += string.Concat(Enumerable.Repeat(" ", (key.Length - source.Length % key.Length) % key.Length));
            int RowCount = source.Length / key.Length;
            KeyValuePair<int, char>[] columns =
                key
                    .Select((elem, ind) => new KeyValuePair<int, char>(ind, elem))
                    .OrderBy(pair => pair.Value).ToArray();
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < RowCount; i++)
            for (int j = 0; j < key.Length; j++)
                result.Append(source[RowCount * columns[j].Key + i]);
            return result.ToString();
        }

        /// <summary>
        ///     Осуществляет декодирование шифра простой перестановки по строковому ключу.
        /// </summary>
        /// <param name="source">Шифртект, который необходимо расшифровать.</param>
        /// <param name="key">Ключ, при помощи которого необходимо провести дешифровку.</param>
        /// <returns>Строка - расшифрованное сообщение.</returns>
        public static string Decode(string source, string key)
        {
            int RowCount = source.Length / key.Length;
            StringBuilder result = new StringBuilder();
            KeyValuePair<int, int>[] columns =
                key
                    .Select((elem, ind) => new KeyValuePair<int, char>(ind, elem))
                    .OrderBy(pair => pair.Value)
                    .Select((pair, ind) => new KeyValuePair<int, int>(pair.Key, ind))
                    .OrderBy(pair => pair.Key)
                    .ToArray();
            for (int i = 0; i < key.Length; i++)
            for (int j = 0; j < RowCount; j++)
                result.Append(source[j * key.Length + columns[i].Value]);
            return result.ToString().TrimEnd();
        }
    }
}