using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KMZILib
{
    public static partial class Ciphers
    {
        /// <summary>
        /// Шифр Цезаря
        /// </summary>
        public static class CaesarsCipher
        {

            private static char GetShiftedChar(char SourceChar, int Shift)
            {
                return Alphabets.CurrentAlphabet[(int)new Comparison.LinearComparison(Alphabets.CurrentAlphabet.IndexOf(SourceChar) + Shift,Alphabets.CurrentAlphabet.Length).A];
            }

            /// <summary>
            /// Осуществляет шифрование строки алгоритмом Цезаря
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
            /// Осуществляет дешифрование строки алгоритмом Цезаря. Можно с ключом, можно и без.
            /// </summary>
            /// <param name="Source">Шифртекст, который необходимо дешифровать</param>
            /// <param name="Key">Ключ, использованный при шифровании. Если не задавать этот параметр, будет произведено дешифрование с перебором ключа.</param>
            /// <returns>Если ключ задан - массив из одной строки - результата дешифрования. Если ключ не задан - массив полученных в результате перебора ключа строк.</returns>
            public static string[] Decrypt(string Source, int Key = -1)
            {
                if (Key != -1)
                    return new[] {Encrypt(Source, -Key)};

                List<string> answer = new List<string>(Alphabets.CurrentAlphabet.Length);
                    for (int i = 1; i <= Alphabets.CurrentAlphabet.Length; i++)
                        answer.Add($"(k = {i}) {Encrypt(Source, -i)}");
                    return answer.ToArray();
            }
        }
    }
}