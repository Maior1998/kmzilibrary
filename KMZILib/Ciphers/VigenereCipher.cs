using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Remoting.Messaging;
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
                int SourceIndex = Alphabets.CurrentAlphabet.IndexOf(SourceChar);
                int KeyIndex = Alphabets.CurrentAlphabet.IndexOf(KeyChar);
                return Alphabets.CurrentAlphabet[(SourceIndex + KeyIndex) % Alphabets.CurrentAlphabet.Length];
            }

            private static char GetDecryptedChar(char EncryptedShar, char KeyChar)
            {
                int SourceIndex = Alphabets.CurrentAlphabet.IndexOf(EncryptedShar);
                int KeyIndex = Alphabets.CurrentAlphabet.IndexOf(KeyChar);
                int ResultIndex = SourceIndex - KeyIndex;
                if (ResultIndex < 0) ResultIndex += Alphabets.CurrentAlphabet.Length;
                return Alphabets.CurrentAlphabet[ResultIndex % Alphabets.CurrentAlphabet.Length];
            }

            /// <summary>
            ///     Представляет методы для работы теста Касиски (KasiskiExamination)
            /// </summary>
            public static class KE
            {
                /// <summary>
                ///     Представляет собой объект логически предполагаемого ключа (KasiskiExaminationResult)
                /// </summary>
                public class KER
                {
                    /// <summary>
                    ///     Вероятные длины ключа
                    /// </summary>
                    public int[] PossibleKeyLengthes { get; set; }

                    /// <summary>
                    ///     Расстояние между двумя повторяющимися подстроками, из которого был сделан вывод о существовании данного ключа
                    /// </summary>
                    public int gLength { get; set; }

                    /// <summary>
                    ///     Предполагаемый ключ
                    /// </summary>
                    public string Key { get; set; }

                    /// <summary>
                    ///     Подстрока, встреченная несколько раз
                    /// </summary>
                    public string FoundedSubstring { get; set; }

                    /// <summary>
                    ///     Длины найденных совпадающих подстрок
                    /// </summary>
                    public int SubstringsLength { get; set; }

                    /// <summary>
                    ///     Число встреченных подстрок, на основании которых было решено создать такой ключ
                    /// </summary>
                    public int Count => Indexes.Length;

                    /// <summary>
                    ///     Массив индексов, по которым были встречены повторяющиеся строки в зашифрованном тексте
                    /// </summary>
                    public int[] Indexes { get; set; }

                    public override bool Equals(object obj)
                    {
                        return FoundedSubstring.Equals(obj);
                    }

                    public override int GetHashCode()
                    {
                        return FoundedSubstring.GetHashCode();
                    }

                    /// <summary>
                    ///     Преобразует ключ в удобный для просмотра человеком формат
                    /// </summary>
                    /// <returns>Текстовое представление ключа</returns>
                    public override string ToString()
                    {
                        return $"Founded: {FoundedSubstring}";
                    }

                    

                }

                /// <summary>
                ///     Возвращает массив возможных длин ключа с учетом всех данных гипотез
                /// </summary>
                /// <param name="results"></param>
                /// <returns></returns>
                public static int[] PossibleLengthes(KER[] results)
                {
                    IEnumerable<int> buffer = new List<int>();
                    buffer = results.Aggregate(buffer, (current, result) => current.Union(result.PossibleKeyLengthes));
                    int[] Result = buffer.ToArray();
                    Array.Sort(Result);
                    return Result;
                }

                /// <summary>
                ///     Анализирует заданный текст и возвращает массив гипотез о возможных длинах ключа
                /// </summary>
                /// <param name="CipherText"></param>
                /// <returns></returns>
                public static KER[] GetKEHypotheses(string CipherText)
                {
                    CipherText = string.Concat(Regex.Split(CipherText, @"\W"));
                    //На вход дали шифртекст. 
                    //Необходимо найти все повторяющиеся подстроки и сделать из них KasiskiExaminationResults.

                    List<KER> GeneralResults = new List<KER>(10000);
                    int CurrentSubstringLength = CipherText.Length / 2;
                    bool[]ThreadsStatus = new bool[CipherText.Length/2-3];
                    //Сначала запускаем кучу потоков для для всех длин от 4 до длины текста, деленной на 2
                    for (; CurrentSubstringLength > 3; CurrentSubstringLength--)
                    {
                        int length = CurrentSubstringLength;
                        Thread Check = new Thread(() =>
                        {
                            Console.WriteLine($"Я поток {Thread.CurrentThread.Name} и я выполняю длину продстроки {length}");
                            List<KER> CurrentResults=new List<KER>();
                            for (int CurrentPosition = 0;
                                CurrentPosition < CipherText.Length - length;
                                CurrentPosition++)
                            {
                                //Создаем текущую подстроку
                                string CurrentSubString = CipherText.Substring(CurrentPosition, length);
                                //Если такая строка уже встречалась, то просто пропускаем ее
                                if (CurrentResults.Any(kasres => kasres.FoundedSubstring == CurrentSubString))
                                {
                                    Console.WriteLine($"{length}) Оказывается, подстрока {CurrentSubString} уже есть.");
                                    continue;
                                }

                                List<Match> Textes = new Regex(CurrentSubString).Matches(CipherText).Cast<Match>()
                                    .ToList();

                                //Если строка встречается всего один раз то тоже ее пропускаем
                                if (Textes.Count == 1) continue;

                                int gLength = Textes[1].Index - Textes[0].Index;
                                for (int i = 0; i < Textes.Count - 1; i++)
                                {
                                    if (Textes[i + 1].Index - Textes[i].Index ==
                                        gLength) continue;
                                    Textes.RemoveAt(i-- + 1);
                                }

                                //Удалили все неподходящие подстроки, которые расположены не на нужном расстоянии друг от друга.
                                //Теперь необходимо заполнить данные о длине ключевого слова
                                //Получаем список делителей - список потенциальных длин ключа
                                //TODO: так работает, когда таких блока всего два, что бывает не всегда

                                int[] SubStrIndexes = Textes.Select(match => match.Index).ToArray();
                                int[]Lengthes = new int[SubStrIndexes.Length-1];
                                for (int i = 0; i < Lengthes.Length; i++)
                                    Lengthes[i] = SubStrIndexes[i + 1] - SubStrIndexes[i];
                                /*
                                 * Группы должны состоять из не менее чем трех символов.
                                 * Тогда расстояния между последовательными возникновениями групп, вероятно, будут кратны длине ключевого слова.
                                 * Предполагаемая длина ключевого слова кратна наибольшему общему делителю всех расстояний.
                                 */


                                int[] Dividers = Comparison.GetUniqueNumberDividersF(
                                    (int) AdvancedEuclidsalgorithm.GCDResult(Lengthes.Select(len => new BigInteger(len))
                                        .ToList()));
                                //Кандидат - любой из них
                                KER buffer = new KER
                                {
                                    Indexes = SubStrIndexes,
                                    gLength = gLength,
                                    PossibleKeyLengthes = Dividers,
                                    SubstringsLength = length,
                                    FoundedSubstring = CurrentSubString
                                };
                                
                                CurrentResults.Add(buffer);
                            }
                            GeneralResults.AddRange(CurrentResults);
                            ThreadsStatus[length-4] = true;
                        }) {IsBackground = true, Name = CurrentSubstringLength.ToString()};
                        Check.Start();
                        
                    }
                    //потом проходим осташийся цикл с длиной 3
                    //TODO: придумать, как заставить все потоки завершиться одновременно.
                    while (ThreadsStatus.Any(status => !status))
                        Thread.Sleep(1000);
                    List<KER> LastResults = new List<KER>();
                    for (int CurrentPosition = 0;
                        CurrentPosition < CipherText.Length - CurrentSubstringLength;
                        CurrentPosition++)
                    {
                        //Создаем текущую подстроку
                        string CurrentSubString = CipherText.Substring(CurrentPosition, CurrentSubstringLength);
                        //Если такая строка уже встречалась, то просто пропускаем ее
                        if (LastResults.Any(kasres => kasres.Key == CurrentSubString)) continue;

                        List<Match> Textes = new Regex(CurrentSubString).Matches(CipherText).Cast<Match>()
                            .ToList();

                        //Если строка встречается всего один раз то тоже ее пропускаем
                        if (Textes.Count == 1) continue;

                        int gLength = Textes[1].Index - Textes[0].Index;
                        for (int i = 0; i < Textes.Count - 1; i++)
                        {
                            if (Textes[i + 1].Index - Textes[i].Index ==
                                gLength) continue;
                            Textes.RemoveAt(i-- + 1);
                        }

                        //Удалили все неподходящие подстроки, которые расположены не на нужном расстоянии друг от друга.
                        //Теперь необходимо заполнить данные о длине ключевого слова
                        //Получаем список делителей - список потенциальных длин ключа
                        int[] Dividers = Comparison.GetUniqueNumberDividers(gLength);
                        KER buffer = new KER
                        {
                            Indexes = Textes.Select(match => match.Index).ToArray(),
                            gLength = gLength,
                            PossibleKeyLengthes = Dividers,
                            SubstringsLength = CurrentSubstringLength,
                            FoundedSubstring = CurrentSubString
                        };
                        LastResults.Add(buffer);
                    }
                    GeneralResults.AddRange(LastResults);
                    return GeneralResults.ToArray();
                }

                /// <summary>
                ///     Осуществляет разбиение теста на отрывки, которые учавствовали в процессе шифрования с одной и той же буквой ключа
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

            /// <summary>
            ///     Осуществляет шифрование строки с использованием заданого лозунга
            /// </summary>
            /// <param name="Source">Открытый текст, к которому необходимо применить алгоритм</param>
            /// <param name="Key">Лозунг - ключ, который будет использован при шифровании</param>
            /// <returns>Шифртекст - результат шифрования</returns>
            public static string Encrypt(string Source, string Key)
            {
                StringBuilder buffer = new StringBuilder(Source);
                for (int i = 0; i < buffer.Length; i++)
                    if (!Alphabets.CurrentAlphabet.Contains(buffer[i]))
                        buffer[i] = '.';

                buffer = buffer.Replace('.', ' ');
                string answer = buffer.ToString();
                while (answer.Contains("  "))
                    answer = answer.Replace("  ", " ");
                buffer = new StringBuilder(answer);
                for (int i = 0, k = 0; i < buffer.Length; i++, k++)
                {
                    if (buffer[i] == ' ')
                    {
                        k--;
                        continue;
                    }

                    buffer[i] = GetEncryptedChar(buffer[i], Key[i % Key.Length]);
                }

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
                StringBuilder buffer = new StringBuilder(EncryptedText);
                for (int i = 0; i < buffer.Length; i++)
                {
                    if (buffer[i] == ' ') continue;
                    buffer[i] = GetDecryptedChar(buffer[i], Key[i % Key.Length]);
                }

                return buffer.ToString();
            }
        }
    }
}