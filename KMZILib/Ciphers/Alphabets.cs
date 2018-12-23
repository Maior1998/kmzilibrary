namespace KMZILib
{
    public static partial class Ciphers
    {
        /// <summary>
        ///     Класс, в котором содержатся данные об обрабатываемых алфавитах.
        /// </summary>
        public static class Alphabets
        {
            /// <summary>
            ///     Русский алфавит (без 'ё'). Верхний регистр.
            /// </summary>
            public const string RussianAlphabet = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";

            /// <summary>
            ///     Английский алфавит. Верхний регистр.
            /// </summary>
            public const string EnglishAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            /// <summary>
            ///     Текущий алфавит, с которым работают шифры.
            /// </summary>
            public static Language CurrentLanguage;

            /// <summary>
            ///     Свойство, предоставляющее алфавит для текущего языка.
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
        }
    }
}