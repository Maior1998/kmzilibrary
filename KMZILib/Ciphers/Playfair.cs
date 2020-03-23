using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace KMZILib.Ciphers
{
    public static class Playfair
    {
        public static string Encode(string source, string key, Languages.ALanguage language = null)
        {

            //определяем алфавит
            if (language is null)
                language = Languages.LangByChar(source[0]);
            //очищаем ненужные символы
            source = new Regex("[^" + language.Alphabet + "]").Replace(source.ToUpper(), string.Empty);
            //создаем матрицу
            int matrixHeight = 1;
            while ((int)Math.Pow(matrixHeight, 2) < language.Alphabet.Length) matrixHeight++;
            int matrixWidth = matrixHeight;
            while ((matrixWidth - 1) * (matrixHeight + 1) >= language.Alphabet.Length)
            {
                matrixWidth--;
                matrixHeight++;
            }
            char[] matrix = Enumerable.Repeat(' ', matrixWidth * matrixHeight).ToArray();
            key.Distinct().Union(language.AlphabetArray).ToArray().CopyTo(matrix, 0);
            //вставляем X в биграммы с одинаковыми сиволами
            StringBuilder result = new StringBuilder(source);
            int bufferLength = result.Length;
            for (int i = 0; i < bufferLength - 1; i += 2)
            {
                if (result[i] != result[i + 1]) continue;
                result.Insert(i + 1, 'X');
                bufferLength++;
            }
            //если в последней биграмме только один символ - добавляем в конец X.
            if (result.Length % 2 != 0) result.Append('X');
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
                    result[i + 1] = matrix[secondRow * matrixWidth + (secondColumn == matrixWidth - 1 ? 0 : secondColumn + 1)];
                }
                else if (firstColumn == secondColumn)
                {
                    result[i] = matrix[(firstRow == matrixHeight - 1 ? 0 : firstRow + 1) * matrixWidth + firstColumn];
                    result[i + 1] = matrix[(secondRow == matrixHeight - 1 ? 0 : secondRow + 1) * matrixWidth + secondColumn];
                }
                else
                {
                    result[i] = matrix[firstRow * matrixWidth + secondColumn];
                    result[i + 1] = matrix[secondRow * matrixWidth + firstColumn];
                }
            }
            return result.ToString();
        }
    }
}
