using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OSULib.Maths;


namespace OSULib.Misc
{
    /// <summary>
    ///     Класс, отвечающий за работу функций теории кодирования.
    /// </summary>
    public static class CodingTheory
    {
        /// <summary>
        /// Возвращает двочиное представление целого числа без первой единицы.
        /// </summary>
        /// <param name="Source">Число, для которого необходимо получить двоичное представление.</param>
        /// <returns>Битовая строка - представление числа без первой единицы.</returns>
        private static string bin(int Source)
        {
            return string.Concat(Misc.GetBinaryString(Source).Skip(1));
        }

        /// <summary>
        ///     Представляет собой двоичный набор данных.
        /// </summary>
        public class ByteSet
        {
            /// <summary>
            ///     Инициализирует пустой вектор.
            /// </summary>
            public ByteSet()
            {
                Value = new byte[0];
            }

            /// <summary>
            ///     Инициализирует вектор, представляющий заданный массив байт.
            /// </summary>
            /// <param name="value">Массив байт, состоящий из нулей и единиц.</param>
            public ByteSet(byte[] value)
            {
                Value = new byte[value.Length];
                value.CopyTo(Value, 0);
            }

            /// <summary>
            ///     Инициализирует копию указанного вектора.
            /// </summary>
            /// <param name="Other">Вектор ,от которого необходимо взять копию.</param>
            public ByteSet(ByteSet Other) : this(Other.Value)
            {
            }

            /// <summary>
            /// Инициализирует новый вектор по заданной строке.
            /// </summary>
            /// <param name="Source">Строка из нулей и единиц.</param>
            public ByteSet(string Source)
            {
                if (Source.Any(chr => chr != '0' && chr != '1'))
                    throw new InvalidCastException(
                        "Строка содержит неизвестные символы! Допускаются только символы \'0\' и \'1\'.''''");
                Value = Source.Select(val => val == '1' ? (byte)1 : (byte)0).ToArray();

            }

            /// <summary>
            ///     Значение текущего бинарного вектора.
            /// </summary>
            public byte[] Value { get; internal set; }

            /// <summary>
            ///     Строковое представление вектора.
            /// </summary>
            public string StringValue => string.Join("", Value);

            /// <summary>
            ///     Длина вектора (число бит).
            /// </summary>
            public int Length => Value.Length;

            /// <summary>
            /// Приписывает бит в конец битовой строки.
            /// </summary>
            /// <param name="value"></param>
            internal void Append(byte value)
            {
                Value = new List<byte>(Value) { value }.ToArray();
            }

            /// <summary>
            /// Вставляет бит по указанному индексу.
            /// </summary>
            /// <param name="TargetIndex">Индекс, в который необходимо вставить бит.</param>
            /// <param name="TargetValue">Бит, который необходимо вставить в вектор.</param>
            internal void Put(int TargetIndex, byte TargetValue)
            {
                byte[] buffer = new byte[Value.Length + 1];
                for (int i = 0; i < TargetIndex; i++)
                    buffer[i] = Value[i];
                buffer[TargetIndex] = TargetValue;
                for (int i = TargetIndex + 1; i < buffer.Length; i++)
                    buffer[i] = Value[i - 1];
                Value = buffer;
            }

            /// <summary>
            /// Вставляет биты по указанному индексу.
            /// </summary>
            /// <param name="TargetIndex">Индекс, в который необходимо вставить биты.</param>
            /// <param name="TargetValue">Массив бит, которые будут вставлены в вектор.</param>
            internal void Put(int TargetIndex, byte[] TargetValue)
            {
                byte[] buffer = new byte[Value.Length + TargetValue.Length];
                for (int i = 0; i < TargetIndex; i++)
                    buffer[i] = Value[i];
                for (int i = 0; i < TargetValue.Length; i++)
                    buffer[i + TargetIndex] = TargetValue[i];
                for (int i = TargetIndex + TargetValue.Length; i < buffer.Length; i++)
                    buffer[i] = Value[i - TargetValue.Length];
                Value = buffer;
            }

            /// <summary>
            /// Вырезает бит на нужной позиции.
            /// </summary>
            /// <param name="TargetIndex">Индекс, по которому необходимо вырезать бит.</param>
            internal ByteSet CutAt(int TargetIndex)
            {
                ByteSet Result = new ByteSet();
                byte[] buffer = new byte[Value.Length - 1];
                for (int i = 0; i < TargetIndex; i++)
                    buffer[i] = Value[i];
                Result.Append(Value[TargetIndex]);
                for (int i = TargetIndex + 1; i < Value.Length; i++)
                    buffer[i - 1] = Value[i];
                Value = buffer;
                return Result;
            }

            /// <summary>
            /// Вырезает нужное число бит от стартовой до конечной позиции включительно.
            /// </summary>
            /// <param name="StartIndex">Индекс начала вырезки.</param>
            /// <param name="EndIndex">Индекс конца вырезки. Включается в саму вырезку.</param>
            internal ByteSet CutAt(int StartIndex, int EndIndex)
            {
                return Cut(StartIndex, EndIndex - StartIndex + 1);
            }

            /// <summary>
            /// Вырезает нужное число бит начиная с указанной позиции.
            /// </summary>
            /// <param name="StartIndex">Индекс, начиная с которого необходимо вырезать биты.</param>
            /// <param name="Length">Число вырезаемых бит.</param>
            internal ByteSet Cut(int StartIndex, int Length)
            {
                ByteSet Result = new ByteSet();
                byte[] buffer = new byte[Value.Length - Length];
                for (int i = 0; i < StartIndex; i++)
                    buffer[i] = Value[i];
                for (int i = 0; i < Length; i++)
                    Result.Append(Value[StartIndex + i]);
                for (int i = StartIndex + Length; i < Value.Length; i++)
                    buffer[i - Length] = Value[i];
                Value = buffer;
                return Result;
            }

            /// <summary>
            ///     Строковое представление битового вектора.
            /// </summary>
            /// <returns>Строка, представляющая все биты ветокра</returns>
            public override string ToString()
            {
                return StringValue;
            }
        }

        /// <summary>
        /// Возвращает энтропию поданной на вход статистики.
        /// </summary>
        /// <param name="StatsDouble">Набор статистических данных в виде массива пар "буква - вероятность".</param>
        /// <returns>Энтропия источника сообщений.</returns>
        public static double GetEntropy(KeyValuePair<char, double>[] StatsDouble)
        {
            double Result = 0;
            foreach (KeyValuePair<char, double> pair in StatsDouble)
                Result += pair.Value * Math.Log(pair.Value, 2);
            Result *= -1;
            return Result;
        }

        /// <summary>
        /// Находит побуквенную статистику источника текста. Алфавит составляется из букв прибывшего текста.
        /// </summary>
        /// <param name="SourceText">Исходный текст.</param>
        /// <param name="sort">Нужно ли сортировать буквы по убыванию статистики.</param>
        /// <returns>Массив пар "буква - вероятность".</returns>
        public static KeyValuePair<char, double>[] GetStatisticOnegram(string SourceText, bool sort = true)
        {
            SourceText = SourceText.ToUpper();
            Dictionary<char, double> Result = new Dictionary<char, double>();
            foreach (char symbol in SourceText)
            {
                if (!Result.ContainsKey(symbol))
                {
                    Result.Add(symbol, 0.0);
                }

                Result[symbol]++;
            }

            double Sum = Result.Values.Sum();
            foreach (char resultKey in Result.Keys.ToList())
                Result[resultKey] /= Sum;
            return sort ? Result.OrderByDescending(a => a.Value).ToArray() : Result.ToArray();
        }

        /// <summary>
        ///     Представляет код Хаффмана.
        /// </summary>
        public static class HuffmanCoding
        {
            /// <summary>
            ///     Осуществляет генерацию кодов, подходящим заданным частотам так, чтобы добиться наименьшей средней длины сообщения.
            /// </summary>
            /// <param name="Probabilities">Массив - частотный анализ исходного алфавита.</param>
            /// <param name="k">Система счисления, в которой будет произведено кодирование.</param>
            /// <param name="AverageLength">Вычисленная в процессе генерации кодов средняя длина сообщения.</param>
            /// <returns>
            ///     Массив <see cref="ByteSet" />[], содержащий в себе коды, расположенные в соответствии введенным частотам в
            ///     порядке убывания.
            /// </returns>
            public static ByteSet[] GetCodes(double[] Probabilities, int k, out double AverageLength)
            {
                AverageLength = 0;
                if (k >= Probabilities.Length)
                {
                    ByteSet[] answer = new ByteSet[Probabilities.Length];
                    for (int i = 0; i < Probabilities.Length; i++)
                    {
                        answer[i] = new ByteSet { Value = new[] { (byte)i } };
                        AverageLength += Probabilities[i];
                    }

                    return answer;
                }

                //k - это число частей, на которые нам необходимо разбивать вероятности
                double[] ProbCop = new double[Probabilities.Length];
                Probabilities.CopyTo(ProbCop, 0);
                Array.Sort(ProbCop, (d, d1) => Math.Sign((d1 - d)));
                //отсортировали свою копию массива в порядке убывания вероятностей

                Stack<int> Insertions = new Stack<int>();
                //n - мощность исходного алфавита (кол-во вероятностей, которое на вход дали)
                //k - мощность конечного алфавита (система счисления)
                //ПЕРВАЯ СВЕРТКА
                int FirstConvolutionLength;
                if (k == 2) FirstConvolutionLength = 2;
                else
                {
                    Comparison.LinearComparison k0 = new Comparison.LinearComparison(ProbCop.Length, k - 1);
                    if (k0.A == 0)
                        FirstConvolutionLength = k - 1;
                    else if (k0.A == 1)
                        FirstConvolutionLength = k;
                    else
                        FirstConvolutionLength = (int)k0.A;
                }

                double Sum = ProbCop.Skip(ProbCop.Length - FirstConvolutionLength).Sum();
                Sum = Math.Round(Sum, 3);
                int Index = 0;
                int ResultLength = ProbCop.Length - FirstConvolutionLength + 1;
                while (ProbCop[Index] >= Sum && Index < ResultLength) Index++;
                Insertions.Push(Index);
                List<double> bufferProb = new List<double>(ProbCop);
                bufferProb.Insert(Index, Sum);
                ProbCop = bufferProb.Take(ResultLength).ToArray();
                //Последующие свертки
                while (ProbCop.Length != k)
                {
                    Sum = ProbCop.Skip(ProbCop.Length - k).Sum();
                    Sum = Math.Round(Sum, 3);
                    //вычислили сумму последних k элементов

                    //надо определить, на какой индекс будем вставлять
                    Index = 0;
                    ResultLength = ProbCop.Length - k + 1;
                    while (ProbCop[Index] >= Sum && Index < ResultLength) Index++;
                    //нашли индекс, на который необходимо вставить полученную сумму

                    Insertions.Push(Index);
                    bufferProb = new List<double>(ProbCop);
                    bufferProb.Insert(Index, Sum);
                    ProbCop = bufferProb.Take(ResultLength).ToArray();
                }

                List<ByteSet> Answer = new List<ByteSet>();
                for (int i = 0; i < ProbCop.Length; i++)
                    Answer.Add(new ByteSet { Value = new[] { (byte)i } });

                int Moving;
                List<ByteSet> buffer;
                //Все восстановления, кроме последнего
                while (Insertions.Count != 1)
                {
                    Moving = Insertions.Pop();
                    //узнали о перестановке
                    buffer = new List<ByteSet>(Answer);
                    buffer.RemoveAt(Moving);
                    for (int i = 0; i < k; i++)
                    {
                        buffer.Add(new ByteSet(Answer[Moving]));
                        buffer.Last().Append((byte)i);
                    }

                    Answer = buffer;
                }

                //ВОССТАНОВЛЕНИЕ ПОСЛЕДНЕЙ СВЕРТКИ
                Moving = Insertions.Pop();
                //узнали о перестановке
                buffer = new List<ByteSet>(Answer);
                buffer.RemoveAt(Moving);
                for (int i = 0; i < FirstConvolutionLength; i++)
                {
                    buffer.Add(new ByteSet(Answer[Moving]));
                    buffer.Last().Append((byte)i);
                }

                Answer = buffer;

                AverageLength += Probabilities.Select((t, i) => t * Answer[i].Length).Sum();
                return Answer.ToArray();
            }
        }

        /// <summary>
        /// Представляет код Шеннона.
        /// </summary>
        public static class ShannonCoding
        {
            /// <summary>
            /// Возвращает значение L, необходимое для алгоритма Шеннона, для заданной вероятности.
            /// </summary>
            /// <param name="Prob">Вероятность, для которой необходимо высчитать длину получаемого кодового слова.</param>
            /// <returns>Целое число - длина необходимого кодового слова.</returns>
            public static int GetL(double Prob)
            {
                return (int)Math.Ceiling(-Math.Log(Prob, 2));
            }

            /// <summary>
            /// Осуществляет кодирование Шеннона.
            /// </summary>
            /// <param name="Probabilities">Массив вероятностей исходного алфавита, отсортированный в порядке убывания частот.</param>
            /// <param name="AverageLength">Средняя длина получаемых кодовых слов.</param>
            /// <returns>Массив кодов слов, поставленный в соответствие каждой букве исходного алфавита в порядке убывания частот</returns>
            public static ByteSet[] Encode(double[] Probabilities, out double AverageLength)
            {
                AverageLength = 0;
                ByteSet[] Result = new ByteSet[Probabilities.Length];
                double[] Probs = Probabilities.OrderByDescending(val => val).ToArray();
                double Sum = 0;
                for (int i = 0; i < Probs.Length; i++)
                {
                    Result[i] = new ByteSet(Misc.DoubleFractToString(Sum, GetL(Probs[i])));
                    Sum += Probs[i];
                }

                AverageLength += Probabilities.Select((t, i) => t * Result[i].Length).Sum();
                return Result;
            }
        }

        /// <summary>
        /// Представляет код Гилберта-Мура.
        /// </summary>
        public static class GilbertCoding
        {
            /// <summary>
            /// Осуществляет кодирование алгоритмом Гилберта-Мура.
            /// </summary>
            /// <param name="Probabilities">Массив вероятностей исходного алфавита, отсортированный в порядке следования букв в исходном тексте.</param>
            /// <param name="AverageLength">Средняя длина получаемых кодовых слов.</param>
            /// <returns>Массив кодов слов, поставленный в соответствие каждой букве исходного алфавита в порядке следования букв в исходном тексте.</returns>
            public static ByteSet[] Encode(double[] Probabilities, out double AverageLength)
            {
                AverageLength = 0;
                double[] Q = new double[Probabilities.Length];
                ByteSet[] Result = new ByteSet[Probabilities.Length];

                for (int i = 0; i < Q.Length; i++)
                {
                    Q[i] += Probabilities[i] / 2;
                    Result[i] = new ByteSet(Misc.DoubleFractToString(Q[i],
                        (int)Math.Ceiling(-Math.Log(Probabilities[i], 2)) + 1));
                    for (int j = i + 1; j < Probabilities.Length; j++)
                        Q[j] += Probabilities[i];
                }

                AverageLength = Result.Select((res, ind) => res.Length * Probabilities[ind]).Sum();
                return Result;
            }
        }

        /// <summary>
        /// Осуществляет декодирование битовой строки по заданному алфавитному набору. Рабоатет только для префиксных кодов.
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Codes"></param>
        /// <returns></returns>
        public static string Decode(string Source, Dictionary<char, string> Codes)
        {
            StringBuilder Result = new StringBuilder();
            int index = 0;
            while (index != Source.Length)
            {
                char CurSymbol = Codes.First(codepair => Source.IndexOf(codepair.Value, index) == index).Key;
                Result.Append(CurSymbol);
                index += Codes[CurSymbol].Length;
            }

            return Result.ToString();
        }

        /// <summary>
        /// Осуществляет кодирование строки в битовую строку по заданному словарю какого-либо кода.
        /// </summary>
        /// <param name="Source">Исходная строка.</param>
        /// <param name="Codes">Словарь, полученный из кодовых методов.</param>
        /// <returns></returns>
        public static string Encode(string Source, Dictionary<char, string> Codes)
        {
            return string.Concat(Source.Select(chr => Codes[chr]));
        }

        /// <summary>
        /// Представляет арифметическое кодирование.
        /// </summary>
        public static class ArithmeticCoding
        {
            /// <summary>
            /// Осуществляет процедуру нахождения арифметического кода. Работает точно для фраз длиной до 15 символов включительно.
            /// </summary>
            public static double Encode(string Source)
            {
                if(Source.Length>15)
                    throw new InvalidOperationException("Длина фразы не должна превышать 15 символов.");
                Source = Source.ToUpper();
                KeyValuePair<char, double>[] Statistic =
                    GetStatisticOnegram(Source, false).OrderBy(row => row.Key).ToArray();
                double[] q = new double[Statistic.Length];
                for (int i = 1; i < Statistic.Length; i++)
                    q[i] = q[i - 1] + Statistic[i - 1].Value;
                double F = 0;
                double G = 1;
                for (int i = 0; i < Source.Length; i++)
                {
                    int LetInd = 0;
                    while (Source[i] != Statistic[LetInd].Key) LetInd++;

                    //F = F + q(xi)*G
                    F += q[LetInd] * G;

                    //G = G*p(xi)
                    G *= Statistic[LetInd].Value;

                }

                return F + G / 2;
            }

            /// <summary>
            /// Осуществляет декодирование арифметического кода. Работает точно для фраз длиной до 15 символов включительно.
            /// </summary>
            public static string Decode(double F, char[] Alphabet, double[] P, int Length)
            {
                if(Length>15)
                    throw new InvalidOperationException("Длина фразы не должна превышать 15 символов.");
                double[] q = new double[P.Length + 1];
                for (int i = 1; i < P.Length; i++)
                    q[i] = q[i - 1] + P[i - 1];
                q[P.Length] = 1;
                double S = 0;
                double G = 1;
                StringBuilder Result = new StringBuilder();
                for (int i = 0; i < Length; i++)
                {
                    int j = 0;
                    while (S + q[j + 1] * G < F) j++;
                    S += q[j] * G;
                    G *= P[j];
                    Result.Append(Alphabet[j]);
                }

                return Result.ToString();
            }


        }

        /// <summary>
        /// Представляет унарный код.
        /// </summary>
        public static class UnaryCoding
        {
            /// <summary>
            /// Осуществляет кодирование в унарный код.
            /// </summary>
            /// <param name="Source">Исходная строка, которую необходимо закодироовать.</param>
            /// <returns>Битовая закодированная строка.</returns>
            public static string Encode(int Source)
            {
                return string.Concat(new String('1', Source - 1), '0');
            }


            /// <summary>
            /// Осуществляет кодирование в унарный код со сдвигом.
            /// </summary>
            /// <param name="Source">Исходная строка, которую необходимо закодироовать.</param>
            /// <returns>Битовая закодированная строка.</returns>
            public static string EncodeShifted(int Source)
            {
                Source++;
                return string.Concat(new String('1', Source - 1), '0');
            }


            /// <summary>
            /// Осуществляет процедуру декодирования унарного кода.
            /// </summary>
            /// <param name="Source"></param>
            /// <returns></returns>
            public static int Decode(string Source)
            {
                return Source.Length;
            }

            /// <summary>
            /// Осуществляет процедуру декодирования унарного кодасо сдвигом.
            /// </summary>
            /// <param name="Source"></param>
            /// <returns></returns>
            public static int DecodeShifted(string Source)
            {
                return Source.Length - 1;
            }
        }

        /// <summary>
        /// Представляет код Левенштейна.
        /// </summary>
        public static class LevenshteinCoding
        {
            /// <summary>
            /// Осуществляет кодирование алгоритмом Левенштейна.
            /// </summary>
            /// <param name="Source"></param>
            /// <returns></returns>
            public static string Encode(int Source)
            {
                if (Source == 0) return "0";
                ByteSet Result = new ByteSet();
                int buffer = Source;
                int C = 1;
                while (buffer != 0)
                {
                    bool[] Binary = Misc.GetBinaryArray(buffer).Skip(1).ToArray();
                    buffer = Binary.Length;
                    if (buffer == 0) break;
                    Result.Put(0, Binary.Select(bol => bol ? (byte)1 : (byte)0).ToArray());
                    C++;
                }

                Result.Put(0, Enumerable.Repeat((byte)1, C - 1).Concat(new[] { (byte)0 }).ToArray());
                return Result.ToString();
            }

            /// <summary>
            /// Осуществляет декодирование алгоритмом Левенштейна.
            /// </summary>
            /// <param name="Source"></param>
            /// <returns></returns>
            public static int Decode(ByteSet Source)
            {
                int c = 0;
                while (Source.Value[c] != 0) c++;
                if (c == 0) return 0;
                Source.Cut(0, c + 1);
                int N = 1;
                int P = c - 1;
                while (P != 0)
                {
                    ByteSet buffer = Source.Cut(0, N);
                    buffer.Put(0, 1);
                    N = 0;
                    for (int i = buffer.Value.Length - 1; i >= 0; i--)
                        if (buffer.Value[i] == 1)
                            N += (int)Math.Pow(2, buffer.Length - 1 - i);
                    P--;
                }
                return N;
            }
        }

        /// <summary>
        /// Представляет код Элайеса.
        /// </summary>
        public static class EliasCoding
        {
            /// <summary>
            /// Осуществляет кодирование алгоритмом Элайеса.
            /// </summary>
            /// <param name="Source">Натуральное число, которое необходимо закодировать.</param>
            /// <returns>Строковое предсталвение кода Элайеса для данного числа.</returns>
            /// <exception cref="InvalidOperationException">Введено ненатуральное число.</exception>
            public static string Encode(int Source)
            {
                if (Source < 1)
                    throw new InvalidOperationException("Число должно быть натуральным!");
                if (Source == 1) return "0";
                string bini = bin(Source);
                string bini_bini = bin(bini.Length);
                return string.Concat(UnaryCoding.Encode(bini_bini.Length + 2),
                    bini_bini,
                    bini);
            }
        }

        /// <summary>
        /// Представляет интервальное кодирование.
        /// </summary>
        public static class RangeCoding
        {
            /// <summary>
            /// Осуществляет кодирование интервальным алгоритмом.
            /// </summary>
            /// <param name="Source"></param>
            /// <returns></returns>
            public static int[] Encode(string Source)
            {
                Source = Source.ToUpper();
                List<char> buffer = Source.Distinct().ToList();
                buffer.Sort();
                Dictionary<char, int> LastFound = new Dictionary<char, int>();
                for (int i = 0; i < buffer.Count; i++)
                    LastFound.Add(buffer[i], i);
                int[] Result = new int[Source.Length];
                Source = string.Concat(string.Join("", LastFound.Select(row => row.Key)), Source);
                for (int i = buffer.Count; i < Source.Length; i++)
                {
                    int sub = i - LastFound[Source[i]] - 1;
                    LastFound[Source[i]] = i;
                    Result[i - buffer.Count] = sub;
                }
                return Result;
            }

        }

        /// <summary>
        /// Представляет кодирование методом "стопки книг".
        /// </summary>
        public static class BookStackCoding
        {
            /// <summary>
            /// Осуществляет кодирование алгоритмом "стопка книг".
            /// </summary>
            /// <param name="Source">Исходная строка, которую нужно закодировать.</param>
            /// <returns>Последовательность натуральных чисел, отражающая данную строку.</returns>
            public static int[] Encode(string Source)
            {
                Source = Source.ToUpper();
                List<char> buffer = Source.Distinct().ToList();
                buffer.Sort();
                Dictionary<char, int> LastFound = new Dictionary<char, int>();
                for (int i = 0; i < buffer.Count; i++)
                    LastFound.Add(buffer[i], i);
                int[] Result = new int[Source.Length];
                Source = string.Concat(string.Join("", LastFound.Select(row => row.Key)), Source);
                for (int i = buffer.Count; i < Source.Length; i++)
                {
                    int sub = i - LastFound[Source[i]] - 1;
                    Result[i - buffer.Count] = Source.Substring(LastFound[Source[i]] + 1, sub).Distinct().Count();
                    LastFound[Source[i]] = i;
                }
                return Result;
            }
        }

        /// <summary>
        /// Представляет алгоритм LZ-77, используемый в архиваторах.
        /// </summary>
        public static class LZ77
        {
            /// <summary>
            /// Осуществляет поиск подстроки в блоке указаной длины.
            /// </summary>
            /// <param name="Source">Исходная строка, в которой необходимо выполнить поиск.</param>
            /// <param name="SourceIndex">Индекс, на которой находится новый символ.</param>
            /// <param name="DictLength">Размер словаря.</param>
            /// <param name="DestinationIndex">Расстояние до совпавшей подстроки.</param>
            /// <param name="DestiantionLength">Длина совпавшей подстроки.</param>
            /// <returns>Найдена подстрока, или нет.</returns>
            private static bool SearchSubstring(string Source, int SourceIndex, int DictLength, out int DestinationIndex, out int DestiantionLength)
            {
                DestinationIndex = -1;
                DestiantionLength = 0;
                for (int i = SourceIndex - DictLength < 0 ? 0 : SourceIndex - DictLength; i < SourceIndex; i++)
                {
                    if (Source[i] != Source[SourceIndex]) continue;
                    int CurrentLength = 0;
                    int CurrentIndex = i;
                    while (SourceIndex + CurrentLength < Source.Length &&
                           Source[SourceIndex + CurrentLength] == Source[CurrentIndex + CurrentLength]) CurrentLength++;
                    if (CurrentLength > DestiantionLength)
                    {
                        DestiantionLength = CurrentLength;
                        DestinationIndex = SourceIndex-CurrentIndex-1;
                    }
                }

                return DestinationIndex != -1;
            }

            /// <summary>
            /// Осуществляет кодирование ансамбля символов алгоритмом LZ-77.
            /// </summary>
            /// <param name="Source">Исходная строка, которую необходимо закодировать.</param>
            /// <param name="DictLength">Размер словаря, W.</param>
            /// <returns>Закодированнся битовая строка.</returns>
            public static string Encode(string Source, int DictLength)
            {
                Source = Source.ToUpper();
                int AlphabetLength = Source.Distinct().Count();
                int AlphabeticSymbolLength = (int) Math.Ceiling(Math.Log(AlphabetLength,2));
                int DictSymbolLength = (int) Math.Ceiling(Math.Log(DictLength,2));
                string Alphabet = string.Concat(Source.Distinct().OrderBy(ch => ch));
                StringBuilder Result = new StringBuilder();
                for (int i = 0; i < Source.Length; i++)
                {
                    if (SearchSubstring(Source, i, DictLength, out int DestinationIndex, out int DestiantionLength))
                    {
                        Result.Append('1');
                        Result.Append(Misc.GetBinaryString(DestinationIndex, DictSymbolLength));
                        Result.Append(LevenshteinCoding.Encode(DestiantionLength));
                        i += DestiantionLength - 1;
                    }
                    else
                    {
                        Result.Append('0');
                        Result.Append(Misc.GetBinaryString(Alphabet.IndexOf(Source[i]), AlphabeticSymbolLength));
                    }
                }
                return Result.ToString();
            }
        }

    }
}