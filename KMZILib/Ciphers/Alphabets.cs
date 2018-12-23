namespace KMZILib
{
    public static partial class Ciphers
    {
        /// <summary>
        ///     Класс, в котором содержатся данные об обрабатываемых алфавитах
        /// </summary>
        public static class Alphabets
        {
            /// <summary>
            ///     Русский алфавит (без 'ё'). Верхний регистр
            /// </summary>
            public const string RussianAlphabet = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";

            /// <summary>
            ///     Английский алфавит. Верхний регистр
            /// </summary>
            public const string EnglishAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            /// <summary>
            ///     Текущий алфавит, с которым работают шифры
            /// </summary>
            public static Language CurrentLanguage;

            /// <summary>
            ///     Свойство, предоставляющее алфавит для текущего языка
            /// </summary>
            public static string CurrentAlphabet
            {
                get
                {
                    switch (CurrentLanguage)
                    {
                        case Language.Russian:
                            return RussianAlphabet;
                        case Language.English:
                            return EnglishAlphabet;
                        default:
                            return RussianAlphabet;
                    }
                }
            }

            //public static Dictionary<char,double> FrequencyRus=new Dictionary<char, double>()
            //{
            //    {'о',0.1097 },
            //    {'е',0.0845 },
            //    {'а',0.0801 },
            //    {'и',0.0735 },
            //    {'н',0.0670 },
            //    {'т',0.0626 },
            //    {'с',0.0547 },
            //    {'р',0.0473 },
            //    {'в',0.0454 },
            //    {'л',0.0440 },
            //    {'к',0.0349 },
            //    {'м',0.0321 },
            //    {'д',0.0298 },
            //    {'п',0.0281 },
            //    {'у',0.0262 },
            //    {'я',0.0201 },
            //    {'ы',0.0190 },
            //    {'ь',0.0174 },
            //    {'г',0.0170 },
            //    {'з',0.0165 },
            //    {'б',0.0159 },
            //    {'ч',0.0144 },
            //    {'й',0.0121 },
            //    {'х',0.0097 },
            //    {'ж',0.0094 },
            //    {'ш',0.0073 },
            //    {'ю',0.0064 },
            //    {'ц',0.0048 },
            //    {'щ',0.0036 },
            //    {'э',0.0032 },
            //    {'ф',0.0026 },
            //    {'ъ',0.0004 },
            //};
        }
    }
}