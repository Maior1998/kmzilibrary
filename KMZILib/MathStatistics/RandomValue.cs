using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMZILib
{
    /// <summary>
    /// Класс, представляющий данные и методы для работы со случайными величинами.
    /// </summary>
    public static class MathStatistics
    {
        /// <summary>
        /// Представляет собой случайную величину.
        /// </summary>
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

            /// <summary>
            /// Среднее значение последовательности.
            /// </summary>
            public double Average=>Statistic.Select(row => row.Key * row.Value).Sum() / Count;

            /// <summary>
            /// Масимальный элемент последовательности.
            /// </summary>
            public double Max => Values.Max();

            /// <summary>
            /// Максимальный по модулю элемент последовательности.
            /// </summary>
            public double MaxAbs => Values[Values.Select(Math.Abs).ToList().IndexOf(Values.Select(Math.Abs).Max())];

            /// <summary>
            /// Минимальный элемент последовательности.
            /// </summary>
            public double Min => Values.Min();

            /// <summary>
            /// Минимальный по модулю элемент последовательности.
            /// </summary>
            public double MinAbs => Values[Values.Select(Math.Abs).ToList().IndexOf(Values.Select(Math.Abs).Min())];

            /// <summary>
            /// Возвращает длину интервала. Вычисляется как <see cref="Max"/> - <see cref="Min"/>.
            /// </summary>
            public double Interval => Max - Min;

            /// <summary>
            /// Мода случайной величины. Если есть несколько одинаково-часто-встречающихся величин, вернется первая из них.
            /// </summary>
            public double Mean => Statistic.First(val=>val.Value==Statistic.Values.Max()).Key;

            /// <summary>
            /// Определяет, является ли величина мультимодальной.
            /// </summary>
            public bool IsMultiModal => Statistic.Count(val => val.Value == Statistic.Values.Max()) > 1;

            /// <summary>
            /// Дисперсия данной величины.
            /// </summary>
            public double Dispersion
            {
                get
                {
                    double Result = 0;
                    foreach (double probsKey in Statistic.Keys)
                        Result += Math.Pow(probsKey - Average, 2) * Statistic[probsKey];
                    return Result / Count;
                }
            }

            /// <summary>
            /// Стандартное отклонение данной величины.
            /// </summary>
            public double StandardDeviation => Math.Sqrt(Dispersion);


        }

    }
}
