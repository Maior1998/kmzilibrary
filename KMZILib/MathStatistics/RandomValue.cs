using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMZILib
{
    public static class MathStatistics
    {
        public class RandomValue
        {
            /// <summary>
            /// Сырой список значений данной случайной величины. По сути, просто числовой ряд.
            /// </summary>
            public readonly IList<double> Values;

            /// <summary>
            /// Список вероятностей данной случайной величины.
            /// </summary>
            public IList<double> Probs=>Statistic.Values.Select(value=>(double)value/Count).ToList();

            /// <summary>
            /// Число всех элементов данной случайной величины. Повторы тоже учитываются.
            /// </summary>
            public int Count => Statistic.Values.Sum();

            /// <summary>
            /// Сумма всех значений данной случайной величины.
            /// </summary>
            public double Sum => Statistic.Select(row => row.Key * row.Value).Sum();

            /// <summary>
            /// Статистика данной случайной величины. Ключи - значения, которые принимала случайная величина. Значения - число раз, когда случайная величина принимала такие значения.
            /// </summary>
            public readonly Dictionary<double, int> Statistic;

            /// <summary>
            /// Инициализирует новую случайную величину по имеющемуся набору её значений. Вероятности/Частоты рассчитываются автоматически исходя из этого списка.
            /// </summary>
            /// <param name="values"></param>
            public RandomValue(IEnumerable<double> values)
            {
                Values = new List<double>(values);
                Statistic=new Dictionary<double, int>();
                foreach (double key in Values)
                {
                    if (!Statistic.ContainsKey(key)) Statistic.Add(key, 0);
                    Statistic[key]++;
                }
            }
            
            /// <summary>
            /// Инициализирует новую случайную величину по имеющеейся статистике, но без самого ряда значений.
            /// </summary>
            /// <param name="values"></param>
            public RandomValue(Dictionary<double, int> statistic)
            {
                Statistic = new Dictionary<double, int>();
                foreach (KeyValuePair<double, int> pair in statistic)
                    Statistic.Add(pair.Key, pair.Value);
            }

            /// <summary>
            /// Инициализирует новую случайную величину по имеющеейся статистике и ряду значений.
            /// </summary>
            /// <param name="values"></param>
            public RandomValue(IList<double> values, Dictionary<double, int> statistic)
            {
                Values = new List<double>(values);
                Statistic = new Dictionary<double, int>();
                foreach (KeyValuePair<double, int> pair in statistic)
                    Statistic.Add(pair.Key, pair.Value);
            }


        }

    }
}
