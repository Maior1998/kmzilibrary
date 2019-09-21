using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
                if(Source.Any(chr=>chr!='0'&&chr!='1')) throw new InvalidCastException("Строка содержит неизвестные символы! Допускаются только символы \'0\' и \'1\'.''''");
                Value=Source.Select(val=>val=='1'?(byte)1:(byte)0).ToArray();

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
            ///     Длина вектора (число байтов)
            /// </summary>
            public int Length => Value.Length;

            internal void Expand(byte value)
            {
                Value = new List<byte>(Value) {value}.ToArray();
            }

            /// <summary>
            ///     Строковое представление байтового ветора
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
        ///     Коды, осуществляющие сжатие данных
        /// </summary>
        public static class DataCompressionCodes
        {
            /// <summary>
            ///     Представляет код Хаффмана
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
                            answer[i] = new ByteSet {Value = new[] {(byte) i}};
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
                            FirstConvolutionLength = (int) k0.A;
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
                        Answer.Add(new ByteSet {Value = new[] {(byte) i}});

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
                            buffer.Last().Expand((byte) i);
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
                        buffer.Last().Expand((byte) i);
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
                private static int GetL(double Prob)
                {
                    //неэффективно. Лучше использовать логарифм, но чет так впадлу округлять
                    int Result;
                    for (Result = 1; Math.Pow(2, -Result) > Prob; Result++)
                    {
                    }

                    return Result;

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
                        Result[i] = new ByteSet(DoubleFractToString(Sum, GetL(Probs[i])));
                        Sum += Probs[i];
                    }
                    AverageLength += Probabilities.Select((t, i) => t * Result[i].Length).Sum();
                    return Result;
                }
            }

            /// <summary>
            /// Возвращает дробную часть вещественного числа в двоичном представлении.
            /// </summary>
            /// <param name="Source">Вещественное число.</param>
            /// <param name="Length">Необходимое число знаков после запятой.</param>
            /// <returns></returns>
            private static string DoubleFractToString(double Source, int Length)
            {
                Source -= (int)Source;
                StringBuilder Result = new StringBuilder();
                while (Result.Length < Length)
                {
                    Source *= 2;
                    if (Source > 1)
                    {
                        Source -= 1;
                        Result.Append('1');
                    }
                    else
                    {
                        Result.Append('0');
                    }
                }

                //Если набрали меньше символов, чем в заданной длине, то дополняем до неё.
                if (Result.Length < Length)
                    Result.Append(new char[Length - Result.Length].Select(ch => '0').ToArray());

                //отдаем ответ.
                return Result.ToString();
            }

            public static class GilbertCoding
            {
                public static ByteSet[] GetCodes(double[] Probabilities, out double AverageLength)
                {

                    AverageLength = 0;
                    double[] Q=new double[Probabilities.Length];
                    ByteSet[] Result = new ByteSet[Probabilities.Length];

                    for (int i = 0; i < Q.Length; i++)
                    {
                        Q[i] += Probabilities[i]/2;
                        Result[i]=new ByteSet(DoubleFractToString(Q[i],(int)-Math.Log(Probabilities[i],2)+1));
                        for (int j = i + 1; j < Probabilities.Length; j++)
                            Q[j] += Probabilities[j];
                    }

                    AverageLength = Result.Select((res, ind) => res.Length * Probabilities[ind]).Sum();
                    return Result.OrderBy(set => set.Length).ToArray();
                }
            }
        }
    }
}