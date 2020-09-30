using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSULib.Ciphers
{
    //DKS - double key swap
    /// <summary>
    ///     Шифр двойной перестановки по строковому ключу.
    /// </summary>
    public static class DKS
    {
        /// <summary>
        ///     Осуществляет кодирование шифром двойной перестановки по строковому ключу.
        /// </summary>
        /// <param name="source">Открытый текст.</param>
        /// <param name="key1">Строка- первый ключ шифрования.</param>
        /// <param name="key2">Строка- второй ключ шифрования.</param>
        /// <returns>Строка - зашифрованное сообщение.</returns>
        public static string Encode(string source, string key1, string key2)
        {
            source = source.ToUpper();
            key1 = key1.ToUpper();
            key2 = key2.ToUpper();
            source += string.Concat(Enumerable.Repeat(' ', Math.Max(0, key1.Length * key2.Length - source.Length)));
            int rowscount = source.Length / key1.Length;
            int columnscount = source.Length / key2.Length;
            KeyValuePair<int, char>[] trans2 =
                key1
                    .Select((elem, ind) => new KeyValuePair<int, char>(ind, elem))
                    .OrderBy(pair => pair.Value).ToArray();
            KeyValuePair<int, char>[] trans1 =
                key2
                    .Select((elem, ind) => new KeyValuePair<int, char>(ind, elem))
                    .OrderBy(pair => pair.Value).ToArray();
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < rowscount; i++)
            {
                result.Append(string.Concat(Enumerable.Repeat(' ', key1.Length)));
                for (int j = 0; j < columnscount; j++)
                    result[columnscount * i + j] = source[key1.Length * trans1[i].Key + trans2[j].Key];
            }

            return result.ToString();
        }

        /// <summary>
        ///     Осуществляет декодирование шифра двойной перестановки по строковому ключу.
        /// </summary>
        /// <param name="source">Шифртект, который необходимо расшифровать.</param>
        /// <param name="key1"> Первый ключ, при помощи которого необходимо провести дешифровку.</param>
        /// <param name="key2"> Второй ключ, при помощи которого необходимо провести дешифровку.</param>
        /// <returns>Строка - расшифрованное сообщение.</returns>
        public static string Decode(string source, string key1, string key2)
        {
            source = source.ToUpper();
            key1 = key1.ToUpper();
            key2 = key2.ToUpper();
            int RowCount = source.Length / key1.Length;
            KeyValuePair<int, int>[] columns =
                key1
                    .Select((elem, ind) => new KeyValuePair<int, char>(ind, elem))
                    .OrderBy(pair => pair.Value)
                    .Select((pair, ind) => new KeyValuePair<int, int>(pair.Key, ind))
                    .OrderBy(pair => pair.Key)
                    .ToArray();
            int ColumnsCount = source.Length / key2.Length;
            StringBuilder result = new StringBuilder();
            KeyValuePair<int, int>[] rows =
                key2
                    .Select((elem, ind) => new KeyValuePair<int, char>(ind, elem))
                    .OrderBy(pair => pair.Value)
                    .Select((pair, ind) => new KeyValuePair<int, int>(pair.Key, ind))
                    .OrderBy(pair => pair.Key)
                    .ToArray();
            for (int i = 0; i < RowCount; i++)
            for (int j = 0; j < ColumnsCount; j++)
                result.Append(source[rows[i].Value * key1.Length + columns[j].Value]);
            return result.ToString().TrimEnd();
        }
    }
}