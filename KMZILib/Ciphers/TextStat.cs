using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KMZILib
{
    /// <summary>
    ///     Статический класс для работы с частотным анализом
    /// </summary>
    public static class TextStat
    {
        /// <summary>
        /// Возвращает результат статистического анализа для каждой буквы, входящей в 
        /// </summary>
        /// <param name="SourceText"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<KeyValuePair<char, int>> GetStatisticOnegram(string SourceText)
        {
            SourceText = SourceText.ToUpper();
            Dictionary<char, int> Result = Ciphers.Languages.CurrentLanguage.AlphabetArray.ToDictionary<char, char, int>(letter => letter, letter => 0);
            foreach (char symbol in SourceText)
            {
                if (!Ciphers.Languages.CurrentLanguage.Alphabet.Contains(symbol))
                    continue;
                Result[symbol]++;
            }
            return Result.OrderByDescending(a => a.Value);
        }

        /// <summary>
        ///     Возвращает строковое представление частотного анализа для всех последовательностей букв 2 в заданной строке
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<KeyValuePair<string, int>> GetStatisticBigram(string Text)
        {
            Text = Text.ToUpper();
            Dictionary<string, int> statistic = new Dictionary<string, int>();
            for (int i = 0; i < Text.Length - 1; i++)

            {
                if (Ciphers.Languages.CurrentLanguage.Alphabet.IndexOf(Text[i]) == -1 ||
                    Ciphers.Languages.CurrentLanguage.Alphabet.IndexOf(Text[i + 1]) == -1)
                    continue;
                string buffer = string.Concat(Text[i], Text[i + 1]);
                if (!statistic.ContainsKey(buffer))
                    statistic.Add(buffer, 0);
                statistic[buffer]++;
            }

            return statistic.OrderByDescending(pair => pair.Value);
        }

        /// <summary>
        ///     Возвращает строковое представление частотного анализа для всех последовательностей букв 3 в заданной строке
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<KeyValuePair<string, int>> GetStatisticThreegram(string Text)
        {
            Text = Text.ToUpper();
            Dictionary<string, int> statistic = new Dictionary<string, int>();
            for (int i = 0; i < Text.Length - 2; i++)

            {
                if (Ciphers.Languages.CurrentLanguage.Alphabet.IndexOf(Text[i]) == -1 ||
                    Ciphers.Languages.CurrentLanguage.Alphabet.IndexOf(Text[i + 1]) == -1 ||
                    Ciphers.Languages.CurrentLanguage.Alphabet.IndexOf(Text[i + 2]) == -1)
                    continue;
                string buffer = string.Concat(Text[i], Text[i + 1], Text[i + 1]);
                if (!statistic.ContainsKey(buffer))
                    statistic.Add(buffer, 0);
                statistic[buffer]++;
            }
            return statistic.OrderByDescending(pair => pair.Value);
        }

        /// <summary>
        ///     Возвращает строковое представление частотного анализа для всех слов длины 1 в заданной строке
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<KeyValuePair<string, int>> GetStatisticWb1L(string Text)
        {
            Text = Text.ToUpper();
            Regex bigrams = new Regex(@"\b[" +
                                      Ciphers.Languages.CurrentLanguage.Alphabet +
                                      @"]\b");
            Dictionary<string, int> statistic = new Dictionary<string, int>();
            foreach (Match match in bigrams.Matches(Text))
            {
                if (!statistic.ContainsKey(match.Value))
                    statistic.Add(match.Value, 0);
                statistic[match.Value]++;
            }
            return statistic.OrderByDescending(pair => pair.Value);
        }

        /// <summary>
        ///     Возвращает строковое представление частотного анализа для всех слов длины 2 в заданной строке
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<KeyValuePair<string, int>> GetStatisticWb2L(string Text)
        {
            Text = Text.ToUpper();
            Regex bigrams = new Regex(@"\b[" +
                                      Ciphers.Languages.CurrentLanguage.Alphabet +
                                      @"]{2}\b");
            Dictionary<string, int> statistic = new Dictionary<string, int>();
            foreach (Match match in bigrams.Matches(Text))
            {
                if (!statistic.ContainsKey(match.Value))
                    statistic.Add(match.Value, 0);
                statistic[match.Value]++;
            }
            return statistic.OrderByDescending(pair => pair.Value);
        }

        /// <summary>
        ///     Возвращает строковое представление частотного анализа для всех слов длины 3 в заданной строке
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<KeyValuePair<string, int>> GetStatisticWb3L(string Text)
        {
            Text = Text.ToUpper();
            Regex bigrams = new Regex(@"\b[" +
                                      Ciphers.Languages.CurrentLanguage.Alphabet +
                                      @"]{3}\b");
            Dictionary<string, int> statistic = new Dictionary<string, int>();
            foreach (Match match in bigrams.Matches(Text))
            {
                if (!statistic.ContainsKey(match.Value))
                    statistic.Add(match.Value, 0);
                statistic[match.Value]++;
            }
            return statistic.OrderByDescending(pair => pair.Value);
        }
    }
}