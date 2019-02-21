using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KMZILib
{
    /// <summary>
    /// Статический класс для работы с линейными уравнениями.
    /// </summary>
    public static class LinearEquations
    {
        /// <summary>
        /// Представляет линейное уравнение.
        /// </summary>
        public class LinearEquation
        {
            /// <summary>
            /// Коэффициенты линейного уравнения. Последний из них - свободный член.
            /// </summary>
            public double[] Coefficients;

            /// <summary>
            /// Инициализирует новое линейное уравнение по имеющему набору вещественных коэффициентов.
            /// </summary>
            /// <param name="Source"></param>
            public LinearEquation(double[] Source)
            {
                Coefficients = new double[Source.Length];
                Source.CopyTo(Coefficients, 0);
            }

            /// <summary>
            /// Инициализирует новое линейное уравнение по имеющему набору целочисленных коэффициентов.
            /// </summary>
            /// <param name="Source"></param>
            public LinearEquation(int[] Source) : this(Source.Select(item => (double)item).ToArray())
            {
            }

            /// <summary>
            /// Инициализирует новое линейное уравнение по его строковому предствлению.
            /// </summary>
            /// <param name="Source"></param>
            public LinearEquation(string Source)
            {
                //Создаем список коэффициентов, который будем заполнять
                List<double> CoefficientsBuffer = new List<double>();
                //Разделяем уравнения на левую и правую части
                Regex EquationRegex = new Regex(@"(?<LeftPart>.+)=\s*(?<RightPartSign>-)?\s*(?<RightPart>\d+)");
                Match Equation = EquationRegex.Match(Source);
                //Находим значение свободного члена
                CoefficientsBuffer.Add(Equation.Groups["RightPartSign"].Value == ""
                    ? double.Parse(Equation.Groups["RightPart"].Value)
                    : -1 * double.Parse(Equation.Groups["RightPart"].Value));
                //Теперь делаем разбор левой части
                Regex CoeffsRegex =
                    new Regex(
                        @"\s*(?<sign>[+-])?\s*(?<value>\s*[1-9][0-9]*)?\s*\w+");
                foreach (Match polynommatch in CoeffsRegex.Matches(Equation.Groups["LeftPart"].Value))
                {
                    //Определяем знак
                    int sign = polynommatch.Groups["sign"].Value.Contains("-") ? -1 : 1;
                    //Определяем абсолютное значение
                    double value = polynommatch.Groups["value"].Value != ""
                        ? double.Parse(polynommatch.Groups["value"].Value)
                        : 1;
                    //Восстанавливаем знак
                    value *= sign;
                    //заносим в список
                    CoefficientsBuffer.Insert(CoefficientsBuffer.Count - 1, value);
                }

                //Теперь можно заполнить массив коэффициентов
                Coefficients = CoefficientsBuffer.ToArray();
            }

            /// <summary>
            /// Возвращает строковое представление линейного сравнения.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                //Заводим массив символов, который затем будем заполнять. Сразу заносим в него первый коэффициент.
                StringBuilder Result = new StringBuilder($"{Coefficients.First()}x1");
                //Дополняем его всеми остальными коэффициентами, кроме последнего, который свободный член.
                for (int i = 1; i < Coefficients.Length - 1; i++)
                    Result.Append($" {(Coefficients[i] < 0 ? "-" : "+")} {Math.Abs(Coefficients[i])}x{i + 1}");
                //Добавляем свободный член
                Result.Append($" = {Coefficients.Last()}");
                //готово
                return Result.ToString();
            }
        }

        /// <summary>
        /// Метод Гаусса решения систем линейных уравнений
        /// </summary>
        public static class GaussMethod
        {

            /// <summary>
            /// Модификации метода Гаусса.
            /// </summary>
            public enum GEModification
            {
                /// <summary>
                /// Стандартный алгоритм.
                /// </summary>
                Standard,
                /// <summary>
                /// Выбор ведущего элемента в строке.
                /// </summary>
                LeadingOnTheLine,
                /// <summary>
                /// Выбор ведущего элемента в столбце.
                /// </summary>
                LeadingOnTheColumn,
                /// <summary>
                /// Выбор ведущего элемента в матрице.
                /// </summary>
                LeadingOnWholeMatrix
            }

            /// <summary>
            /// Осуществляет решение системы линейных уравнений заданных в виде матрицы методом Гаусса.
            /// </summary>
            /// <param name="MatrixArray"></param>
            /// <param name="style"></param>
            /// <param name="Debug"></param>
            /// <returns></returns>
            public static Vector Solve(double[][] MatrixArray, GEModification style = GEModification.Standard, bool Debug = false)
            {
                Matrix MatrixCopy = Straight(MatrixArray, out Dictionary<int, int> offset, style, Debug);
                return Reverse(MatrixCopy, offset);

            }

            /// <summary>
            /// Обратный ход алгоритма Гаусса решения систем линейных уравнений.
            /// </summary>
            /// <param name="MatrixCopy"></param>
            /// <param name="offset"></param>
            /// <returns></returns>
            public static Vector Reverse(Matrix MatrixCopy, Dictionary<int, int> offset)
            {
                //Обратный ход
                double[] Result = new double[MatrixCopy.Values.Length];
                Result[Result.Length - 1] = MatrixCopy.Values[MatrixCopy.Values.Length - 1].Last();
                for (int i = Result.Length - 2; i >= 0; i--)
                {
                    Result[i] = MatrixCopy.Values[i].Last();
                    for (int j = i + 1; j < MatrixCopy.LengthY; j++)
                        Result[i] -= MatrixCopy[i][j] * Result[j];
                }

                return new Vector(offset.OrderBy(row => row.Value).Select(row => Result[row.Key]).ToArray());
            }

            /// <summary>
            /// Прямой ход алгоритма Гаусса решения систем линейных уравнений.
            /// </summary>
            /// <param name="MatrixArray"></param>
            /// <param name="offset"></param>
            /// <param name="style"></param>
            /// <param name="Debug"></param>
            /// <returns></returns>
            public static Matrix Straight(double[][] MatrixArray,out Dictionary<int, int> offset, GEModification style = GEModification.Standard,
                bool Debug = false)
            {
                Matrix MatrixCopy = new Matrix(MatrixArray) { HasFreeCoefficient = true };
                //Прямой ход
                offset = new Dictionary<int, int>();
                for (int i = 0; i < MatrixArray.Length; i++)
                    offset.Add(i, i);
                for (int i = 0; i < MatrixCopy.LengthY; i++)
                {
                    //TODO: После показа лабораторной можно убрать
                    if (Debug)
                    {
                        Console.WriteLine(MatrixCopy);
                        Console.WriteLine("---------------------------------------------------------\n");
                    }

                    int MaxIndex;
                    switch (style)
                    {
                        case GEModification.LeadingOnTheLine:
                            //получили индекс максимального по модулю элемента в текущей строке
                            MaxIndex = MatrixCopy.GetMaxAbsInRowIndex(i);
                            if (i < MaxIndex)
                            {
                                MatrixCopy.SwapColumns(i, MaxIndex);
                                SwapDict(offset, i, MaxIndex);
                            }

                            break;
                        case GEModification.LeadingOnTheColumn:
                            MaxIndex = MatrixCopy.GetMaxAbsInColumnIndex(i);
                            if (i < MaxIndex)
                                MatrixCopy.SwapLines(i, MaxIndex);

                            break;
                        case GEModification.LeadingOnWholeMatrix:
                            int MaxIndexI, MaxIndexJ;
                            int[] max = MatrixCopy.GetMaxAbsIndex();
                            MaxIndexI = max[0];
                            MaxIndexJ = max[1];
                            if (i <= MaxIndexI && i <= MaxIndexJ)
                            {
                                MatrixCopy.SwapLines(i, MaxIndexI);
                                MatrixCopy.SwapColumns(i, MaxIndexJ);
                                SwapDict(offset, i, MaxIndexJ);
                            }
                            break;
                        default:
                            break;

                    }
                    MatrixCopy.Values[i] = MatrixCopy[i].Select(val => val / MatrixCopy.Values[i][i]).ToArray();
                    //Сделали первый ненулевой элемент единицей
                    if (Debug)
                    {
                        Console.WriteLine(MatrixCopy);
                        Console.WriteLine("---------------------------------------------------------\n");
                    }

                    for (int j = i + 1; j < MatrixCopy.LengthY; j++)
                    {
                        double Multiplier = -(MatrixCopy[j][i] / MatrixCopy.Values[i][i]);
                        MatrixCopy[j] = MatrixCopy[j].Select((val, index) => val + MatrixCopy.Values[i][index] * Multiplier).ToArray();
                    }
                    //У всех остальных строчек обнулили i-ый столбец
                }

                return MatrixCopy;
            }
            private static void SwapDict(Dictionary<int, int> Source, int i, int j)
            {
                int buffer = Source[i];
                Source[i] = Source[j];
                Source[j] = buffer;
            }
        }

        /// <summary>
        /// LU-метод решения систем линейных уравнений.
        /// </summary>
        public static class LUMethod
        {
            /// <summary>
            /// Возвращает определитель матрицы, вычисленный с использованием LU-метода.
            /// </summary>
            /// <param name="Source"></param>
            /// <returns></returns>
            public static double GetDefinite(Matrix Source)
            {
                return GetLU(Source.Values)[1].Definite;
            }

            /// <summary>
            /// LU метод решения СЛУ.
            /// </summary>
            /// <param name="MatrixArray"></param>
            /// <param name="Debug"></param>
            /// <returns></returns>
            public static Vector Solve(double[][] MatrixArray, bool Debug = false)
            {
                Matrix[] buffer = GetLU(MatrixArray, Debug);
                Matrix L = buffer[0];
                Matrix U = buffer[1];

                Console.WriteLine($"L:\n{L}\n\nU:\n{U}");

                Vector Y = GaussMethod.Solve(L.Values.Select((row, index) => row.Concat(new[] { MatrixArray[index].Last() }).ToArray()).ToArray());
                Console.WriteLine($"\nY:\n{Y}");

                Vector X = GaussMethod.Solve(U.Values.Select((row, index) => row.Concat(new[] { Y[index] }).ToArray()).ToArray());

                return X;
            }

            /// <summary>
            /// Осуществляет разложение заданной матрицы на произвдение матриц L*U.
            /// </summary>
            /// <param name="MatrixArray"></param>
            /// <param name="Debug"></param>
            /// <returns></returns>
            public static Matrix[] GetLU(double[][] MatrixArray, bool Debug = false)
            {
                int n = MatrixArray.Length;
                Matrix A = new Matrix(MatrixArray) { HasFreeCoefficient = true }.WithoutFreeCoefficients;
                Matrix L = new Matrix(new double[n][].Select(row => new double[n]).ToArray());
                Matrix U = new Matrix(new double[n][].Select(row => new double[n]).ToArray());

                for (int i = 0; i < n; i++)
                {
                    //Переносим то, что можно просто скопировать
                    //и что будет служить в качестве необходимых
                    //начальных данных для дальнейших вычислений

                    //Первая строка матрицы U будет совпадать с соответствующей у А
                    U[0][i] = A[0][i];

                    //Диагональ у матрицы L состоит из единичек
                    L[i][i] = 1;

                    //Первый столбец матрицы A имеет вид A[i][0] = L[i][0] * U[0][0]
                    //Очевидно, можно сразу найти элементы L[i][0], зная U[0][0].
                    //А мы его уже знаем, это A[0][0]
                    L[i][0] = A[i][0] / A[0][0];
                }
                //Теперь можно вычислить остальные коэффициенты

                for (int dimension = 1; dimension < n; dimension++)
                {
                    //dimension - индекс "уголка" матрицы A

                    //Пробегаем каждый столбец в уголке,
                    //таким образом задавая всю строчку
                    //от U[dimension][dimension] до U[dimension][n-1]
                    for (int j = dimension; j < n; j++)
                    {
                        U[dimension][j] = A[dimension][j];
                        for (int i = 0; i < dimension; i++)
                            U[dimension][j] -= L[dimension][i] * U[i][j];

                    }

                    //Теперь пробегаем каждую строку,
                    //таким образом задавая весь столбец
                    //от U[dimension+1][dimension] до U[n-1][dimension]
                    for (int j = dimension + 1; j < n; j++)
                    {
                        L[j][dimension] = A[j][dimension];
                        for (int i = 0; i < dimension; i++)
                            L[j][dimension] -= L[j][i] * U[i][dimension];
                        L[j][dimension] /= U[dimension][dimension];
                    }
                }
                return new []{L,U};
            }

        }
    }
}
