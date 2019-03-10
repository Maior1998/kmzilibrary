using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KMZILib
{
    /// <summary>
    /// Статический класс, предоставляющий методы для работы с линейными рекуррентными последовательностями (Constant-Recursive Sequence).
    /// </summary>
    public static class CRS
    {
        /// <summary>
        /// Представляет собой регистр сдвига с линейной обратной связью (Linear-feedback shift register).
        /// </summary>
        public class LFSR
        {
            /// <summary>
            /// Представляет формулу n-ого члена последовательности в виде массива коэффициентов.
            /// </summary>
            public int[] Formula;

            /// <summary>
            /// Представляет очередь имеющихся значений последовательности. 
            /// </summary>
            public Queue<int> Values;

            private int FirstIndex = 0;

            /// <summary>
            /// Инициализирует новый регистр сдвига по его строковому представлению (например, an+4=an+3 + 5an+2 - 3an).
            /// </summary>
            /// <param name="Source"></param>
            /// <param name="InitializeVector"></param>
            public LFSR(string Source, int[] InitializeVector)
            {
                Values = new Queue<int>(InitializeVector);
                Regex LFSRPartRegex = new Regex(@"(?<sign>-)?\s*(?<value>\d+)?\s*a\s*n\s*(?:\+\s*(?<index>\d+))?");
                Match[] LFSRParts = LFSRPartRegex.Matches(Source).Cast<Match>().ToArray();
                Formula = new int[Convert.ToInt32(LFSRParts[0].Groups["index"].Value)];
                if (Formula.Length != InitializeVector.Length)
                    throw new InvalidOperationException($"Число элементов, участвующих в операциях ({Formula.Length}) не равно размеру начального вектора ({InitializeVector.Length})");
                for (int i = 1; i < LFSRParts.Length; i++)
                {
                    //Считали индекс, в который необходимо поместить коэффициент
                    int index = LFSRParts[i].Groups["index"].Value != "" ? Convert.ToInt32(LFSRParts[i].Groups["index"].Value) : 0;
                    int value = LFSRParts[i].Groups["value"].Value == ""
                        ? 1
                        : Convert.ToInt32(LFSRParts[i].Groups["value"].Value);
                    if (LFSRParts[i].Groups["sign"].Value != "")
                        value *= -1;
                    Formula[index] = value;
                }
            }

            /// <summary>
            /// Строковое представление текущих значений в регистре сдвига.
            /// </summary>
            public string CurrentState
            {
                get { return string.Join("; ",Values.Select((val, ind) => $"a[{ind + FirstIndex}]={val}")); }
            }

            /// <summary>
            /// Вычисляет следующий член последовательности и осуществляет сдвиг.
            /// </summary>
            /// <returns></returns>
            public int GetNext()
            {
                int newvalue = 0;
                int i = 0;
                foreach (int val in Values)
                {
                    newvalue += val * Formula[i];
                    i++;
                }
                Values.Dequeue();
                Values.Enqueue(newvalue);
                FirstIndex++;
                return newvalue;
            }

            /// <summary>
            /// Возвращает строковое представление формулы общего члена данной ЛРП.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return
                    $"an+{Formula.Length} = {string.Join(" + ", Formula.Select((element, index) => element == 0 ? "" : $"{element}an+{index}").Where(str => str != "").Reverse())}";
            }
        }

        /// <summary>
        /// Представляет собой модульный регистр сдвига с линейной обратной связью (Modular Linear-feedback shift register).
        /// </summary>
        public class MLFSR
        {
            /// <summary>
            /// Представляет формулу n-ого члена последовательности в виде массива коэффициентов.
            /// </summary>
            public Comparison.LinearComparison[] Formula;

            /// <summary>
            /// Представляет очередь имеющихся значений последовательности. 
            /// </summary>
            public Queue<int> Values;

            /// <summary>
            /// Хранит в себе историю всех значений, начиная с вектора инициализации.
            /// </summary>
            public Queue<int> ValuesHistory;

            /// <summary>
            /// Хранит в себе историю всех значений в виде вектором-состояний, начиная с вектора инициализации.
            /// </summary>
            public Queue<Vector> StatesHistory;

            /// <summary>
            /// Модуль, на котором работает данный линейный регистр.
            /// </summary>
            public int Module;

            private int FirstIndex = 0;

            /// <summary>
            /// Вектор инициализации данного регистра и ЛРП.
            /// </summary>
            public readonly Vector InitializeVector;

            /// <summary>
            /// Инициализирует новый модульный регистр сдвига по его строковому представлению (например, an+4=an+3 + 5an+2 - 3an).
            /// </summary>
            /// <param name="Source"></param>
            /// <param name="module"></param>
            /// <param name="initializevector"></param>
            public MLFSR(int[] Source, int module, int[] initializevector)
            {
                InitializeVector = new Vector(initializevector.Select(val => (double)val).ToArray());
                ValuesHistory = new Queue<int>(initializevector);
                StatesHistory = new Queue<Vector>();
                Module = module;
                Formula = Source.Reverse().Select(val => new Comparison.LinearComparison(val, Module)).ToArray();
                Values = new Queue<int>(initializevector);
                StatesHistory.Enqueue(new Vector(Values.Select(val => (double)val).ToArray()));
                if (Formula.Length != initializevector.Length)
                    throw new InvalidOperationException($"Число элементов, участвующих в операциях ({Formula.Length}) не равно размеру начального вектора ({initializevector.Length})");
                }

            /// <summary>
            /// Инициализирует новый модульный регистр сдвига по его строковому представлению (например, an+4=an+3 + 5an+2 - 3an).
            /// </summary>
            /// <param name="Source"></param>
            /// <param name="module"></param>
            /// <param name="initializevector"></param>
            public MLFSR(string Source,int module, int[] initializevector)
            {
                InitializeVector=new Vector(initializevector.Select(val=>(double)val).ToArray());
                ValuesHistory = new Queue<int>(initializevector);
                StatesHistory=new Queue<Vector>();
                Module = module;
                Values = new Queue<int>(initializevector);
                StatesHistory.Enqueue(new Vector(Values.Select(val => (double)val).ToArray()));
                Regex LFSRPartRegex = new Regex(@"(?<sign>-)?\s*(?<value>\d+)?\s*a\s*n\s*(?:\+\s*(?<index>\d+))?");
                Match[] LFSRParts = LFSRPartRegex.Matches(Source).Cast<Match>().ToArray();
                Formula = new Comparison.LinearComparison[Convert.ToInt32(LFSRParts[0].Groups["index"].Value)].Select(comp=>new Comparison.LinearComparison(0,Module)).ToArray();
                if (Formula.Length != initializevector.Length)
                    throw new InvalidOperationException($"Число элементов, участвующих в операциях ({Formula.Length}) не равно размеру начального вектора ({initializevector.Length})");
                for (int i = 1; i < LFSRParts.Length; i++)
                {
                    //Считали индекс, в который необходимо поместить коэффициент
                    int index = LFSRParts[i].Groups["index"].Value != "" ? Convert.ToInt32(LFSRParts[i].Groups["index"].Value) : 0;
                    int value = LFSRParts[i].Groups["value"].Value == ""
                        ? 1
                        : Convert.ToInt32(LFSRParts[i].Groups["value"].Value);
                    if (LFSRParts[i].Groups["sign"].Value != "")
                        value *= -1;
                    Formula[index].A=value;
                }
            }

            /// <summary>
            /// Строковое представление текущих значений в регистре сдвига.
            /// </summary>
            public string CurrentState
            {
                get { return string.Join("; ", Values.Select((val, ind) => $"a[{ind + FirstIndex}]={val}")); }
            }

            /// <summary>
            /// Возвращает графическое ASCII представление регистра. Для отображения лучше использовать монотонный шрифты. Например, Consolas или Liquida Console.
            /// </summary>
            /// <returns></returns>
            public string Draw()
            {

                /*
                   ╭───────────────►╢+╟─────►╢+╟──────╮
                   │                 ▲        ▲       │
                   ╧                 ╧        ╧       │
                   3                 5        1       │
                   ╤                 ╤        ╤       │
                 ◄─┴╢ An ╟◄──╢An+1╟◄─┴╢An+2╟◄─┴╢An+3╟◄┘
                 */
                string[] Result = new string[6];
                //Итог будет 6 строк в высоту.

                //В этот список будем записывать индексы вертикальных черт 
                List<int> VerticalsIndexes = new List<int>();
                //это массив ненулевых коэффициентов формулы
                int[] NonZeroValues = Formula.Select(comp => (int)comp.A).Where(val => val != 0).ToArray();

                //Сначала заполняем последнюю строку.
                StringBuilder buffer=new StringBuilder();
                for (int i = 0; i < Formula.Length; i++)
                {
                    if(Formula[i].A != 0)
                        VerticalsIndexes.Add(buffer.Length+2);
                    buffer.Append($"◄─{(Formula[i].A==0? "─" : "┴")}─╢An{(i==0?"":$"+{i}")}╟");
                }
                buffer.Append("◄┘");
                Result[5] = buffer.ToString();
                buffer.Clear();
                //Запонили последнюю строку и массив индексов
                int Length = Result[5].Length;

                //Теперь заполняем предпоследнюю строку (4)
                buffer.Append(string.Join("", new char[Length].Select(ch => ' ')));
                buffer[buffer.Length - 1] = '│';
                for(int i=0;i<VerticalsIndexes.Count;i++)
                    buffer[VerticalsIndexes[i]] = NonZeroValues[i] == 1? '│' : '╤';
                Result[4] = buffer.ToString();
                buffer.Clear();
                //заполнили предпоследнюю строку

                //строка с индексом 3
                buffer.Append(string.Join("", new char[Length].Select(ch => ' ')));
                buffer[buffer.Length - 1] = '│';
                for (int VertIndex = 0; VertIndex < VerticalsIndexes.Count; VertIndex++)
                    buffer[VerticalsIndexes[VertIndex]] = NonZeroValues[VertIndex] == 1 ? '│' : NonZeroValues[VertIndex].ToString()[0];
                Result[3] = buffer.ToString();
                buffer.Clear();

                //строка с индексом 2
                buffer.Append(string.Join("", new char[Length].Select(ch => ' ')));
                buffer[buffer.Length - 1] = '│';
                for (int i = 0; i < VerticalsIndexes.Count; i++)
                    buffer[VerticalsIndexes[i]] = NonZeroValues[i] == 1 ? '│' : '╧';
                Result[2] = buffer.ToString();
                buffer.Clear();

                //строка с индексом 1
                buffer.Append(string.Join("", new char[Length].Select(ch => ' ')));
                buffer[buffer.Length - 1] = '│';
                for (int i = 0; i < VerticalsIndexes.Count; i++)
                    buffer[VerticalsIndexes[i]] = '▲';
                Result[1] = buffer.ToString();
                buffer.Clear();

                //строка 0
                buffer.Append(string.Join("",new char[Length].Select(ch => '─')));
                buffer[VerticalsIndexes.First()] = '╭';
                for (int i = 0; i < VerticalsIndexes.First(); i++)
                    buffer[i] = ' ';
                buffer[buffer.Length - 1] = '╮';
                foreach (int plusindex in VerticalsIndexes.Skip(1))
                {
                    buffer[plusindex - 2] = '►';
                    buffer[plusindex - 1] = '╢';
                    buffer[plusindex] = '+';
                    buffer[plusindex + 1] = '╟';
                }
                Result[0] = buffer.ToString();
                buffer.Clear();
                return string.Join("\n",Result);
            }

            /// <summary>
            /// Текущее состояние линейного регистра сдвига в виде вектора.
            /// </summary>
            public Vector StateVector
            {
                get { return new Vector(Values.Select(val => (double) val).ToArray()); }
            }

            /// <summary>
            /// Сбрасывает данный линейный регистр.
            /// </summary>
            public void Reset()
            {
                Values=new Queue<int>(InitializeVector.Coordinates.Select(val=>(int)val));
                ValuesHistory = new Queue<int>(Values);
                StatesHistory= new Queue<Vector>(new []{new Vector(Values.Select(val => (double)val).ToArray())});
                FirstIndex = 0;
            }

            /// <summary>
            /// Период данной ЛРП. Сбрасывает регистр.
            /// </summary>
            public int Period
            {
                get
                {
                    Reset();
                    for (int i = 0; i < (int)Math.Pow(Module, Formula.Length) - 1; i++)
                    {
                        ValuesHistory.Enqueue(GetNext(false));
                        Vector current = StateVector;
                        if (StatesHistory.Contains(current))
                        {
                            Reset();
                            return i + 1;
                        }
                        StatesHistory.Enqueue(current);
                    }
                    return -1;
                }
            }

            /// <summary>
            /// Вычисляет следующий член последовательности и осуществляет сдвиг.
            /// </summary>
            /// <returns></returns>
            public int GetNext(bool AddToHistory=true)
            {
                Comparison.LinearComparison newvalue = new Comparison.LinearComparison(0, Module);
                int i = 0;
                foreach (int val in Values)
                {
                    newvalue.A += val * Formula[i].A;
                    i++;
                }
                Values.Dequeue();
                Values.Enqueue((int)newvalue.A);
                if (AddToHistory)
                {
                    ValuesHistory.Enqueue((int) newvalue.A);
                    StatesHistory.Enqueue(new Vector(Values.Select(val => (double)val).ToArray()));
                }

                FirstIndex++;
                return (int)newvalue.A;
            }

            /// <summary>
            /// Возвращает строковое представление формулы общего члена данной ЛРП.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return
                    $"an+{Formula.Length} = {string.Join(" + ", Formula.Select((element, index) => element.A == 0 ? "" : $"{element.A}an+{index}").Where(str => str != "").Reverse())}";
            }
        }
    }
}
