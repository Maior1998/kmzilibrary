using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace KMZILib
{
    public static partial class Ciphers
    {
        /// <summary>
        ///     Шифр Виженера
        /// </summary>
        public static class VigenereCipher
        {
            private static char GetEncryptedChar(char SourceChar, char KeyChar)
            {
                int SourceIndex = Languages.CurrentLanguage.Alphabet.IndexOf(SourceChar);
                int KeyIndex = Languages.CurrentLanguage.Alphabet.IndexOf(KeyChar);
                return Languages.CurrentLanguage.Alphabet[(SourceIndex + KeyIndex) % Languages.CurrentLanguage.Alphabet.Length];
            }

            private static char GetDecryptedChar(char EncryptedShar, char KeyChar)
            {
                int SourceIndex = Languages.CurrentLanguage.Alphabet.IndexOf(EncryptedShar);
                int KeyIndex = Languages.CurrentLanguage.Alphabet.IndexOf(KeyChar);
                Comparison.LinearComparison ResultIndex = new Comparison.LinearComparison(SourceIndex - KeyIndex, Languages.CurrentLanguage.Alphabet.Length);
                return Languages.CurrentLanguage.Alphabet[(int)ResultIndex.A];
            }

            /// <summary>
            ///     Осуществляет шифрование строки с использованием заданного лозунга
            /// </summary>
            /// <param name="Source">Открытый текст, к которому необходимо применить алгоритм</param>
            /// <param name="Key">Лозунг - ключ, который будет использован при шифровании</param>
            /// <returns>Шифртекст - результат шифрования</returns>
            public static string Encrypt(string Source, string Key)
            {
                StringBuilder buffer = new StringBuilder(string.Concat(Regex.Split(Source.ToUpper(), @"\W")));
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
                StringBuilder buffer = new StringBuilder(string.Concat(Regex.Split(EncryptedText.ToUpper(), @"\W")));
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
                    CipherText = string.Concat(Regex.Split(CipherText, @"\W"));
                    //На вход дали шифртекст. 
                    //Необходимо найти все повторяющиеся подстроки и сделать из них KasiskiExaminationResults.
                    /*
                                 * TODO: алгоритм:
                                 * Подсчитать расстояния между всеми совпадающими триграммами в тексте
                                 * Подсчитать НОД для каждой пары таких расстояний
                                 * Выбрать из этого НОДа наиболее встречаемые
                                 */
                    int CurrentSubstringLength = Math.Min((int) Math.Sqrt(CipherText.Length), 30);
                    bool[] ThreadsStatus = new bool[CurrentSubstringLength - 2];
                    Dictionary<int, int>[] CurSubstrPosKeysArray= new Dictionary<int, int>[CurrentSubstringLength - 2];

                    //Сначала запускаем кучу потоков для для всех длин от 3 до корня из длины текста
                    for (; CurrentSubstringLength > 2; CurrentSubstringLength--)
                    {
                        int length = CurrentSubstringLength;
                        Thread Check = new Thread(() =>
                        {

                            //Console.WriteLine($"Выполняю длину подстроки {length}");
                            Dictionary<int, int> CurSubstrPosKeys =
                                PossibleKeyLengthes(CipherText, length);
                            CurSubstrPosKeysArray[length-3] = CurSubstrPosKeys;

                            ThreadsStatus[length - 3] = true;
                            Console.WriteLine($"Длина {length} закончена.");
                        }) {IsBackground = true, Name = CurrentSubstringLength.ToString()};
                        Check.Start();
                    }

                    //ждем завершения всех потоков
                    while (ThreadsStatus.Any(status => !status))
                    {
                        Console.WriteLine($"Длина {ThreadsStatus.ToList().FindIndex(status => !status)+3} все еще не закончена");
                        Thread.Sleep(1000);
                    }
                    
                    //теперь необходимо слить все словари в один.
                    Dictionary<int,int> Result = new Dictionary<int, int>();
                    foreach (Dictionary<int, int> dictionary in CurSubstrPosKeysArray)
                    {
                        foreach (int dictionaryKey in dictionary.Keys)
                        {
                            if (!Result.ContainsKey(dictionaryKey)) Result.Add(dictionaryKey, 0);
                            Result[dictionaryKey] += dictionary[dictionaryKey];
                        }
                    }
                    //слили все словари в один. Через жопу, но как иначе?


                    //словарь НОДов длин заполнен. теперь необходимо выбрать самые частые
                    return Result.
                        OrderByDescending(pair => pair.Value).
                        Select(pair => pair.Key).
                        Take(10).
                        ToArray();
                }

                private static Dictionary<int, int> PossibleKeyLengthes(string Source, int SubstringLength)
                {
                    Dictionary<int,int> Lengths=new Dictionary<int, int>();
                    List<string> LastResults = new List<string>();
                    for (int CurrentPosition = 0;
                        CurrentPosition < Source.Length - SubstringLength;
                        CurrentPosition++)
                    {
                        //Создаем текущую подстроку
                        string CurrentSubString = Source.Substring(CurrentPosition, 3);
                        //Если такая строка уже встречалась, то просто пропускаем ее
                        if (LastResults.Any(kasres => kasres == CurrentSubString))
                            continue;

                        List<Match> Textes = new Regex(CurrentSubString).Matches(Source).Cast<Match>()
                            .ToList();

                        //Если строка встречается всего один раз то тоже ее пропускаем
                        if (Textes.Count == 1) continue;

                        //Теперь необходимо заполнить данные о длине ключевого слова
                        //Получаем список делителей - список потенциальных длин ключа


                        //получили индексы всех вхождений нашей подстроки
                        int[] SubStrIndexes = Textes.Select(match => match.Index).ToArray();

                        //завели список под расстояния между каждой парой подстрок
                        //TODO: не между каждыми парами, а между каждыми ПОСЛЕДОВАТЕЛЬНЫМИ ПАРАМИ
                        int[] SubStrLengthes = new int[SubStrIndexes.Length - 1];
                        for (int i = 0; i < SubStrLengthes.Length; i++)
                            SubStrLengthes[i] = SubStrIndexes[i + 1] - SubStrIndexes[i] - 3;
                        //заполнили массив расстояний между двумя последовательными длинами
                        //теперь необходимо посчитать все НОДы
                        //"Предполагаемая длина ключевого слова кратна наибольшему общему делителю всех расстояний."
                        int LengthGCD = (int)
                            AdvancedEuclidsalgorithm.GCDResult(SubStrLengthes.Select(num => (BigInteger)num)
                                .ToList());
                        foreach (int divider in Comparison.GetUniqueNumberDividersF(LengthGCD))
                        {
                            if(divider<=2) continue;
                            if (!Lengths.ContainsKey(divider)) Lengths.Add(divider, 0);
                            Lengths[divider]++;
                        }

                        //Записали. теперь добавляем результат в список для того, чтобы еще раз его
                        //не посчитать и продолжаем

                        LastResults.Add(CurrentSubString);
                    }
                    //словарь НОДов длин заполнен. теперь необходимо выбрать самые частые
                    return Lengths;
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
                    if (KeyLength <= 1) return new[] {Source};
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
}