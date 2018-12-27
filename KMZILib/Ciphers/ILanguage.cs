﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMZILib
{
    public static partial class  Ciphers
    {

        public abstract class ALanguage
        {
            public static string Alphabet { get; protected set; }
            public static List<char> Frequency { get; protected set; }

            public static List<char> FrequencyWb1L { get; protected set; }
            public static List<string> FrequencyWb2L { get; protected set; }

            public static List<string> FrequencyBigrams { get; protected set; }
            public static List<string> FrequencyThreegrams { get; protected set; }
        }

        public class RussianLanguage : ALanguage
        {
            static RussianLanguage()
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

    }
    
}
