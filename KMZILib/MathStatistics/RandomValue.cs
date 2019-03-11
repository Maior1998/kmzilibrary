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
            #region Поля
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
            #endregion

            #region Конструкторы
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
            #endregion

            #region Свойства

            private int count = -1;

            /// <summary>
            ///     Число всех элементов данной случайной величины. Повторы тоже учитываются.
            /// </summary>
            public int Count => count == -1 ? count = Statistic.Values.Sum() : count;


            private double sum = -1;
            /// <summary>
            ///     Сумма всех значений данной случайной величины. Для вычисления необходима частотная статистика
            ///     <see cref="Statistic" />.
            /// </summary>
            public double Sum => sum == -1 ? sum = Statistic.Select(row => row.Key * row.Value).Sum() : sum;

            private double average = double.NaN;
            /// <summary>
            ///     Среднее значение последовательности.
            /// </summary>
            public double Average => double.IsNaN(average) ? average = Statistic.Select(row => row.Key * row.Value).Sum() / Count : average;

            private double max = double.NaN;
            /// <summary>
            ///     Масимальный элемент последовательности.
            /// </summary>
            public double Max => double.IsNaN(max) ? max = Values.Max() : max;

            private double maxabs = double.NaN;
            /// <summary>
            ///     Максимальный по модулю элемент последовательности.
            /// </summary>
            public double MaxAbs => double.IsNaN(maxabs) ? maxabs = Values[Values.Select(Math.Abs).ToList().IndexOf(Values.Select(Math.Abs).Max())] : maxabs;


            private double min = double.NaN;
            /// <summary>
            ///     Минимальный элемент последовательности.
            /// </summary>
            public double Min => double.IsNaN(min) ? min = Values.Min() : min;

            private double minabs = double.NaN;
            /// <summary>
            ///     Минимальный по модулю элемент последовательности.
            /// </summary>
            public double MinAbs => double.IsNaN(minabs) ? minabs = Values[Values.Select(Math.Abs).ToList().IndexOf(Values.Select(Math.Abs).Min())] : minabs;

            private double interval = double.NaN;
            /// <summary>
            ///     Возвращает длину интервала. Вычисляется как <see cref="Max" /> - <see cref="Min" />.
            /// </summary>
            public double Interval => double.IsNaN(interval) ? interval = Max - Min : interval;

            private double mean = double.NaN;
            /// <summary>
            ///     Мода случайной величины. Если есть несколько одинаково-часто-встречающихся величин, вернется первая из них.
            /// </summary>
            public double Mean => double.IsNaN(mean) ? mean = Statistic.First(val => val.Value == Statistic.Values.Max()).Key : mean;

            /// <summary>
            ///     Определяет, является ли величина мультимодальной.
            /// </summary>
            public bool IsMultiModal => Statistic.Count(val => val.Value == Statistic.Values.Max()) > 1;

            private double dispersion = double.NaN;
            /// <summary>
            ///     Дисперсия данной величины.
            /// </summary>
            public double Dispersion
            {
                get
                {
                    if (!double.IsNaN(dispersion))
                        return dispersion;
                    double Result = 0;
                    foreach (double probsKey in Statistic.Keys)
                        Result += Math.Pow(probsKey - Average, 2) * Statistic[probsKey];
                    dispersion = Result / (Count - 1);
                    return dispersion;
                }
            }

            /// <summary>
            /// Описательная статистика данной случайной величины.
            /// </summary>
            public string DescriptiveStatistics =>
                $"Среднее : {Average}\n" +
                $"Стандартная ошибка : {StandardErrorGeneral}\n" +
                $"Медиана : {Median}\n" +
                $"Мода : {Mean}\n" +
                $"Стандартное отклонение : {StandardDeviation}\n" +
                $"Дисперсия выборки : {Dispersion}\n" +
                $"Эксцесс : {Kurtosis}\n" +
                $"Ассиметричность : {Skewness}\n" +
                $"Интервал : {Interval}\n" +
                $"Минимум : {Min}\n" +
                $"Максимум : {Max}\n" +
                $"Сумма : {Sum}\n" +
                $"Счет : {Count}";


            private double standarddeviation = double.NaN;
            /// <summary>
            ///     Стандартное отклонение данной величины.
            /// </summary>
            public double StandardDeviation => double.IsNaN(standarddeviation) ? standarddeviation = Math.Sqrt(Dispersion) : standarddeviation;

            private double standarderror = double.NaN;
            /// <summary>
            /// Стандартная ошибка для выборки, когда n != N.
            /// </summary>
            public double StandardError => double.IsNaN(standarderror) ? standarderror = Dispersion / Math.Sqrt(Count) : standarderror;

            private double standarderrorgeneral = double.NaN;
            /// <summary>
            /// Стандартная ошибка для генеральной совокупности, когда n = N.
            /// </summary>
            public double StandardErrorGeneral => double.IsNaN(standarderrorgeneral) ? standarderrorgeneral = StandardDeviation / Math.Sqrt(Count) : standarderrorgeneral;

            private double mathexeption = double.NaN;
            /// <summary>
            ///     Математическое ожидание данной случайной величины. Требует наличие списка вероятностей для вычисления.
            /// </summary>
            public double MathExeption => double.IsNaN(mathexeption) ? mathexeption = Probs.Select(row => row.Key * row.Value).Sum() : mathexeption;

            private double kurtosis = double.NaN;
            /// <summary>
            ///     Коэффициент эксцесса данной случайной величины.
            /// </summary>
            public double Kurtosis
            {
                get
                {
                    if (!double.IsNaN(kurtosis))
                        return kurtosis;
                    double n = Count;
                    double fdel = (n * (n + 1)) / ((n - 1) * (n - 2) * (n - 3));
                    double sum = Values.Select(val => Math.Pow((val - Average) / StandardDeviation, 4)).Sum();
                    double sdel = (3 * Math.Pow(n - 1, 2)) / ((n - 2) * (n - 3));
                    return kurtosis = fdel * sum - sdel;
                }
            }

            private double skewness = double.NaN;
            /// <summary>
            /// Коэффициент ассимметрии данной случайной величины.
            /// </summary>
            public double Skewness
            {
                get
                {
                    if (!double.IsNaN(skewness))
                        return skewness;
                    double n = Count;
                    double fdel = n / ((n - 1) * (n - 2));
                    double sum = Values.Select(val => Math.Pow((val - Average) / StandardDeviation, 3)).Sum();
                    return skewness = fdel * sum;
                }
            }

            private double median = double.NaN;
            /// <summary>
            /// Медиана данной случайной величины.
            /// </summary>
            public double Median
            {
                get
                {
                    if (!double.IsNaN(median))
                        return median;
                    double[] Sorted = new double[Values.Length];
                    Values.CopyTo(Sorted, 0);

                    Array.Sort(Sorted);
                    median = Sorted.Length % 2 == 0
                        ? (Sorted[Sorted.Length / 2] + Sorted[Sorted.Length / 2 - 1]) / 2.0
                        : Sorted[Sorted.Length / 2];
                    return median;
                }
            }


            #endregion

            #region Методы
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
            /// Возвращает нормлизованную случайную величину, полученную из данной.
            /// </summary>
            /// <returns></returns>
            public RandomValue GetNormalizedValue()
            {
                return new RandomValue(Values.Select(val => val / Interval));
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
                foreach (KeyValuePair<double, double> pair in Probs)
                {
                    double key = Math.Pow(pair.Key - currentexeption, k);
                    if (!buffer.ContainsKey(key))
                        buffer.Add(key, 0);
                    buffer[key] += pair.Value;
                }

                return new RandomValue(buffer).MathExeption;
            }

            /// <summary>
            ///     Возвращает центральный момент данной случайной величины заданного порядка.
            /// </summary>
            /// <param name="k"></param>
            /// <returns></returns>
            public double CentralMomentTEST(int k)
            {
                return new RandomValue(Values.Select(val => Math.Pow(val * (1 - MathExeption), k))).MathExeption;
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

            #endregion

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