using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;

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
            
            private double squaredsum = Double.NaN;
            /// <summary>
            ///     Сумма квадратов значений данной случайной величины. Для вычисления необходима частотная статистика
            ///     <see cref="Statistic" />.
            /// </summary>
            public double SquaredSum => Double.IsNaN(squaredsum)
                ? squaredsum = Values.Select(val => Math.Pow(val, 2)).Sum()
                : squaredsum;

            private double sum = Double.NaN;
            /// <summary>
            ///     Сумма всех значений данной случайной величины. Для вычисления необходима частотная статистика
            ///     <see cref="Statistic" />.
            /// </summary>
            public double Sum => Double.IsNaN(sum) ? sum = Statistic.Select(row => row.Key * row.Value).Sum() : sum;

            private double average = Double.NaN;
            /// <summary>
            ///     Среднее значение последовательности.
            /// </summary>
            public double Average => Double.IsNaN(average) ? average = Statistic.Select(row => row.Key * row.Value).Sum() / Count : average;

            private double max = Double.NaN;
            /// <summary>
            ///     Масимальный элемент последовательности.
            /// </summary>
            public double Max => Double.IsNaN(max) ? max = Values.Max() : max;

            private double maxabs = Double.NaN;
            /// <summary>
            ///     Максимальный по модулю элемент последовательности.
            /// </summary>
            public double MaxAbs => Double.IsNaN(maxabs) ? maxabs = Values[Values.Select(Math.Abs).ToList().IndexOf(Values.Select(Math.Abs).Max())] : maxabs;


            private double min = Double.NaN;
            /// <summary>
            ///     Минимальный элемент последовательности.
            /// </summary>
            public double Min => Double.IsNaN(min) ? min = Values.Min() : min;

            private double minabs = Double.NaN;
            /// <summary>
            ///     Минимальный по модулю элемент последовательности.
            /// </summary>
            public double MinAbs => Double.IsNaN(minabs) ? minabs = Values[Values.Select(Math.Abs).ToList().IndexOf(Values.Select(Math.Abs).Min())] : minabs;

            private double interval = Double.NaN;
            /// <summary>
            ///     Возвращает длину интервала. Вычисляется как <see cref="Max" /> - <see cref="Min" />.
            /// </summary>
            public double Interval => Double.IsNaN(interval) ? interval = Max - Min : interval;

            private double mean = Double.NaN;
            /// <summary>
            ///     Мода случайной величины. Если есть несколько одинаково-часто-встречающихся величин, вернется первая из них.
            /// </summary>
            public double Mean => Double.IsNaN(mean) ? mean = Statistic.First(val => val.Value == Statistic.Values.Max()).Key : mean;

            /// <summary>
            ///     Определяет, является ли величина мультимодальной.
            /// </summary>
            public bool IsMultiModal => Statistic.Count(val => val.Value == Statistic.Values.Max()) > 1;

            private double dispersion = Double.NaN;
            /// <summary>
            ///     Дисперсия данной величины.
            /// </summary>
            public double Dispersion
            {
                get
                {
                    if (!Double.IsNaN(dispersion))
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
            
            private double standarddeviation = Double.NaN;
            /// <summary>
            ///     Стандартное отклонение данной величины.
            /// </summary>
            public double StandardDeviation => Double.IsNaN(standarddeviation) ? standarddeviation = Math.Sqrt(Dispersion) : standarddeviation;

            private double standarderror = Double.NaN;
            /// <summary>
            /// Стандартная ошибка для выборки, когда n != N.
            /// </summary>
            public double StandardError => Double.IsNaN(standarderror) ? standarderror = Dispersion / Math.Sqrt(Count) : standarderror;

            private double standarderrorgeneral = Double.NaN;
            /// <summary>
            /// Стандартная ошибка для генеральной совокупности, когда n = N.
            /// </summary>
            public double StandardErrorGeneral => Double.IsNaN(standarderrorgeneral) ? standarderrorgeneral = StandardDeviation / Math.Sqrt(Count) : standarderrorgeneral;

            private double mathexeption = Double.NaN;
            /// <summary>
            ///     Математическое ожидание данной случайной величины. Требует наличие списка вероятностей для вычисления.
            /// </summary>
            public double MathExeption => Double.IsNaN(mathexeption) ? mathexeption = Probs.Select(row => row.Key * row.Value).Sum() : mathexeption;

            private double kurtosis = Double.NaN;
            /// <summary>
            ///     Коэффициент эксцесса данной случайной величины.
            /// </summary>
            public double Kurtosis
            {
                get
                {
                    if (!Double.IsNaN(kurtosis))
                        return kurtosis;
                    double n = Count;
                    double fdel = (n * (n + 1)) / ((n - 1) * (n - 2) * (n - 3));
                    double sum = Values.Select(val => Math.Pow((val - Average) / StandardDeviation, 4)).Sum();
                    double sdel = (3 * Math.Pow(n - 1, 2)) / ((n - 2) * (n - 3));
                    return kurtosis = fdel * sum - sdel;
                }
            }

            private double skewness = Double.NaN;
            /// <summary>
            /// Коэффициент ассимметрии данной случайной величины.
            /// </summary>
            public double Skewness
            {
                get
                {
                    if (!Double.IsNaN(skewness))
                        return skewness;
                    double n = Count;
                    double fdel = n / ((n - 1) * (n - 2));
                    double sum = Values.Select(val => Math.Pow((val - Average) / StandardDeviation, 3)).Sum();
                    return skewness = fdel * sum;
                }
            }

            private double median = Double.NaN;
            /// <summary>
            /// Медиана данной случайной величины.
            /// </summary>
            public double Median
            {
                get
                {
                    if (!Double.IsNaN(median))
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
                return String.Join(", ", Values);
            }
        }

        /// <summary>
        /// Возвращает результат F-критерия или, другими словами, критерия Фишера. 
        /// </summary>
        /// <param name="DeterminationCoefficient">Коэффициент детерминации.</param>
        /// <param name="TableLength">Число наблюдений.</param>
        /// <param name="ColumnsLength">Число параметров.</param>
        /// <returns></returns>
        public static double FTest(double DeterminationCoefficient, int TableLength, int ColumnsLength)
        {
            double Result = DeterminationCoefficient / (ColumnsLength - 1);
            Result *= (TableLength - ColumnsLength) / (1 - DeterminationCoefficient);
            return Result;
        }

        /// <summary>
        /// Возвращает значение коэффициента корреляции Пирсона для двух заданных величин.
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static double GetCorrelationValue(RandomValue First, RandomValue Second)
        {
            if (First.Count!=Second.Count)
                throw new InvalidOperationException("Число элементов в величинах должно совпадать.");
            int n = First.Count;
            double Numerator = n * First.Values.Select((elem, ind) => elem * Second.Values[ind]).Sum() - First.Sum*Second.Sum;
            double Denominator =
                Math.Abs(n * First.SquaredSum - Math.Pow(First.Sum, 2));
            Denominator *= Math.Abs(n * Second.SquaredSum - Math.Pow(Second.Sum, 2));
            Denominator = Math.Sqrt(Denominator);
            return Numerator / Denominator;
        }

        /// <summary>
        /// Возвращает таблицу корреляций Пирсона.
        /// </summary>
        /// <returns></returns>
        public static double[][] GetCorrelationTable(RandomValue[] Columns)
        {
            double[][] Result = new double[Columns.Length][].Select(row => new double[Columns.Length]).ToArray();
            for (int i = 0; i < Columns.Length; i++)
            {
                for (int j = 0; j < i; j++)
                    Result[i][j] = Result[j][i] = GetCorrelationValue(Columns[i], Columns[j]);
                Result[i][i] = 1;
            }
            return Result;
        }

        /// <summary>
        /// Возвращает значение функции Лапласа из таблицы.
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static double LaplasFunc(double Source)
        {
            int sign = (int)(Source / Math.Abs(Source));
            Source = Math.Abs(Source);
            double Result = 0;
            if (Source >= 5)
            {
                Result = 0.5;
            }
            else if (LaplasTable.ContainsKey(Source))
                Result = LaplasTable[Source];
            else
            {
                double[] Keys = LaplasTable.Keys.OrderBy(val => val).ToArray();
                //необходимо добежать до первого ключа, который больше чем текущий x
                //а потом по пропорциям выдать соответствующее значение функции лапласа
                double Higher = Keys.First(val => val > Source);
                double Lower = Keys[Keys.ToList().IndexOf(Higher)-1];
                Result = LaplasTable[Lower] +
                         (LaplasTable[Higher] - LaplasTable[Lower]) * ((Source - Lower) / (Higher - Lower))
                    ;
            }

            return Result * sign;
        }

        /// <summary>
        /// Возвращает значение критерия хи-квадрат для данной величины с заданным числом интервалов.
        /// </summary>
        /// <param name="Source">Случайная величина, для которой необходимо рассчитать критерий</param>
        /// <param name="CountOfIntervals">Число интервалов разбиения.</param>
        /// <returns></returns>
        public static double ChiSquaredTest(RandomValue Source, int CountOfIntervals=0,bool PrintM=false)
        {
            if (CountOfIntervals == 0)
            {
                if (Source.Count < 15)
                    CountOfIntervals = 1;
                else if (Source.Count < 225)
                    CountOfIntervals = 3;
                else if (Source.Count < 1250)
                    CountOfIntervals = (int)Math.Ceiling(Source.Count / (double)50);
                else
                    CountOfIntervals = 25;
            }

            //Начало - откуда начнем рассматривать интервалы
            double Start = Source.Min;

            //Конец - окончание последнего интервала
            double End = Source.Max;

            //Шаг - зависит от числа поданных на вход интервалов
            double Step = (End - Start) / CountOfIntervals;

            //Стандартное отклонение величины
            double StandardDeviation = Source.StandardDeviation;

            //Среднее значение выборки
            double Average = Source.Average;

            //Массив теоретических частот
            double[] MTheoral = new double[CountOfIntervals];

            //Массив практических частот
            int[] MPract = new int[CountOfIntervals];

            //Массив хи квадратов для каждого интервала
            double[] HiQua = new double[CountOfIntervals];

            int i = 0;
            for (double j = Start; i<CountOfIntervals; j += Step, i++)
            {
                //Начало текущего интервала
                double CurrentStart = j;

                //Конец текущего интервала
                double CurrentEnd = j + Step;

                //Вычисляем теоретическую частоту
                MTheoral[i] = Source.Count*Math.Abs(LaplasFunc(Math.Abs(CurrentEnd - Average) / StandardDeviation) -
                                        LaplasFunc(Math.Abs(CurrentStart - Average) / StandardDeviation));

                //Получаем число величин, попавших в интервал. Это практическая частота.
                MPract[i] = Source.Values.Count(val => val >= CurrentStart && val < CurrentEnd);
                //Вычисляем критерий хи квадрат для данного интервала
                if (MPract[i] == (int)Math.Round(MTheoral[i]))
                    HiQua[i] = 0;
                if (Math.Abs(MTheoral[i]) < 0.5)
                    HiQua[i] = Math.Pow(MPract[i], 2);
                else
                    HiQua[i] = Math.Pow(MPract[i] - (int)Math.Round(MTheoral[i]), 2) / MTheoral[i];
            }

            if (PrintM)
            {
                Console.WriteLine($"MPract = {string.Join(", ", MPract)}" );
                Console.WriteLine($"MTheoral = {string.Join(", ", MTheoral.Select(Convert.ToInt32))}");
                Console.WriteLine();
            }

            //Результат сумма критериев со всех интервалов
            double HiQuaResult = HiQua.Sum();
            return HiQuaResult;
        }

        /// <summary>
        /// Определяет, верна ли гипотеза о нормальности данного распределения. Число интервалов определяется автоматически.
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static bool IsNormal(RandomValue Source,bool PrintM=false)
        {
            int CountOfIntervals;
            
            if (Source.Count < 225)
                CountOfIntervals = 4;
            else if (Source.Count < 1250)
                CountOfIntervals = (int)Math.Ceiling(Source.Count / (double)50);
            else
                CountOfIntervals = 25;
            double SourceChiSquared = ChiSquaredTest(Source, CountOfIntervals,PrintM);
            int FreeDegree = CountOfIntervals - 3;
            double CriticalChiSquared = GetChiSquaredCritical(FreeDegree);
            return CriticalChiSquared >= SourceChiSquared;
        }

        /// <summary>
        /// Возвращает значение множественного коэффициента корреляции. (MultipleCorrelationValue)
        /// </summary>
        /// <param name="CorrelationTable"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static double GetMCV(Matrix CorrelationTable, int i)
        {
            return Math.Sqrt(1 - CorrelationTable.Definite / Matrix.GetMinor(CorrelationTable, i, i));
        }

        /// <summary>
        /// Возвращает значение частного коэффициента корреляции между переменными с индексами i и j. (PartialCorrelationCoefficientValue)
        /// </summary>
        /// <param name="CorrelationTable"></param>
        /// <returns></returns>
        public static double GetPCCV(Matrix CorrelationTable, int i, int j)
        {
            return Matrix.GetAlgebraicAddition(CorrelationTable, i, j) / Math.Sqrt(
                       Matrix.GetAlgebraicAddition(CorrelationTable, i, i) *
                       Matrix.GetAlgebraicAddition(CorrelationTable, j, j));
        }

        /// <summary>
        /// Возвращает таблицу частных коэффициентов корреляции. (PartialCorrelationCoefficientTable)
        /// </summary>
        /// <param name="CorrelationTable"></param>
        /// <returns></returns>
        public static double[][] GetPCCT(Matrix CorrelationTable)
        {
            double[][] Result = new double[CorrelationTable.LengthY][].Select(row => new double[CorrelationTable.LengthY]).ToArray();
            for (int i = 0; i < CorrelationTable.LengthY; i++)
            {
                for (int j = 0; j < i; j++)
                    Result[i][j] = Result[j][i] = GetPCCV(CorrelationTable,i,j);
                Result[i][i] = 1;
            }
            return Result;
        }

        /// <summary>
        /// Возвращает значение t значимости коэффициента корреляции Стюдента для заданного выборочного частного коэффициента корреляции и числа параметров.
        /// </summary>
        /// <param name="PCCV"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double GetCSC(double PCCV, int n)
        {
            return Math.Abs(PCCV) * Math.Sqrt((n - 2) / (1 - Math.Pow(PCCV, 2)));
        }

        /// <summary>
        /// Возвращает критическое значение для значимости коэффициента корреляции Стюдента при a = 0.95 и заданного числа степеней свободы.
        /// </summary>
        /// <param name="FreedomDegree"></param>
        /// <returns></returns>
        public static double GetCSCCritical(int FreedomDegree) => CSCCritical[FreedomDegree];

        /// <summary>
        /// Возвращает критической значение критерия хи-квадрат для a = 0.05 с заданным числом степеней свободы.
        /// </summary>
        /// <param name="CountFreeDegree"></param>
        /// <returns></returns>
        public static double GetChiSquaredCritical(int CountFreeDegree) => ChiSquaredCriticals[CountFreeDegree - 1];
        private static readonly double[] ChiSquaredCriticals =
        {
            3.84146, 5.99146, 7.81473, 9.48773, 11.0705, 12.59159, 14.06714, 15.50731, 16.91898, 18.30704, 19.67514,
            21.02607, 22.36203, 23.68479, 24.99579, 26.29623, 27.58711, 28.8693, 30.14353, 31.41043, 32.67057, 33.92444,
            35.17246, 36.41503, 37.65248, 38.88514, 40.11327, 41.33714, 42.55697, 43.77297
        };
        /// <summary>
        /// Критические значения для значимости коэффициента корреляции Стюдента при a = 0.95.
        /// </summary>
        private static readonly Dictionary<int, double> CSCCritical = new Dictionary<int, double>()
        {
            {1, 12.706},
            {2, 4.302},
            {3, 3.182},
            {4, 2.776},
            {5, 2.57},
            {6, 2.446},
            {7, 2.3646},
            {8, 2.306},
            {9, 2.2622},
            {10, 2.2281},
            {11, 2.201},
            {12, 2.1788},
            {13, 2.1604},
            {14, 2.1448},
            {15, 2.1314},
            {16, 2.119},
            {17, 2.1098},
            {18, 2.1009},
            {19, 2.093},
            {20, 2.086},
            {21, 2.0790},
            {22, 2.0739},
            {23, 2.0687},
            {24, 2.0639},
            {25, 2.0595},
            {26, 2.059},
            {27, 2.0518},
            {28, 2.0484},
            {29, 2.0452},
            {30, 2.0423}
        };
        private static readonly Dictionary<double, double> LaplasTable = new Dictionary<double, double>
        {
            {0, 0},
            {0.01, 0.004},
            {0.02, 0.008},
            {0.03, 0.012},
            {0.04, 0.016},
            {0.05, 0.0199},
            {0.06, 0.0239},
            {0.07, 0.0279},
            {0.08, 0.0319},
            {0.09, 0.0359},
            {0.1, 0.0398},
            {0.11, 0.0438},
            {0.12, 0.0478},
            {0.13, 0.0517},
            {0.14, 0.0557},
            {0.15, 0.0596},
            {0.16, 0.0636},
            {0.17, 0.0675},
            {0.18, 0.0714},
            {0.19, 0.0753},
            {0.2, 0.0793},
            {0.21, 0.0832},
            {0.22, 0.0871},
            {0.23, 0.091},
            {0.24, 0.0948},
            {0.25, 0.0987},
            {0.26, 0.1026},
            {0.27, 0.1064},
            {0.28, 0.1103},
            {0.29, 0.1141},
            {0.3, 0.1179},
            {0.31, 0.1217},
            {0.32, 0.1255},
            {0.33, 0.1293},
            {0.34, 0.1331},
            {0.35, 0.1368},
            {0.36, 0.1406},
            {0.37, 0.1443},
            {0.38, 0.148},
            {0.39, 0.1517},
            {0.4, 0.1554},
            {0.41, 0.1591},
            {0.42, 0.1628},
            {0.43, 0.1664},
            {0.44, 0.17},
            {0.45, 0.1736},
            {0.46, 0.1772},
            {0.47, 0.1808},
            {0.48, 0.1844},
            {0.49, 0.1879},
            {0.5, 0.1915},
            {0.51, 0.195},
            {0.52, 0.1985},
            {0.53, 0.2019},
            {0.54, 0.2054},
            {0.55, 0.2088},
            {0.56, 0.2123},
            {0.57, 0.2157},
            {0.58, 0.219},
            {0.59, 0.2224},
            {0.6, 0.2257},
            {0.61, 0.2291},
            {0.62, 0.2324},
            {0.63, 0.2357},
            {0.64, 0.2389},
            {0.65, 0.2422},
            {0.66, 0.2454},
            {0.67, 0.2486},
            {0.68, 0.2517},
            {0.69, 0.2549},
            {0.7, 0.258},
            {0.71, 0.2611},
            {0.72, 0.2642},
            {0.73, 0.2673},
            {0.74, 0.2703},
            {0.75, 0.2734},
            {0.76, 0.2764},
            {0.77, 0.2794},
            {0.78, 0.2823},
            {0.79, 0.2852},
            {0.8, 0.2881},
            {0.81, 0.291},
            {0.82, 0.2939},
            {0.83, 0.2967},
            {0.84, 0.2995},
            {0.85, 0.3023},
            {0.86, 0.3051},
            {0.87, 0.3078},
            {0.88, 0.3106},
            {0.89, 0.3133},
            {0.9, 0.3159},
            {0.91, 0.3186},
            {0.92, 0.3212},
            {0.93, 0.3238},
            {0.94, 0.3264},
            {0.95, 0.3289},
            {0.96, 0.3315},
            {0.97, 0.334},
            {0.98, 0.3365},
            {0.99, 0.3389},
            {1, 0.3413},
            {1.01, 0.3438},
            {1.02, 0.3461},
            {1.03, 0.3485},
            {1.04, 0.3508},
            {1.05, 0.3531},
            {1.06, 0.3554},
            {1.07, 0.3577},
            {1.08, 0.3599},
            {1.09, 0.3621},
            {1.1, 0.3643},
            {1.11, 0.3665},
            {1.12, 0.3686},
            {1.13, 0.3708},
            {1.14, 0.3729},
            {1.15, 0.3749},
            {1.16, 0.377},
            {1.17, 0.379},
            {1.18, 0.381},
            {1.19, 0.383},
            {1.2, 0.3849},
            {1.21, 0.3869},
            {1.22, 0.3883},
            {1.23, 0.3907},
            {1.24, 0.3925},
            {1.25, 0.3944},
            {1.26, 0.3962},
            {1.27, 0.398},
            {1.28, 0.3997},
            {1.29, 0.4015},
            {1.3, 0.4032},
            {1.31, 0.4049},
            {1.32, 0.4066},
            {1.33, 0.4082},
            {1.34, 0.4099},
            {1.35, 0.4115},
            {1.36, 0.4131},
            {1.37, 0.4147},
            {1.38, 0.4162},
            {1.39, 0.4177},
            {1.4, 0.4192},
            {1.41, 0.4207},
            {1.42, 0.4222},
            {1.43, 0.4236},
            {1.44, 0.4251},
            {1.45, 0.4265},
            {1.46, 0.4279},
            {1.47, 0.4292},
            {1.48, 0.4306},
            {1.49, 0.4319},
            {1.5, 0.4332},
            {1.51, 0.4345},
            {1.52, 0.4357},
            {1.53, 0.437},
            {1.54, 0.4382},
            {1.55, 0.4394},
            {1.56, 0.4406},
            {1.57, 0.4418},
            {1.58, 0.4429},
            {1.59, 0.4441},
            {1.6, 0.4452},
            {1.61, 0.4463},
            {1.62, 0.4474},
            {1.63, 0.4484},
            {1.64, 0.4495},
            {1.65, 0.4505},
            {1.66, 0.4515},
            {1.67, 0.4525},
            {1.68, 0.4535},
            {1.69, 0.4545},
            {1.7, 0.4554},
            {1.71, 0.4564},
            {1.72, 0.4573},
            {1.73, 0.4582},
            {1.74, 0.4591},
            {1.75, 0.4599},
            {1.76, 0.4608},
            {1.77, 0.4616},
            {1.78, 0.4625},
            {1.79, 0.4633},
            {1.8, 0.4641},
            {1.81, 0.4649},
            {1.82, 0.4656},
            {1.83, 0.4664},
            {1.84, 0.4671},
            {1.85, 0.4678},
            {1.86, 0.4686},
            {1.87, 0.4693},
            {1.88, 0.4699},
            {1.89, 0.4706},
            {1.9, 0.4713},
            {1.91, 0.4719},
            {1.92, 0.4726},
            {1.93, 0.4732},
            {1.94, 0.4738},
            {1.95, 0.4744},
            {1.96, 0.475},
            {1.97, 0.4756},
            {1.98, 0.4761},
            {1.99, 0.4767},
            {2, 0.4772},
            {2.02, 0.4783},
            {2.04, 0.4793},
            {2.06, 0.4803},
            {2.08, 0.4812},
            {2.1, 0.4821},
            {2.12, 0.483},
            {2.14, 0.4838},
            {2.16, 0.4846},
            {2.18, 0.4854},
            {2.2, 0.4861},
            {2.22, 0.4868},
            {2.24, 0.4875},
            {2.26, 0.4881},
            {2.28, 0.4887},
            {2.3, 0.4893},
            {2.32, 0.4898},
            {2.34, 0.4904},
            {2.36, 0.4909},
            {2.38, 0.4913},
            {2.4, 0.4918},
            {2.42, 0.4922},
            {2.44, 0.4927},
            {2.46, 0.4931},
            {2.48, 0.4934},
            {2.5, 0.4938},
            {2.52, 0.4941},
            {2.54, 0.4945},
            {2.56, 0.4948},
            {2.58, 0.4951},
            {2.6, 0.4953},
            {2.62, 0.4956},
            {2.64, 0.4959},
            {2.66, 0.4961},
            {2.68, 0.4963},
            {2.7, 0.4965},
            {2.72, 0.4967},
            {2.74, 0.4969},
            {2.76, 0.4971},
            {2.78, 0.4973},
            {2.8, 0.4974},
            {2.82, 0.4976},
            {2.84, 0.4977},
            {2.86, 0.4979},
            {2.88, 0.498},
            {2.9, 0.4981},
            {2.92, 0.4982},
            {2.94, 0.4984},
            {2.96, 0.4985},
            {2.98, 0.4986},
            {3, 0.49865},
            {3.2, 0.49931},
            {3.4, 0.49966},
            {3.6, 0.49984},
            {3.8, 0.49992},
            {4, 0.49996},
            {4.5, 0.49999},
            {5, 0.49999}
        };
    }
}