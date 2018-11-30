using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KMZILib
{
    /// <summary>
    /// Статический класс для работа с частотным анализом
    /// </summary>
    public static class TextStat
    {
        /// <summary>
        /// Получает процентную статистику по всем буквам, входящим в заданную упорядоченную последовательность
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static string GetStatistic(IOrderedEnumerable<KeyValuePair<char, double>> arr)
        {
            StringBuilder Answer = new StringBuilder();

            int i = 0;
            foreach (KeyValuePair<char, double> stat in arr)
            {
                Answer.Append('{' + $"{stat.Key} - {stat.Value}" + '}'+$" ({CurrentFrequencyList[i++]} ?)\n");
            }

            return Answer.ToString();
        }

        /// <summary>
        /// Возвращает строковое представление частотного анализа для всех слов длины 2 в заданной строке
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string GetStatisticWb2L(string Text)
        {
            Text = Text.ToUpper();
            Regex bigrams = new Regex(@"\b\w{2}\b");
            Dictionary<string, int> statistic = new Dictionary<string, int>();
            foreach (Match match in bigrams.Matches(Text))
            {
                if (statistic.ContainsKey(match.Value)) statistic[match.Value]++;
                else
                    statistic.Add(match.Value, 1);
            }

            StringBuilder answer = new StringBuilder();
            int i = 0;
            foreach (KeyValuePair<string, int> pair in statistic.OrderByDescending(pair => pair.Value))
            {
                if (i == CurrentFrequencyWb2L.Count) break;
                answer.Append($"{pair.Key} - ({pair.Value}) ({CurrentFrequencyWb2L[i++]}?)\n");
            }

            return answer.ToString();
        }

        /// <summary>
        /// Возвращает строковое представление частотного анализа для всех слов длины 1 в заданной строке
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string GetStatisticWb1L(string Text)
        {
            Text = Text.ToUpper();
            Regex bigrams = new Regex(@"\b\w\b");
            Dictionary<string, int> statistic = new Dictionary<string, int>();
            foreach (Match match in bigrams.Matches(Text))
            {
                if (statistic.ContainsKey(match.Value)) statistic[match.Value]++;
                else
                    statistic.Add(match.Value, 1);
            }

            StringBuilder answer = new StringBuilder();
            int i = 0;
            foreach (KeyValuePair<string, int> pair in statistic.OrderByDescending(pair => pair.Value))
            {
                if (i == CurrentFrequencyWb1L.Count) break;
                answer.Append($"{pair.Key} - ({pair.Value}) ({CurrentFrequencyWb1L[i++]}?)\n");
            }

            return answer.ToString();
        }

        /// <summary>
        /// Возвращает строковое представление частотного анализа для всех последовательностей букв 3 в заданной строке
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
                if (Ciphers.Alphabets.CurrentAlphabet.IndexOf(Text[i]) == -1 || 
                    Ciphers.Alphabets.CurrentAlphabet.IndexOf(Text[i + 1]) == -1||
                    Ciphers.Alphabets.CurrentAlphabet.IndexOf(Text[i + 2]) == -1) continue;
                string buffer = Text[i].ToString() + Text[i + 1].ToString() + Text[i+2].ToString();
                if (statistic.ContainsKey(buffer)) statistic[buffer]++;
                else
                    statistic.Add(buffer, 1);
            }
            StringBuilder answer = new StringBuilder();
            statistic = statistic.OrderByDescending(pair => pair.Value).ToDictionary(pair=>pair.Key,pair=>pair.Value);
            i = 0;
            foreach (string key in statistic.Keys)
            {
                if (i >= CurrentFrequencyThreegrams.Count) break;
                answer.Append($"{key} - ({statistic[key]}) ({CurrentFrequencyThreegrams[i++]}?)\n");
            }
            return answer.ToString();
        }

        /// <summary>
        /// Возвращает строковое представление частотного анализа для всех последовательностей букв 2 в заданной строке
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
                if (Ciphers.Alphabets.CurrentAlphabet.IndexOf(Text[i]) == -1 || Ciphers.Alphabets.CurrentAlphabet.IndexOf(Text[i + 1]) == -1) continue;
                string buffer = Text[i].ToString() + Text[i + 1].ToString();
                if (statistic.ContainsKey(buffer)) statistic[buffer]++;
                else
                    statistic.Add(buffer, 1);
            }

            StringBuilder answer=new StringBuilder();
            i = 0;
            foreach (KeyValuePair<string, int> pair in statistic.OrderByDescending(pair => pair.Value))
            {
                if (i == CurrentFrequencyBigrams.Count) break;
                answer.Append($"{pair.Key} - ({pair.Value}) ({CurrentFrequencyBigrams[i++]}?)\n");
            }

            return answer.ToString();
        }

        /// <summary>
        /// Возвращает список частот для текущего выбранного языка <see cref="Ciphers.Alphabets.CurrentLanguage"/>
        /// </summary>
        public static List<char> CurrentFrequencyList
        {
            get
            {
                switch (Ciphers.Alphabets.CurrentLanguage)
                {
                    case Ciphers.Language.Russian:
                        return FrequencyRus;
                    case Ciphers.Language.English:
                        return FrequencyEng;
                    default:
                        return FrequencyRus;
                }
            }

        }

        /// <summary>
        /// Возвращает список частот всех однобуквенных слов для текущего языка <see cref="Ciphers.Alphabets.CurrentLanguage"/>
        /// </summary>
        public static List<char> CurrentFrequencyWb1L
        {
            get
            {
                switch (Ciphers.Alphabets.CurrentLanguage)
                {
                    case Ciphers.Language.Russian:
                        return FrequencyWb1LRUS;
                    case Ciphers.Language.English:
                        return FrequencyWb1LENG;
                    default:
                        return FrequencyWb1LRUS;
                }
            }

        }

        /// <summary>
        /// Возвращает список частот всех слов длины 2 для текущего языка <see cref="Ciphers.Alphabets.CurrentLanguage"/>
        /// </summary>
        public static List<string> CurrentFrequencyWb2L
        {
            get
            {
                switch (Ciphers.Alphabets.CurrentLanguage)
                {
                    case Ciphers.Language.Russian:
                        return FrequencyWb2LRUS;
                    case Ciphers.Language.English:
                        return FrequencyWb2LENG;
                    default:
                        return FrequencyWb2LRUS;
                }
            }

        }

        /// <summary>
        /// Возвращает список частот всех последовательностей букв длины 2 для текущего языка <see cref="Ciphers.Alphabets.CurrentLanguage"/>
        /// </summary>
        public static List<string> CurrentFrequencyBigrams
        {
            get
            {
                switch (Ciphers.Alphabets.CurrentLanguage)
                {
                    case Ciphers.Language.Russian:
                        return FrequencyBigramsRUS;
                    case Ciphers.Language.English:
                        return FrequencyBigramsENG;
                    default:
                        return FrequencyBigramsRUS;
                }
            }

        }

        /// <summary>
        /// Возвращает список частот всех последовательностей букв длины 3 для текущего языка <see cref="Ciphers.Alphabets.CurrentLanguage"/>
        /// </summary>
        public static List<string> CurrentFrequencyThreegrams
        {
            get
            {
                switch (Ciphers.Alphabets.CurrentLanguage)
                {
                    case Ciphers.Language.Russian:
                        return FrequencyThreegramsRUS;
                    case Ciphers.Language.English:
                        return FrequencyThreegramsENG;
                    default:
                        return FrequencyThreegramsRUS;
                }
            }

        }

        /// <summary>
        /// Упорядоченный по убыванию список самых частых букв русского языка <see cref="Ciphers.Language.Russian"/>
        /// </summary>
        public static List<char> FrequencyRus = new List<char>()
        {
            'О',
            'Е',
            'А',
            'И',
            'Н',
            'Т',
            'С',
            'Р',
            'В',
            'Л',
            'К',
            'М',
            'Д',
            'П',
            'У',
            'Я',
            'Ы',
            'Ь',
            'Г',
            'З',
            'Б',
            'Ч',
            'Й',
            'Х',
            'Ж',
            'Ш',
            'Ю',
            'Ц',
            'Щ',
            'Э',
            'Ф',
            'Ъ'
        };

        /// <summary>
        /// Упорядоченный по убыванию список самых частых букв английского языка <see cref="Ciphers.Language.English"/>
        /// </summary>
        public static List<char> FrequencyEng = new List<char>()
        {
            'E',
            'T',
            'A',
            'O',
            'I',
            'N',
            'S',
            'R',
            'H',
            'L',
            'D',
            'C',
            'U',
            'M',
            'F',
            'P',
            'G',
            'W',
            'Y',
            'B',
            'V',
            'K',
            'X',
            'J',
            'Q',
            'Z'
        };

        /// <summary>
        /// Упорядоченный по убыванию список самых частых однобуквенных слов русского языка <see cref="Ciphers.Language.Russian"/>
        /// </summary>
        public static List<char> FrequencyWb1LRUS=new List<char>()
        {
            'И',
            'В',
            'Я',
            'С',
            'А',
            'К',
            'У'
        };

        /// <summary>
        /// Упорядоченный по убыванию список самых частых слов длины 2 русского языка <see cref="Ciphers.Language.Russian"/>
        /// </summary>
        public static List<string> FrequencyWb2LRUS = new List<string>()
        {
            "НЕ",
            "НА",
            "ОН",
            "ПО",
            "НО",
            "МЫ",
            "ИЗ",
            "ТО",
            "ЗА"
        };

        /// <summary>
        /// Упорядоченный по убыванию список самых частых последовательностей из 2 букв русского языка <see cref="Ciphers.Language.Russian"/>
        /// </summary>
        public static List<string> FrequencyBigramsRUS = new List<string>()
        {
            "СТ",
            "НО",
            "ЕН",
            "ТО",
            "НА",
            "ОВ",
            "НИ",
            "РА",
            "ВО"
        };

        /// <summary>
        /// Упорядоченный по убыванию список самых частых последовательностей из 3 букв русского языка <see cref="Ciphers.Language.Russian"/>
        /// </summary>
        public static List<string> FrequencyThreegramsRUS = new List<string>()
        {
            "СТО",
            "ЕНО",
            "НОВ",
            "ТОВ",
            "ОВО",
            "ОВА",
        };

        /// <summary>
        /// Упорядоченный по убыванию список самых частых однобуквенных слов английского языка <see cref="Ciphers.Language.English"/>
        /// </summary>
        public static List<char> FrequencyWb1LENG = new List<char>()
        {
            'A',
            'I'
        };

        /// <summary>
        /// Упорядоченный по убыванию список самых частых слов длины 2 английского языка <see cref="Ciphers.Language.English"/>
        /// </summary>
        public static List<string> FrequencyWb2LENG = new List<string>()
        {
            "OF",
            "TO",
            "IN",
            "IS",
            "ON",
            "HE",
            "IT",
            "AS",
            "AT"
        };

        /// <summary>
        /// Упорядоченный по убыванию список самых частых последовательностей из 2 букв английского языка <see cref="Ciphers.Language.English"/>
        /// </summary>
        public static List<string> FrequencyBigramsENG = new List<string>()
        {
            "TH",
            "HE",
            "IN",
            "ER",
            "AN",
            "RE",
            "ES",
            "ON",
            "ST",
            "NT"
        };

        /// <summary>
        /// Упорядоченный по убыванию список самых частых последовательностей из 3 букв английского языка <see cref="Ciphers.Language.English"/>
        /// </summary>
        public static List<string> FrequencyThreegramsENG = new List<string>()
        {
            "THE",
            "AND",
            "ING",
            "ENT",
            "ION",
            "HER",
            "FOR",
            "THA"
        };
    }
}
