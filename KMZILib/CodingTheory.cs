using System;
using System.Collections.Generic;
using System.Linq;

namespace KMZILib
{
    /// <summary>
    /// Класс, отвечающий за работу функций теории кодирования
    /// </summary>
    public static class CodingTheory
    {
        /// <summary>
        /// Представляет собой двоичный набор данных
        /// </summary>
        public class ByteSet
        {
            /// <summary>
            /// Значение текущего бинарного вектора
            /// </summary>
            public byte[] Value { get; internal set; }

            internal void Expand(byte value)
            {
                Value = new List<byte>(Value) {value}.ToArray();
            }

            /// <summary>
            /// Строковое представление вектора
            /// </summary>
            public string StringValue =>string.Join("", Value);

            /// <summary>
            /// Длина вектора (число байтов)
            /// </summary>
            public int Length => Value.Length;

            /// <summary>
            /// Инициализирует пустой вектор
            /// </summary>
            public ByteSet()
            {
                Value = new byte[0];
            }

            /// <summary>
            /// Инициализирует вектор, представляющий заданный массив байтов
            /// </summary>
            /// <param name="value"></param>
            public ByteSet(byte[] value)
            {
                Value = new byte[value.Length];
                value.CopyTo(Value,0);
            }

            /// <summary>
            /// Инициализирует копию указанного вектора
            /// </summary>
            /// <param name="Other"></param>
            public ByteSet(ByteSet Other) : this(Other.Value)
            {
            }

            /// <summary>
            /// Строковое представление байтового ветора
            /// </summary>
            /// <returns>Строка, представляющая все байты ветокра</returns>
            public override string ToString()
            {
                return StringValue;
            }
        }
        /// <summary>
        /// Коды, осуществляющие сжатие данных
        /// </summary>
        public static class DataCompressionCodes
        {
            /// <summary>
            /// Представляет код Хаффмана
            /// </summary>
            public static class HuffmanCoding
            {
                /// <summary>
                /// Осуществляет генерацию кодов, подходящим заданным частотам там, чтобы добиться наименьшей средней длины сообщения
                /// </summary>
                /// <param name="Probabilities">Массив - частотный анализ исходного алфавита</param>
                /// <param name="k">Система счисления, в которой будет произведено кодирование</param>
                /// <param name="AverageLength">Вычисленная в процессе генерации кодов средняя длина сообщения</param>
                /// <returns>Массив <see cref="ByteSet"/>[], содержащий в себе коды, расположенные в соответствии введенным частотам в порядке убывания. </returns>
                public static ByteSet[] Encode(double[] Probabilities, int k, out double AverageLength)
                {
                    AverageLength = 0;
                    if (k >= Probabilities.Length)
                    {
                        ByteSet[] answer= new ByteSet[Probabilities.Length];
                        for (int i = 0; i < Probabilities.Length; i++)
                        {
                            answer[i] = new ByteSet() {Value = new[] {(byte) i}};
                            AverageLength += Probabilities[i];
                        }
                        return answer;
                    }
                        
                    //k - это число частей, на которые нам необходимо разбивать вероятности
                    double[] ProbCop = new double[Probabilities.Length];
                    Probabilities.CopyTo(ProbCop,0);
                    Array.Sort(ProbCop,(d, d1) => (int)(d1-d));
                    //отсортировали свою копию массива в порядке убывания вероятностей

                    Stack<int> Insertions = new Stack<int>();
                    while (ProbCop.Length > k)
                    {
                        double Sum = ProbCop.Skip(ProbCop.Length - k).Sum();
                        Sum = Math.Round(Sum, 3);
                        //вычислили сумму последних k элементов

                        //надо определить, на какой индекс будем вставлять
                        int Index = 0;
                        int ResultLength = ProbCop.Length - k + 1;
                        while (ProbCop[Index] >= Sum && Index < ResultLength) Index++;
                        //нашли индекс, на который необходимо вставить полученную сумму

                        Insertions.Push(Index);
                        List<double>bufferProb = new List<double>(ProbCop);
                        bufferProb.Insert(Index,Sum);
                        ProbCop = bufferProb.Take(ResultLength).ToArray();
                    }
                    List<ByteSet> Answer = new List<ByteSet>();
                    for(int i=0;i<ProbCop.Length;i++)
                        Answer.Add(new ByteSet{Value = new[]{(byte)i}});
                    while (Insertions.Count != 0)
                    {
                        int Moving = Insertions.Pop();
                        //узнали о перестановке
                        List<ByteSet> buffer = new List<ByteSet>(Answer);
                        buffer.RemoveAt(Moving);
                        for (int i = 0; i < k; i++)
                        {
                            buffer.Add(new ByteSet(Answer[Moving]));
                            buffer.Last().Expand((byte)i);
                        }
                        Answer = buffer;
                    }

                    AverageLength += Probabilities.Select((t, i) => t * Answer[i].Length).Sum();
                    return Answer.ToArray();
                }
            }
        }
    }
}
