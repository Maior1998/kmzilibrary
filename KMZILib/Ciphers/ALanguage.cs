using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
            /// Список всех имеющихся языков.
            /// </summary>
            public static List<ALanguage> LanguagesList=new List<ALanguage>()
            {
                RussianLanguage.GetInstanse(),
                EnglishLanguage.GetInstanse(),
            };

            /// <summary>
            /// Абстрактный класс, который указывает поля, которыми обладают все языки.
            /// </summary>
            public abstract class ALanguage
            {
                /// <summary>
                /// Содержит сущность текущего языка. Это поле необходимо для того, чтобы не создавать кучу копий одних и тех же статистических параметров.
                /// </summary>
                private static ALanguage instance;

                /// <summary>
                /// Строковое представление алфавита языка в верхнем регистре. Менять на свой страх и риск.
                /// </summary>
                public string Alphabet { get; set; }
                /// <summary>
                /// Представляет упорядоченный по частоте встречи список букв алфавита.
                /// </summary>
                public List<char> Frequency { get; protected set; }
                /// <summary>
                /// Упорядоченный по частоте встречи список пар букв, которые встречаются друг за другом.
                /// </summary>
                public List<string> FrequencyBigrams { get; protected set; }
                /// <summary>
                /// Упорядоченный по частоте встречи список троек букв, которые встречаются друг за другом.
                /// </summary>
                public List<string> FrequencyThreegrams { get; protected set; }
                /// <summary>
                /// Упорядоченный по частоте встречи список слов длины 1.
                /// </summary>
                public List<char> FrequencyWb1L { get; protected set; }
                /// <summary>
                /// Упорядоченный по частоте встречи список слов длины 2.
                /// </summary>
                public List<string> FrequencyWb2L { get; protected set; }
                /// <summary>
                /// Алфавит языка, представленный в виде массива символов в верхнем регистре.
                /// </summary>
                public char[] AlphabetArray => Alphabet.ToCharArray();

            }

            /// <summary>
            /// Возвращает язык, который содержит указанный символ. Если язык найти не удалось, вернет null.
            /// </summary>
            /// <param name="Symbol"></param>
            /// <returns>Если язык найден - сам язык. Если не найден - null</returns>
            public static ALanguage LangByChar(char Symbol)
            {
                foreach (ALanguage language in LanguagesList)
                    if (language.Alphabet.Contains(char.ToUpper(Symbol)))
                        return language;
                return null;
            }

            /// <summary>
            /// Представляет данные для работы с русским языком
            /// </summary>
            public class RussianLanguage : ALanguage
            {
                private static ALanguage instance;

                /// <summary>
                /// Инициализирует статистические данные русского алфавита
                /// </summary>
                private RussianLanguage()
                {
                    Alphabet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
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

                /// <summary>
                /// Возвращает строковую запись имени данного языка
                /// </summary>
                /// <returns>Строковая запись имени данного языка</returns>
                public override string ToString()
                {
                    return "Русский язык";
                }

                /// <inheritdoc />
                /// <summary>
                /// Получает комплект данных о русском языке.
                /// </summary>
                /// <returns>Набор данных (включая алфавит) текущего русского языка.</returns>
                public static ALanguage GetInstanse()
                {
                    if (ReferenceEquals(instance, null))
                        return instance = new RussianLanguage();
                    return instance;
                }
            }

            /// <summary>
            /// Представляет данные для работы с английским языком
            /// </summary>
            public class EnglishLanguage : ALanguage
            {
                private static ALanguage instance;

                /// <summary>
                /// Инициализирует статистические данные английского алфавита
                /// </summary>
                private EnglishLanguage()
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

                /// <summary>
                /// Возвращает строковую запись имени данного языка
                /// </summary>
                /// <returns>Строковая запись имени данного языка</returns>
                public override string ToString()
                {
                    return "Английский язык";
                }

                /// <inheritdoc />
                /// <summary>
                /// Получает комплект данных об английском языке.
                /// </summary>
                /// <returns>Набор данных (включая алфавит) текущего английского языка.</returns>
                public static ALanguage GetInstanse()
                {
                    if (ReferenceEquals(instance, null))
                        return instance = new EnglishLanguage();
                    return instance;
                }

                
            }
        }
    }
}
