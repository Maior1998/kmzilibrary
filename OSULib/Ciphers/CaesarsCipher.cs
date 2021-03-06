﻿using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using OSULib.Maths;
using static OSULib.Maths.Comparison;

namespace OSULib.Ciphers
{
    /// <summary>
    ///     Шифр Цезаря
    /// </summary>
    public static class CaesarsCipher
    {
        /// <summary>
        ///     Возвращает смещенный на заданное расстояние символ.
        /// </summary>
        /// <param name="SourceChar"></param>
        /// <param name="Shift"></param>
        /// <returns></returns>
        public static char GetShiftedChar(char SourceChar, int Shift)
        {
            return Languages.CurrentLanguage.Alphabet[
                (int) new Comparison.LinearComparison(Languages.CurrentLanguage.Alphabet.IndexOf(SourceChar) + Shift,
                    Languages.CurrentLanguage.Alphabet.Length).A];
        }

        /// <summary>
        ///     Осуществляет шифрование строки алгоритмом Цезаря
        /// </summary>
        /// <param name="Source">Открытый текст, которые необходимо зашифровать</param>
        /// <param name="Key">Ключ, определяющий сдвиг</param>
        /// <returns>Шифртекст - результат применения алгоритма</returns>
        public static string Encrypt(string Source, int Key)
        {
            Source = string.Concat(Regex.Split(Source, @"\W"));
            StringBuilder buffer = new StringBuilder(Source.ToUpper());
            for (int i = 0; i < buffer.Length; i++)
            {
                if (char.IsWhiteSpace(buffer[i])) continue;
                buffer[i] = GetShiftedChar(buffer[i], Key);
            }

            return buffer.ToString();
        }

        /// <summary>
        ///     Осуществляет дешифрование строки алгоритмом Цезаря. Можно с ключом, можно и без.
        /// </summary>
        /// <param name="Source">Шифртекст, который необходимо дешифровать</param>
        /// <param name="Key">
        ///     Ключ, использованный при шифровании. Если не задавать этот параметр, будет произведено дешифрование с
        ///     перебором ключа.
        /// </param>
        /// <returns>
        ///     Если ключ задан - массив из одной строки - результата дешифрования. Если ключ не задан - массив полученных в
        ///     результате перебора ключа строк.
        /// </returns>
        public static string[] Decrypt(string Source, int Key = -1)
        {
            if (Key != -1)
                return new[] {Encrypt(Source, -Key)};

            List<string> answer = new List<string>(Languages.CurrentLanguage.Alphabet.Length);
            for (int i = 1; i <= Languages.CurrentLanguage.Alphabet.Length; i++)
                answer.Add($"(k = {i} ({Languages.CurrentLanguage.Alphabet[i - 1]})) {Encrypt(Source, -i)}");
            return answer.ToArray();
        }
    }
}