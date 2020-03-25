using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KMZILib.Ciphers
{
    /// <summary>
    ///     Шифр Плейфора.
    /// </summary>
    public static class Playfair
    {
        /// <summary>
        ///     Осуществляет кодирование шифром Плейфора. При шифровании удаляет все инородные от языка символы.
        /// </summary>
        /// <param name="source">Открытый текст.</param>
        /// <param name="key">Строка-ключ шифрования.</param>
        /// <param name="language">
        ///     Язык, из которого состоит исходный текст. Если не будет указан, будет определен из первого
        ///     символа исходного текста.
        /// </param>
        /// <returns>Строка - зашифрованное сообщение.</returns>
        public static string Encode(string source, string key, Languages.ALanguage language = null)
        {
            key = key.ToUpper();
            //определяем алфавит
            if (language is null)
                language = Languages.LangByChar(source[0]);
            //очищаем ненужные символы
            source = new Regex("[^" + language.Alphabet + "]").Replace(source.ToUpper(), string.Empty);
            //создаем матрицу
            int matrixHeight = 1;
            while ((int) Math.Pow(matrixHeight, 2) < language.Alphabet.Length) matrixHeight++;
            int matrixWidth = matrixHeight;
            while ((matrixWidth - 1) * (matrixHeight + 1) >= language.Alphabet.Length)
            {
                matrixWidth--;
                matrixHeight++;
            }

            char[] matrix = Enumerable.Repeat(' ', matrixWidth * matrixHeight).ToArray();
            char[] alphabet = key.Distinct().Union(language.AlphabetArray).ToArray();
            alphabet.CopyTo(matrix, 0);
            for (int i = alphabet.Length; i < matrix.Length; i++)
                matrix[i] = (i - alphabet.Length).ToString().Last();
            //вставляем X в биграммы с одинаковыми сиволами
            StringBuilder result = new StringBuilder(source);
            int bufferLength = result.Length;
            for (int i = 0; i < bufferLength - 1; i += 2)
            {
                if (result[i] != result[i + 1]) continue;
                result.Insert(i + 1, language.Frequency.Last());
                bufferLength++;
            }

            //если в последней биграмме только один символ - добавляем в конец X.
            if (result.Length % 2 != 0) result.Append(language.Frequency.Last());
            for (int i = 0; i < result.Length - 1; i += 2)
            {
                int firstPos = Array.IndexOf(matrix, result[i]);
                int firstRow = firstPos / matrixWidth;
                int firstColumn = firstPos % matrixWidth;
                int secondPos = Array.IndexOf(matrix, result[i + 1]);
                int secondRow = secondPos / matrixWidth;
                int secondColumn = secondPos % matrixWidth;
                if (firstRow == secondRow)
                {
                    result[i] = matrix[firstRow * matrixWidth + (firstColumn == matrixWidth - 1 ? 0 : firstColumn + 1)];
                    result[i + 1] =
                        matrix[secondRow * matrixWidth + (secondColumn == matrixWidth - 1 ? 0 : secondColumn + 1)];
                }
                else if (firstColumn == secondColumn)
                {
                    result[i] = matrix[(firstRow == matrixHeight - 1 ? 0 : firstRow + 1) * matrixWidth + firstColumn];
                    result[i + 1] =
                        matrix[(secondRow == matrixHeight - 1 ? 0 : secondRow + 1) * matrixWidth + secondColumn];
                }
                else
                {
                    result[i] = matrix[firstRow * matrixWidth + secondColumn];
                    result[i + 1] = matrix[secondRow * matrixWidth + firstColumn];
                }
            }

            return result.ToString();
        }

        /// <summary>
        ///     Осуществляет декодирование шифра Плейфора.
        /// </summary>
        /// <param name="source">Шифртект, который необходимо расшифровать.</param>
        /// <param name="key">Ключ, при помощи которого необходимо провести дешифровку.</param>
        /// <param name="language">
        ///     Язык, которым нужно оперировать при операциях. Если не будет указан, будет определен из первого
        ///     символа исходного текста.
        /// </param>
        /// <returns>Строка - расшифрованное сообщение.</returns>
        public static string Decode(string source, string key, Languages.ALanguage language = null)
        {
            key = key.ToUpper();
            //определяем алфавит
            if (language is null)
                language = Languages.LangByChar(source[0]);
            //создаем матрицу
            int matrixHeight = 1;
            while ((int) Math.Pow(matrixHeight, 2) < language.Alphabet.Length) matrixHeight++;
            int matrixWidth = matrixHeight;
            while ((matrixWidth - 1) * (matrixHeight + 1) >= language.Alphabet.Length)
            {
                matrixWidth--;
                matrixHeight++;
            }

            char[] matrix = Enumerable.Repeat(' ', matrixWidth * matrixHeight).ToArray();
            char[] alphabet = key.Distinct().Union(language.AlphabetArray).ToArray();
            alphabet.CopyTo(matrix, 0);
            for (int i = alphabet.Length; i < matrix.Length; i++)
                matrix[i] = (i - alphabet.Length).ToString().Last();
            StringBuilder buffer = new StringBuilder(source);

            for (int i = 0; i < buffer.Length - 1; i += 2)
            {
                int firstPos = Array.IndexOf(matrix, buffer[i]);
                int firstRow = firstPos / matrixWidth;
                int firstColumn = firstPos % matrixWidth;
                int secondPos = Array.IndexOf(matrix, buffer[i + 1]);
                int secondRow = secondPos / matrixWidth;
                int secondColumn = secondPos % matrixWidth;
                if (firstRow == secondRow)
                {
                    buffer[i] = matrix[firstRow * matrixWidth + (firstColumn == 0 ? matrixWidth - 1 : firstColumn - 1)];
                    buffer[i + 1] =
                        matrix[secondRow * matrixWidth + (secondColumn == 0 ? matrixWidth - 1 : secondColumn - 1)];
                }
                else if (firstColumn == secondColumn)
                {
                    buffer[i] = matrix[(firstRow == 0 ? matrixHeight - 1 : firstRow - 1) * matrixWidth + firstColumn];
                    buffer[i + 1] =
                        matrix[(secondRow == 0 ? matrixHeight - 1 : secondRow - 1) * matrixWidth + secondColumn];
                }
                else
                {
                    buffer[i] = matrix[firstRow * matrixWidth + secondColumn];
                    buffer[i + 1] = matrix[secondRow * matrixWidth + firstColumn];
                }
            }

            for (int i = buffer.Length - 3; i >= 0; i--)
                if (buffer[i] == buffer[i + 2] && buffer[i + 1] == language.Frequency.Last() && i % 2 == 0)
                    buffer.Remove(i + 1, 1);

            if (buffer[buffer.Length - 1] == language.Frequency.Last()) buffer.Remove(buffer.Length - 1, 1);


            return buffer.ToString().TrimEnd();
        }
    }
}