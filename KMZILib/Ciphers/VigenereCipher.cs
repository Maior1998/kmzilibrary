using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KMZILib.Ciphers
{
    /// <summary>
    ///     Шифр Виженера
    /// </summary>
    public static class VigenereCipher
    {
        /// <summary>
        ///     Возвращает текст, из которого удаляются все неалфавитные символы.
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static string GetOnlyAlphabetCharacters(string Source)
        {
            return string.Concat(Regex.Split(Source.ToUpper(), @"[^" + Languages.CurrentLanguage.Alphabet + "]",
                RegexOptions.IgnoreCase));
        }

        /// <summary>
        ///     Возвращает таблицу Виженера для данного языка.
        /// </summary>
        /// <param name="Lang"></param>
        /// <returns></returns>
        public static string[] GetVigenereTable(Languages.ALanguage Lang)
        {
            string[] Result = new string[Lang.Alphabet.Length];
            for (int i = 0; i < Lang.Alphabet.Length; i++)
                Result[i] = CaesarsCipher.Encrypt(Lang.Alphabet, i);
            return Result;
        }

        private static char GetEncryptedChar(char SourceChar, char KeyChar)
        {
            int SourceIndex = Languages.CurrentLanguage.Alphabet.IndexOf(SourceChar);
            int KeyIndex = Languages.CurrentLanguage.Alphabet.IndexOf(KeyChar);
            return Languages.CurrentLanguage.Alphabet[
                (SourceIndex + KeyIndex) % Languages.CurrentLanguage.Alphabet.Length];
        }

        /// <summary>
        ///     Возвращает символ, полученный в результате дешифрования символа текста символом ключа.
        /// </summary>
        /// <param name="EncryptedShar"></param>
        /// <param name="KeyChar"></param>
        /// <returns></returns>
        public static char GetDecryptedChar(char EncryptedShar, char KeyChar)
        {
            int SourceIndex = Languages.CurrentLanguage.Alphabet.IndexOf(EncryptedShar);
            int KeyIndex = Languages.CurrentLanguage.Alphabet.IndexOf(KeyChar);
            Comparison.LinearComparison ResultIndex =
                new Comparison.LinearComparison(SourceIndex - KeyIndex, Languages.CurrentLanguage.Alphabet.Length);
            return Languages.CurrentLanguage.Alphabet[(int) ResultIndex.A];
        }


        /// <summary>
        ///     Осуществляет шифрование строки с использованием заданного лозунга
        /// </summary>
        /// <param name="Source">Открытый текст, к которому необходимо применить алгоритм</param>
        /// <param name="Key">Лозунг - ключ, который будет использован при шифровании</param>
        /// <returns>Шифртекст - результат шифрования</returns>
        public static string Encrypt(string Source, string Key)
        {
            StringBuilder buffer = new StringBuilder(GetOnlyAlphabetCharacters(Source));
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = GetEncryptedChar(buffer[i], Key[i % Key.Length]);
            return buffer.ToString();
        }

        /// <summary>
        ///     Осуществляет дешифрование строки с использованием заданого лозунга
        /// </summary>
        /// <param name="EncryptedText">Шифртекст, который необходимо расшифровать</param>
        /// <param name="Key">Лозунг - ключ, который будет использован при дешифровании</param>
        /// <returns>Строка - результат дешифрования</returns>
        public static string Decrypt(string EncryptedText, string Key)
        {
            StringBuilder buffer = new StringBuilder(GetOnlyAlphabetCharacters(EncryptedText));
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = GetDecryptedChar(buffer[i], Key[i % Key.Length]);
            return buffer.ToString();
        }

        /// <summary>
        ///     Представляет методы для работы теста Касиски (KasiskiExamination)
        /// </summary>
        public static class KE
        {
            /// <summary>
            ///     Анализирует заданный текст и возвращает массив гипотез о возможных длинах ключа
            /// </summary>
            /// <param name="CipherText"></param>
            /// <returns></returns>
            public static int[] GetKEHypotheses(string CipherText)
            {
                Dictionary<int, int> PossibleKeyLengthes = new Dictionary<int, int>();
                CipherText = GetOnlyAlphabetCharacters(CipherText);
                for (int Length = 3; Length < 6; Length++)
                    //Length - переменная, которая отвечает за текущую исследуемую длину подстрок.
                for (int SubstringIndex = 0; SubstringIndex <= CipherText.Length - Length; SubstringIndex++)
                {
                    //Теперь мы выбираем подстроки длины Length начиная с индекса SubstringIndex.
                    string CurrentSubstring = CipherText.Substring(SubstringIndex, Length);
                    //Если такая подстрока уже была
                    //(то есть индекс первого ее вхождения меньше чем SubstringIndex)
                    //То пропускаем ее
                    if (CipherText.IndexOf(CurrentSubstring) < SubstringIndex)
                        continue;

                    //Собрали все индексы таких подстрок.
                    int[] Indexes = new Regex(CurrentSubstring).Matches(CipherText).Cast<Match>()
                        .Select(match => match.Index).ToArray();

                    //Теперь пробегаем по этому массиву и находим разности между каждыми
                    //Двумя индексами.
                    for (int i = 0; i < Indexes.Length - 1; i++)
                    for (int j = i + 1; j < Indexes.Length; j++)
                    {
                        int CurIndex = Indexes[j] - Indexes[i];
                        //Необходимо найти список делителей этого числа.
                        int[] dividers = GetUniqueNumberDividersF(CurIndex);
                        //Полученный список помещаем в словарь
                        foreach (int Divider in dividers)
                        {
                            if (!PossibleKeyLengthes.ContainsKey(Divider))
                                PossibleKeyLengthes.Add(Divider, 0);
                            PossibleKeyLengthes[Divider]++;
                        }
                    }
                }

                return PossibleKeyLengthes
                    .OrderByDescending(pair => pair.Value)
                    .Take(10)
                    .Select(pair => pair.Key)
                    .ToArray();
            }

            /// <summary>
            ///     Получает набор уникальных делителей числа при помощи факторизации и возвращает в виде массива.
            /// </summary>
            /// <param name="Number" />
            /// <returns></returns>
            public static int[] GetUniqueNumberDividersF(int Number)
            {
                if (Number < 0)
                    Number *= -1;
                List<int> dividers = new List<int>();
                int i = 1;
                int Limit = Number / 2;
                while (++i <= Limit)
                    if (Number % i == 0)
                        dividers.Add(i);
                dividers.Add(Number);
                return dividers.ToArray();
            }

            /// <summary>
            ///     Осуществляет разбиение теста на отрывки, которые участвовали в процессе шифрования с одной и той же буквой ключа
            /// </summary>
            /// <param name="Source">Шифр-текст</param>
            /// <param name="KeyLength">Длина предполагаемого ключа</param>
            /// <returns>Массив строк - фрагментов, которые учавствовали в процессе шифрования с одной и той же буквой ключа</returns>
            public static string[] GetStringFragmentsFromKeyLength(string Source, int KeyLength)
            {
                Source = string.Concat(Regex.Split(Source, @"\W"));
                if (KeyLength <= 1)
                    return new[] {Source};
                StringBuilder[] Result = new StringBuilder[KeyLength].Select(sb => new StringBuilder()).ToArray();
                //Проинициализировали все StringBuilder'ы

                for (int i = 0; i < Source.Length; i++)
                    Result[i % KeyLength].Append(Source[i]);
                //разобрали текст по StringBuilder'ам.

                return Result.Select(sb => sb.ToString()).ToArray();
            }

            /// <summary>
            ///     Получает цельую строку из фрагментов, полученных в <see cref="GetStringFragmentsFromKeyLength" />
            /// </summary>
            /// <param name="Fragments">Фрагменты строки, полученные в <see cref="GetStringFragmentsFromKeyLength" /></param>
            /// <returns>Цельная строка исходного текста</returns>
            public static string GetStringFromTextFragments(string[] Fragments)
            {
                StringBuilder Result = new StringBuilder();
                int Length = Fragments.Aggregate(0, (sum, frag) => sum + frag.Length);
                for (int i = 0; i < Length; i++)
                    Result.Append(Fragments[i % Fragments.Length][i / Fragments.Length]);
                return Result.ToString();
            }
        }
    }
}