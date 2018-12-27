using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMZILib
{
    public static partial class Ciphers
    {
        /// <summary>
        /// Класс предоставляющий данные и методы для работы с естественными языками.
        /// </summary>
        public static class Languages
        {
            /// <summary>
            /// Текущий язык, с которым оперируют методы.
            /// </summary>
            public static ALanguage CurrentLanguage;

            /// <summary>
            /// Абстрактный класс, который указывает поля, которыми обладают все языки.
            /// </summary>
            public abstract class ALanguage
            {
                /// <summary>
                /// Строковое представление алфавита языка.
                /// </summary>
                public string Alphabet { get; protected set; }
                /// <summary>
                /// Представляет упорядоченный по частоте встречи список букв алфавита
                /// </summary>
                public List<char> Frequency { get; protected set; }
                /// <summary>
                /// Упорядоченный по частоте встречи список слов длины 1
                /// </summary>
                public List<char> FrequencyWb1L { get; protected set; }
                /// <summary>
                /// Упорядоченный по частоте встречи список слов длины 2
                /// </summary>
                public List<string> FrequencyWb2L { get; protected set; }
                /// <summary>
                /// Упорядоченный по частоте встречи список пар букв, которые встречаются друг за другом
                /// </summary>
                public List<string> FrequencyBigrams { get; protected set; }
                /// <summary>
                /// Упорядоченный по частоте встречи список троек букв, которые встречаются друг за другом
                /// </summary>
                public List<string> FrequencyThreegrams { get; protected set; }
            }

            /// <summary>
            /// Представляет данные для работы с русским языком
            /// </summary>
            public class RussianLanguage : ALanguage
            {
                public RussianLanguage()
                {
                    Alphabet = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
                    Frequency = new List<char>
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
                    FrequencyWb1L = new List<char>
                    {
                        'И',
                        'В',
                        'Я',
                        'С',
                        'А',
                        'К',
                        'У'
                    };
                    FrequencyWb2L = new List<string>
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
                    FrequencyBigrams = new List<string>
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
                    FrequencyThreegrams = new List<string>
                    {
                        "СТО",
                        "ЕНО",
                        "НОВ",
                        "ТОВ",
                        "ОВО",
                        "ОВА"
                    };

                }
            }

            /// <summary>
            /// Представляет данные для работы с английским языком
            /// </summary>
            public class EnglishLanguage : ALanguage
            {
                public EnglishLanguage()
                {
                    Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    Frequency = new List<char>
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
                    FrequencyWb1L = new List<char>
                    {
                        'A',
                        'I'
                    };
                    FrequencyWb2L = new List<string>
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
                    FrequencyBigrams = new List<string>
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
                    FrequencyThreegrams = new List<string>
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
        }
    }
}
