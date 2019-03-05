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
            /// Зранит в себе историю всех значений, начиная с вектора инициализации.
            /// </summary>
            public Queue<int> History;

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
            public MLFSR(string Source,int module, int[] initializevector)
            {
                InitializeVector=new Vector(initializevector.Select(val=>(double)val).ToArray());
                History = new Queue<int>(initializevector);
                Module = module;
                Values = new Queue<int>(initializevector);
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
            /// Сбрасывает данный линейный регистр.
            /// </summary>
            public void Reset()
            {
                Values=new Queue<int>(InitializeVector.Coordinates.Select(val=>(int)val));
                History = new Queue<int>(InitializeVector.Coordinates.Select(val => (int)val));
            }

            /// <summary>
            /// Период данной ЛРП. Сбрасывает регистр.
            /// </summary>
            public int Period
            {
                get
                {
                    throw new NotImplementedException();
                    Console.WriteLine($"T <= {Math.Pow(Module,Formula.Length)-1}");
                    return 0;
                }
            }

            /// <summary>
            /// Вычисляет следующий член последовательности и осуществляет сдвиг.
            /// </summary>
            /// <returns></returns>
            public int GetNext()
            {
                Comparison.LinearComparison newvalue = new Comparison.LinearComparison(0,Module);
                int i = 0;
                foreach (int val in Values)
                {
                    newvalue.A += val * Formula[i].A;
                    i++;
                }
                Values.Dequeue();
                Values.Enqueue((int)newvalue.A);
                History.Enqueue((int)newvalue.A);
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
