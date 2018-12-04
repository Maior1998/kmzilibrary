using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            public void Expand(byte value)
            {
                Value = new List<byte>(Value) {value}.ToArray();
            }

            public string StringValue =>string.Join("", Value);

            public int Length => Value.Length;

            public ByteSet()
            {

            }

            public ByteSet(byte[] value)
            {
                Value = new byte[value.Length];
                value.CopyTo(Value,0);
            }

            public ByteSet(ByteSet Other) : this(Other.Value)
            {

            }

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
            public static class HuffmanCoding
            {
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

                    Stack<SumMoving> Insertions = new Stack<SumMoving>();
                    while (ProbCop.Length > k)
                    {
                        double Sum = ProbCop.Skip(ProbCop.Length - k).Sum();
                        //вычислили сумму последних k элементов

                        //надо определить, на какой индекс будем вставлять
                        int Index = 0;
                        int ResultLength = ProbCop.Length - k + 1;
                        //TODO: точно меньше?
                        while (ProbCop[Index] >= Sum && Index < ResultLength) Index++;
                        //нашли индекс, на который необходимо вставить полученную сумму

                        Insertions.Push(new SumMoving {From = ProbCop.Length - k, To = Index});
                        List<double>bufferProb = new List<double>(ProbCop);
                        bufferProb.Insert(Index,Sum);
                        ProbCop = bufferProb.Take(ResultLength).ToArray();
                    }
                    /*Этот процесс работает корректно за исключением того,
                     *что вставки должны оформляться немного по другому
                     *
                     * Необходимо указывать, повышается или повышается сумма
                     * Но как вообще определить повышается она или понижается?
                    */
                    List<ByteSet> Answer = new List<ByteSet>();
                    for(int i=0;i<ProbCop.Length;i++)
                        Answer.Add(new ByteSet{Value = new[]{(byte)i}});
                    while (Insertions.Count != 0)
                    {
                        SumMoving Moving = Insertions.Pop();
                        //узнали о перестановке
                        List<ByteSet> buffer = new List<ByteSet>(Answer);
                        buffer.RemoveAt(Moving.To);
                        for (int i = 0; i < k; i++)
                        {
                            buffer.Add(new ByteSet(Answer[Moving.To]));
                            buffer[buffer.Count-1].Expand((byte)i);
                        }

                        Answer = buffer;
                    }

                    AverageLength += Probabilities.Select((t, i) => t * Answer[i].Length).Sum();
                    return Answer.ToArray();
                }

                private class SumMoving
                {
                    public int From;
                    public int To;


                    public bool IsIncreased => From - To > 0;
                    public override string ToString()
                    {
                        return $"{From} -> {To}";
                    }
                }
            }
        }
    }
}
