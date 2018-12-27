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
        ///     Получает процентную статистику по всем буквам, входящим в заданную упорядоченную последовательность
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static string GetStatistic(IOrderedEnumerable<KeyValuePair<char, double>> arr)
        {
            StringBuilder Answer = new StringBuilder();

            int i = 0;
            foreach (KeyValuePair<char, double> stat in arr)
                Answer.Append('{' + $"{stat.Key} - {stat.Value}" + '}' + $" ({Ciphers.Languages.CurrentLanguage.Frequency[i++]} ?)\n");

            return Answer.ToString();
        }

        /// <summary>
        ///     Возвращает строковое представление частотного анализа для всех слов длины 2 в заданной строке
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string GetStatisticWb2L(string Text)
        {
            Text = Text.ToUpper();
            Regex bigrams = new Regex(@"\b\w{2}\b");
            Dictionary<string, int> statistic = new Dictionary<string, int>();
            foreach (Match match in bigrams.Matches(Text))
                if (statistic.ContainsKey(match.Value))
                    statistic[match.Value]++;
                else
                    statistic.Add(match.Value, 1);

            StringBuilder answer = new StringBuilder();
            int i = 0;
            foreach (KeyValuePair<string, int> pair in statistic.OrderByDescending(pair => pair.Value))
            {
                if (i == Ciphers.Languages.CurrentLanguage.FrequencyWb2L.Count) break;
                answer.Append($"{pair.Key} - ({pair.Value}) ({Ciphers.Languages.CurrentLanguage.FrequencyWb2L[i++]}?)\n");
            }

            return answer.ToString();
        }

        /// <summary>
        ///     Возвращает строковое представление частотного анализа для всех слов длины 1 в заданной строке
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string GetStatisticWb1L(string Text)
        {
            Text = Text.ToUpper();
            Regex bigrams = new Regex(@"\b\w\b");
            Dictionary<string, int> statistic = new Dictionary<string, int>();
            foreach (Match match in bigrams.Matches(Text))
                if (statistic.ContainsKey(match.Value))
                    statistic[match.Value]++;
                else
                    statistic.Add(match.Value, 1);

            StringBuilder answer = new StringBuilder();
            int i = 0;
            foreach (KeyValuePair<string, int> pair in statistic.OrderByDescending(pair => pair.Value))
            {
                if (i == Ciphers.Languages.CurrentLanguage.FrequencyWb1L.Count) break;
                answer.Append($"{pair.Key} - ({pair.Value}) ({Ciphers.Languages.CurrentLanguage.FrequencyWb1L[i++]}?)\n");
            }

            return answer.ToString();
        }

        /// <summary>
        ///     Возвращает строковое представление частотного анализа для всех последовательностей букв 3 в заданной строке
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string GetStatisticThreegram(string Text)
        {
            Text = Text.ToUpper();
            Dictionary<string, int> statistic = new Dictionary<string, int>();
            int i;
            for (i = 0; i < Text.Length - 2; i++)
                
            {
                if (Ciphers.Languages.CurrentLanguage.Alphabet.IndexOf(Text[i]) == -1 ||
                    Ciphers.Languages.CurrentLanguage.Alphabet.IndexOf(Text[i + 1]) == -1 ||
                    Ciphers.Languages.CurrentLanguage.Alphabet.IndexOf(Text[i + 2]) == -1) continue;
                string buffer = Text[i] + Text[i + 1].ToString() + Text[i + 2];
                if (statistic.ContainsKey(buffer)) statistic[buffer]++;
                else
                    statistic.Add(buffer, 1);
            }

            StringBuilder answer = new StringBuilder();
            statistic = statistic.OrderByDescending(pair => pair.Value)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
            i = 0;
            foreach (string key in statistic.Keys)
            {
                if (i >= Ciphers.Languages.CurrentLanguage.FrequencyThreegrams.Count) break;
                answer.Append($"{key} - ({statistic[key]}) ({Ciphers.Languages.CurrentLanguage.FrequencyThreegrams[i++]}?)\n");
            }

            return answer.ToString();
        }

        /// <summary>
        ///     Возвращает строковое представление частотного анализа для всех последовательностей букв 2 в заданной строке
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string GetStatisticBigram(string Text)
        {
            Text = Text.ToUpper();
            Dictionary<string, int> statistic = new Dictionary<string, int>();
            int i;
            for (i = 0; i < Text.Length - 1; i++)

            {
                if (Ciphers.Languages.CurrentLanguage.Alphabet.IndexOf(Text[i]) == -1 ||
                    Ciphers.Languages.CurrentLanguage.Alphabet.IndexOf(Text[i + 1]) == -1) continue;
                string buffer = Text[i] + Text[i + 1].ToString();
                if (statistic.ContainsKey(buffer)) statistic[buffer]++;
                else
                    statistic.Add(buffer, 1);
            }

            StringBuilder answer = new StringBuilder();
            i = 0;
            foreach (KeyValuePair<string, int> pair in statistic.OrderByDescending(pair => pair.Value))
            {
                if (i == Ciphers.Languages.CurrentLanguage.FrequencyBigrams.Count) break;
                answer.Append($"{pair.Key} - ({pair.Value}) ({Ciphers.Languages.CurrentLanguage.FrequencyBigrams[i++]}?)\n");
            }

            return answer.ToString();
        }
    }
}