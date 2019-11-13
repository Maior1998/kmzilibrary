using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;
using static KMZILib.Comparison;


namespace KMZILib
{
    /// <summary>
    ///     Класс, отвечающий за работу функций теории кодирования
    /// </summary>

    public static class CodingTheory
    {
        /// <summary>
        ///     Представляет собой двоичный набор данных
        /// </summary>
        public class ByteSet
        {
            /// <summary>
            ///     Инициализирует пустой вектор
            /// </summary>
            public ByteSet()
            {
                Value = new byte[0];
            }

            /// <summary>
            ///     Инициализирует вектор, представляющий заданный массив байт.
            /// </summary>
            /// <param name="value"></param>
            public ByteSet(byte[] value)
            {
                Value = new byte[value.Length];
                value.CopyTo(Value, 0);
            }

            /// <summary>
            ///     Инициализирует копию указанного вектора
            /// </summary>
            /// <param name="Other"></param>
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
            ///     Значение текущего бинарного вектора
            /// </summary>
            public byte[] Value { get; internal set; }

            /// <summary>
            ///     Строковое представление вектора
            /// </summary>
            public string StringValue => string.Join("", Value);

            /// <summary>
            ///     Длина вектора (число байт)
            /// </summary>
            public int Length => Value.Length;

            internal void Expand(byte value)
            {
                Value = new List<byte>(Value) { value }.ToArray();
            }

            /// <summary>
            /// Вставляет бит по указанному индексу.
            /// </summary>
            /// <param name="TargetIndex"></param>
            /// <param name="TargetValue"></param>
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
            /// <param name="TargetIndex"></param>
            /// <param name="TargetValue"></param>
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
            ///     Строковое представление байтового вектора.
            /// </summary>
            /// <returns>Строка, представляющая все байты ветокра</returns>
            public override string ToString()
            {
                return StringValue;
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
        /// Возвращает энтропию поданной на вход статистики
        /// </summary>
        /// <param name="StatsDouble"></param>
        /// <returns></returns>
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
        /// <returns></returns>
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
        ///     Коды, осуществляющие сжатие данных.
        /// </summary>
        public static class DataCompressionCodes
        {
            /// <summary>
            /// Коды алфавитного кодирования.
            /// </summary>
            public static class AlphabeticalCoding
            {
                /// <summary>
                ///     Представляет код Хаффмана.
                /// </summary>
                public static class HuffmanCoding
                {
                    /// <summary>
                    ///     Осуществляет генерацию кодов, подходящим заданным частотам так, чтобы добиться наименьшей средней длины сообщения
                    /// </summary>
                    /// <param name="Probabilities">Массив - частотный анализ исходного алфавита</param>
                    /// <param name="k">Система счисления, в которой будет произведено кодирование</param>
                    /// <param name="AverageLength">Вычисленная в процессе генерации кодов средняя длина сообщения</param>
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
                            LinearComparison k0 = new LinearComparison(ProbCop.Length, k - 1);
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
                                buffer.Last().Expand((byte)i);
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
                            buffer.Last().Expand((byte)i);
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
                    /// <param name="Prob"></param>
                    /// <returns></returns>
                    public static int GetL(double Prob)
                    {
                        return (int)Math.Ceiling(-Math.Log(Prob, 2));
                    }

                    /// <summary>
                    /// Осуществляет кодирование Шеннона.
                    /// </summary>
                    /// <param name="Probabilities">Массив вероятностей исходных символов. Будут возвращены отсортированные в порядке убывания вероятностей коды.</param>
                    /// <returns></returns>
                    public static ByteSet[] GetCodes(double[] Probabilities, out double AverageLength)
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
                    public static ByteSet[] GetCodes(double[] Probabilities, out double AverageLength)
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
            }


            /// <summary>
            /// Представляет арифметическое кодирование.
            /// </summary>
            public static class ArithmeticCoding
            {
                /// <summary>
                /// Осуществляет процедуру нахождения арифметического кода. Работает точно для фраз длиной до 12 символов включительно.
                /// </summary>
                public static double GetCode(string Source, out double AverageLength)
                {
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

                    AverageLength = 0;
                    return F + G / 2;
                }

                /// <summary>
                /// Осуществляет декодирование арифметического кода.
                /// </summary>
                public static string Decode(double F, char[] Alphabet, double[] P, int Length)
                {
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
            /// Представляет код Левенштейна.
            /// </summary>
            public static class LevenshteinCoding
            {
                /// <summary>
                /// Осуществляет кодирование алгоритмом Левенштейна.
                /// </summary>
                /// <param name="Source"></param>
                /// <returns></returns>
                public static ByteSet GetCode(int Source)
                {
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

                    Result.Put(0, Enumerable.Repeat((byte)1, C).Concat(new[] { (byte)0 }).ToArray());
                    return Result;
                }


            }
        }
    }
}