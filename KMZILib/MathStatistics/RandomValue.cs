using System;
using System.Collections.Generic;
using System.Linq;

namespace KMZILib
{
    /// <summary>
    ///     Класс, представляющий данные и методы для работы со случайными величинами.
    /// </summary>
    public static class MathStatistics
    {
        /// <summary>
        ///     Представляет собой случайную величину.
        /// </summary>
        public class RandomValue
        {
            /// <summary>
            ///     Список вероятностей данной случайной величины. Ключи - значения, которые принимала случайная величина. Значения -
            ///     вероятность того, что величина примет это значение.
            /// </summary>
            public readonly Dictionary<double, double> Probs;

            /// <summary>
            ///     Статистика данной случайной величины. Ключи - значения, которые принимала случайная величина. Значения - число раз,
            ///     когда случайная величина принимала такие значения.
            /// </summary>
            public readonly Dictionary<double, int> Statistic;

            /// <summary>
            ///     Сырой список значений данной случайной величины. По сути, просто числовой ряд.
            /// </summary>
            public readonly double[] Values;

            /// <summary>
            ///     Инициализирует новую случайную величину по имеющемуся набору её значений. Вероятности/Частоты рассчитываются
            ///     автоматически исходя из этого списка.
            /// </summary>
            /// <param name="values"></param>
            public RandomValue(IEnumerable<double> values)
            {
                Values = new List<double>(values).ToArray();
                Statistic = new Dictionary<double, int>();
                foreach (double key in Values)
                {
                    if (!Statistic.ContainsKey(key))
                        Statistic.Add(key, 0);
                    Statistic[key]++;
                }

                Probs = new Dictionary<double, double>();
                foreach (KeyValuePair<double, int> row in Statistic)
                    Probs.Add(row.Key, (double)row.Value / Count);
            }

            /// <summary>
            ///     Инициализирует новую случайную величину по имеющеейся статистике, но без самого ряда значений.
            /// </summary>
            /// <param name="statistic"></param>
            public RandomValue(Dictionary<double, int> statistic)
            {
                Statistic = new Dictionary<double, int>(statistic);
                Values = statistic.Keys.ToArray();
                Probs = new Dictionary<double, double>();
                foreach (KeyValuePair<double, int> row in Statistic)
                    Probs.Add(row.Key, (double)row.Value / Count);
            }

            /// <summary>
            ///     Инициализирует новую случайную величину по имеющемуся набору вероятностей, но без статистических данных.
            /// </summary>
            /// <param name="probs"></param>
            public RandomValue(Dictionary<double, double> probs)
            {
                Probs = new Dictionary<double, double>(probs);
                Values = probs.Keys.ToArray();
            }

            /// <summary>
            ///     Инициализирует новую случайную величину по имеющемуся набору вероятностей и набору значений.
            /// </summary>
            /// <param name="values"></param>
            /// <param name="probs"></param>
            public RandomValue(IEnumerable<double> values, Dictionary<double, double> probs)
            {
                Values = new List<double>(values).ToArray();
                Probs = new Dictionary<double, double>(probs);
                Values = probs.Keys.ToArray();
            }

            /// <summary>
            ///     Инициализирует новую случайную величину по имеющеейся статистике и ряду значений.
            /// </summary>
            /// <param name="values"></param>
            /// <param name="statistic"></param>
            public RandomValue(IList<double> values, Dictionary<double, int> statistic)
            {
                Values = values.ToArray();
                Statistic = new Dictionary<double, int>(statistic);
                Probs = new Dictionary<double, double>();
                foreach (KeyValuePair<double, int> row in Statistic)
                    Probs.Add(row.Key, (double)row.Value / Count);
            }

            /// <summary>
            ///     Число всех элементов данной случайной величины. Повторы тоже учитываются.
            /// </summary>
            public int Count => Statistic.Values.Sum();

            /// <summary>
            ///     Сумма всех значений данной случайной величины. Для вычисления необходима частотная статистика
            ///     <see cref="Statistic" />.
            /// </summary>
            public double Sum => Statistic.Select(row => row.Key * row.Value).Sum();

            /// <summary>
            ///     Среднее значение последовательности.
            /// </summary>
            public double Average => Statistic.Select(row => row.Key * row.Value).Sum() / Count;

            /// <summary>
            ///     Масимальный элемент последовательности.
            /// </summary>
            public double Max => Values.Max();

            /// <summary>
            ///     Максимальный по модулю элемент последовательности.
            /// </summary>
            public double MaxAbs => Values[Values.Select(Math.Abs).ToList().IndexOf(Values.Select(Math.Abs).Max())];

            /// <summary>
            ///     Минимальный элемент последовательности.
            /// </summary>
            public double Min => Values.Min();

            /// <summary>
            ///     Минимальный по модулю элемент последовательности.
            /// </summary>
            public double MinAbs => Values[Values.Select(Math.Abs).ToList().IndexOf(Values.Select(Math.Abs).Min())];

            /// <summary>
            ///     Возвращает длину интервала. Вычисляется как <see cref="Max" /> - <see cref="Min" />.
            /// </summary>
            public double Interval => Max - Min;

            /// <summary>
            ///     Мода случайной величины. Если есть несколько одинаково-часто-встречающихся величин, вернется первая из них.
            /// </summary>
            public double Mean => Statistic.First(val => val.Value == Statistic.Values.Max()).Key;

            /// <summary>
            ///     Определяет, является ли величина мультимодальной.
            /// </summary>
            public bool IsMultiModal => Statistic.Count(val => val.Value == Statistic.Values.Max()) > 1;

            /// <summary>
            ///     Дисперсия данной величины.
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
            ///     Стандартное отклонение данной величины.
            /// </summary>
            public double StandardDeviation => Math.Sqrt(Dispersion);

            /// <summary>
            /// Стандартная ошибка данной величины.
            /// </summary>
            public double StandartError => Dispersion / Math.Sqrt(Count);

            public double StandartErrorGeneral => StandardDeviation / Math.Sqrt(Count);

            /// <summary>
            ///     Математическое ожидание данной случайной величины. Требует наличие списка вероятностей для вычисления.
            /// </summary>
            public double MathExeption => Probs.Select(row => row.Key * row.Value).Sum();

            /// <summary>
            ///     Коэффициент эксцесса данной случайной величины.
            /// </summary>
            public double Kurtosis => CentralMoment(4) / Math.Pow(Dispersion, 2) - 3;

            /// <summary>
            /// Коэффициент ассимметрии данной случайной величины.
            /// </summary>
            public double Skewness => CentralMoment(3) / Math.Pow(StandardDeviation, 3);

            /// <summary>
            /// Медиана данной случайной величины.
            /// </summary>
            public double Median
            {
                get
                {
                    double[] Sorted = new double[Values.Length];
                    Values.CopyTo(Sorted, 0);

                    Array.Sort(Sorted);

                    return Sorted.Length % 2 == 0 ?
                      (Sorted[Sorted.Length / 2] + Sorted[Sorted.Length / 2 - 1]) / 2.0
                    : Sorted[Sorted.Length / 2];
                }
            }

            /// <summary>
            ///     Возвращает начальный момент данной случайной величины заданного порядка.
            /// </summary>
            /// <param name="k"></param>
            /// <returns></returns>
            public double StartingMoment(int k)
            {
                Dictionary<double, double> buffer = new Dictionary<double, double>();
                foreach (KeyValuePair<double, double> pair in Probs)
                    buffer.Add(Math.Pow(pair.Key, k), pair.Value);
                return new RandomValue(buffer).MathExeption;
            }

            /// <summary>
            ///     Возвращает абсолютный начальный момент данной случайной величины заданного порядка.
            /// </summary>
            /// <param name="k"></param>
            /// <returns></returns>
            public double StartingAbsMoment(int k)
            {
                Dictionary<double, double> buffer = new Dictionary<double, double>();
                foreach (KeyValuePair<double, double> pair in Probs)
                    buffer.Add(Math.Pow(Math.Abs(pair.Key), k), pair.Value);
                return new RandomValue(buffer).MathExeption;
            }

            /// <summary>
            ///     Возвращает центральный момент данной случайной величины заданного порядка.
            /// </summary>
            /// <param name="k"></param>
            /// <returns></returns>
            public double CentralMoment(int k)
            {
                double currentexeption = MathExeption;
                Dictionary<double, double> buffer = new Dictionary<double, double>();
                //TODO: в словарь попадают пары с одинковыми ключами. Предположительно из-за того, что четная степень.
                foreach (KeyValuePair<double, double> pair in Probs)
                {
                    double key = Math.Pow(pair.Key * (1 - currentexeption), k);
                    if (!buffer.ContainsKey(key)) buffer.Add(key, 0);
                    buffer[key] += pair.Value;
                }

                return new RandomValue(buffer).MathExeption;
            }

            /// <summary>
            ///     Возвращает абсолютный центральный момент данной случайной величины заданного порядка.
            /// </summary>
            /// <param name="k"></param>
            /// <returns></returns>
            public double CentralAbsMoment(int k)
            {
                double currentexeption = MathExeption;
                Dictionary<double, double> buffer = new Dictionary<double, double>();
                foreach (KeyValuePair<double, double> pair in Probs)
                    buffer.Add(Math.Pow(Math.Abs(pair.Key * (1 - currentexeption)), k), pair.Value);
                return new RandomValue(buffer).MathExeption;
            }

            /// <summary>
            /// Возвращает перечисление всех значений случайной величины.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return string.Join(", ", Values);
            }
        }
    }
}