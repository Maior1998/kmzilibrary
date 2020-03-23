using System.Linq;
using System.Text;

namespace KMZILib.Ciphers
{
    //SIC - Simple Permutation Cipher
    /// <summary>
    ///     Шифр простой перестановки.
    /// </summary>
    public static class SIC
    {
        /// <summary>
        ///     Осуществляет кодирование шифром простой перестановки.
        /// </summary>
        /// <param name="source">Открытый текст.</param>
        /// <param name="key">Массив-ключ шифрования.</param>
        /// <returns>Строка - зашифрованное сообщение.</returns>
        public static string Encode(string source, int[] key)
        {
            source += string.Concat(Enumerable.Repeat(" ", (key.Length - source.Length % key.Length) % key.Length));

            StringBuilder result = new StringBuilder();
            for (int i = 0; i < source.Length / key.Length; i++)
            {
                int multiplier = i;
                result.Append(string.Concat(key.Select(elem => source[elem - 1 + key.Length * multiplier])));
            }

            return result.ToString();
        }

        /// <summary>
        ///     Осуществляет декодирование шифра простой перестановки.
        /// </summary>
        /// <param name="source">Шифртект, который необходимо расшифровать.</param>
        /// <param name="key">Ключ, при помощи которого необходимо провести дешифровку.</param>
        /// <returns>Строка - расшифрованное сообщение.</returns>
        public static string Decode(string source, int[] key)
        {
            StringBuilder result = new StringBuilder(string.Concat(Enumerable.Repeat(' ', source.Length)));
            for (int i = 0; i < source.Length / key.Length; i++)
            for (int j = 0; j < key.Length; j++)
                result[key[j] + i * key.Length - 1] = source[j + i * key.Length];
            return result.ToString().TrimEnd();
        }
    }
}