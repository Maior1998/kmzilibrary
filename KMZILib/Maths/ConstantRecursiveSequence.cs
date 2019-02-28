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
            public int[] Formula;

            public Queue<int> Values;

            private int FirstIndex = 0;
            

            /// <summary>
            /// Инициализирует новый регистр сдвига по его строковому представлению (например, an+4=an+3 + 5an+2 - 3an).
            /// </summary>
            /// <param name="Source"></param>
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

            public string CurrentState
            {
                get { return string.Join("; ",Values.Select((val, ind) => $"a[{ind + FirstIndex}]={val}")); }
            }

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

            public override string ToString()
            {
                return
                    $"an+{Formula.Length} = {string.Join(" + ", Formula.Select((element, index) => element == 0 ? "" : $"{element}an+{index}").Where(str => str != "").Reverse())}";
            }
        }
    }
}
