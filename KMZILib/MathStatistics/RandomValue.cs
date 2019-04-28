using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using Data = KMZILib.Misc.Data;

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
                $"Количество элементов : {Count}";

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
        /// Возвращает результат F-критерия или, другими словами, критерия Фишера для множественных коэффициентов корреляции. 
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
        /// Возвращает критическое значение F критерия для данных чисел степеней свободы.
        /// </summary>
        /// <param name="FreedomDegreeFirst"></param>
        /// <param name="FreedomDegreeSecond"></param>
        /// <returns></returns>
        public static double GetFCritical(int FreedomDegreeFirst, int FreedomDegreeSecond)
        {
            int RowIndex = Data.FCCritical.Keys.OrderBy(rowindex => Math.Abs(rowindex - FreedomDegreeFirst)).First();
            int ColumnIndex = Data.FCCritical.First().Value.Keys
                .OrderBy(columnindex => Math.Abs(columnindex - FreedomDegreeSecond)).First();
            return Data.FCCritical[RowIndex][ColumnIndex];
        }

        /// <summary>
        /// Возвращает значимость по F критерию.
        /// </summary>
        /// <param name="FTestValue"></param>
        /// <param name="FreedomDegreeFirst"></param>
        /// <param name="FreedomDegreeSecond"></param>
        /// <returns></returns>
        public static bool IsSignificantByFTest(double FTestValue, int FreedomDegreeFirst, int FreedomDegreeSecond)
        {
            double Critical = GetFCritical(FreedomDegreeFirst, FreedomDegreeSecond);
            return FTestValue >= Critical;
        }

        /// <summary>
        /// Возвращает значение коэффициента корреляции Пирсона для двух заданных величин.
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static double GetCorrelationValue(RandomValue First, RandomValue Second)
        {
            if (First.Count != Second.Count)
                throw new InvalidOperationException("Число элементов в величинах должно совпадать.");
            int n = First.Count;
            double Numerator = n * First.Values.Select((elem, ind) => elem * Second.Values[ind]).Sum() - First.Sum * Second.Sum;
            double Denominator =
                Math.Abs(n * First.SquaredSum - Math.Pow(First.Sum, 2));
            Denominator *= Math.Abs(n * Second.SquaredSum - Math.Pow(Second.Sum, 2));
            Denominator = Math.Sqrt(Denominator);
            return Numerator / Denominator;
        }

        /// <summary>
        /// Возвращает таблицу парных корреляций Пирсона.
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
        ///     Возвращает значение функции Лапласа из таблицы.
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static double LaplasFunc(double Source)
        {
            int sign = (int)(Source / Math.Abs(Source));
            Source = Math.Abs(Source);
            double Result = 0;
            if (Source >= 5)
                Result = 0.5;
            else if (Data.LaplasTable.ContainsKey(Source))
                Result = Data.LaplasTable[Source];
            else
            {
                double[] Keys = Data.LaplasTable.Keys.OrderBy(val => val).ToArray();
                //необходимо добежать до первого ключа, который больше чем текущий x
                //а потом по пропорциям выдать соответствующее значение функции лапласа
                double Higher = Keys.First(val => val > Source);
                double Lower = Keys[Keys.ToList().IndexOf(Higher) - 1];
                Result = Data.LaplasTable[Lower] +
                         (Data.LaplasTable[Higher] - Data.LaplasTable[Lower]) * ((Source - Lower) / (Higher - Lower))
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
        public static double ChiSquaredTest(RandomValue Source, int CountOfIntervals = 0, bool PrintM = false)
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
            for (double j = Start; i < CountOfIntervals; j += Step, i++)
            {
                //Начало текущего интервала
                double CurrentStart = j;

                //Конец текущего интервала
                double CurrentEnd = j + Step;

                //Вычисляем теоретическую частоту
                MTheoral[i] = Source.Count * Math.Abs(LaplasFunc(Math.Abs(CurrentEnd - Average) / StandardDeviation) -
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
                Console.WriteLine($"MPract = {string.Join(", ", MPract)}");
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
        public static bool IsNormal(RandomValue Source, bool PrintM = false)
        {
            int CountOfIntervals;

            if (Source.Count < 225)
                CountOfIntervals = 4;
            else if (Source.Count < 1250)
                CountOfIntervals = (int)Math.Ceiling(Source.Count / (double)50);
            else
                CountOfIntervals = 25;
            double SourceChiSquared = ChiSquaredTest(Source, CountOfIntervals, PrintM);
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
                    Result[i][j] = Result[j][i] = GetPCCV(CorrelationTable, i, j);
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
        public static double GetCSCCritical(int FreedomDegree)
        {
            if (FreedomDegree > Misc.Data.CSCCritical.Keys.Max())
                return 2;
            return Misc.Data.CSCCritical[FreedomDegree];
        }

        /// <summary>
        /// Возвращает регрессионное уравнения для заданных столбцов-параметров и результирующего столбца матрицы наблюдений.
        /// </summary>
        /// <param name="Parametres"></param>
        /// <param name="ResultValue"></param>
        /// <returns></returns>
        public static double[] GetRegressionEquation(RandomValue[] Parametres, RandomValue ResultValue)
        {
            double[][] XArray = new double[Parametres.First().Count][];
            for (int i = 0; i < XArray.Length; i++)
            {
                XArray[i] = new double[Parametres.Length + 1];
                XArray[i][0] = 1;
                Parametres.Select(param => param.Values[i]).ToArray().CopyTo(XArray[i], 1);
            }
            Matrix X = new Matrix(XArray);
            Matrix Y = new Matrix(new[] { ResultValue.Values }).Transpose();
            return ((X.TransposedCopy() * X).Reverse * X.TransposedCopy() * Y).TransposedCopy().Values[0];
        }

        /// <summary>
        /// Возвращает эмпирическое значение F-критерия оценка значимости регрессионного уравнения.
        /// </summary>
        /// <param name="Parametres"></param>
        /// <param name="ResultValue"></param>
        /// <param name="RegressionEquation"></param>
        /// <returns></returns>
        public static double GetFCriteriaEquation(RandomValue[] Parametres, RandomValue ResultValue,
            double[] RegressionEquation)
        {
            double[][] XArray = new double[Parametres.First().Count][];
            for (int i = 0; i < XArray.Length; i++)
            {
                XArray[i] = new double[Parametres.Length + 1];
                XArray[i][0] = 1;
                Parametres.Select(param => param.Values[i]).ToArray().CopyTo(XArray[i], 1);
            }
            Matrix X = new Matrix(XArray);
            double[] NewY = (X * new Matrix(new[] {RegressionEquation}).Transpose()).Values.Select(row => row.First())
                .ToArray();
            int n = Parametres.Length + 1;
            int k = 1;
            return (new Matrix(new[] {NewY}).Transpose() * new Matrix(new[] {NewY})).Values.First().First() / (k + 1) *
                   (n - k - 1) / (ResultValue.Values.Select((val, ind) => Math.Pow(val - NewY[ind], 2)).Sum());
        }


        /// <summary>
        /// Возвращает критической значение критерия хи-квадрат для a = 0.05 с заданным числом степеней свободы.
        /// </summary>
        /// <param name="CountFreeDegree"></param>
        /// <returns></returns>
        public static double GetChiSquaredCritical(int CountFreeDegree) => Data.ChiSquaredCriticals[CountFreeDegree - 1];
    };
}