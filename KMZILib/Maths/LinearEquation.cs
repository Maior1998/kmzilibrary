using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Осуществляет решение системы линейных уравнений заданных в виде матрицы методом Гаусса.
        /// </summary>
        /// <param name="MatrixArray"></param>
        /// <returns></returns>
        public static double[] GaussianElimination(double[][] MatrixArray, GEModification style=GEModification.Standart)
        {

            Matrix MatrixCopy = new Matrix(MatrixArray){HasFreeCoefficient = true};
            //Прямой ход
            Dictionary<int, int> offset = new Dictionary<int, int>();
            for (int i = 0; i < MatrixArray.Length; i++)
                offset.Add(i, i);
            for (int i = 0; i < MatrixCopy.LengthY; i++)
            {
                //TODO: После показа лабораторной можно убрать
                Console.WriteLine(string.Join("\n", MatrixCopy.Values.Select(str => string.Join("\t", str))));
                Console.WriteLine("---------------------------------------------------------\n");
                MatrixCopy.Values[i] = MatrixCopy[i].Select(val => val / MatrixCopy.Values[i][i]).ToArray();
                //Сделали первый ненулевой элемент единицей
                int MaxIndex;
                switch (style)
                {
                    case GEModification.LeadingOnTheLine:
                        //получили индекс максимального по модулю элемента в текущей строке
                        MaxIndex = MatrixCopy.GetMaxAbsInRowIndex(i);
                        MatrixCopy.SwapColumns(i, MaxIndex);
                        SwapDict(offset,i,MaxIndex);
                        break;
                    case GEModification.LeadingOnTheColumn:
                        MaxIndex = MatrixCopy.GetMaxAbsInColumnIndex(i);
                        MatrixCopy.SwapLines(i, MaxIndex);
                        SwapDict(offset, i, MaxIndex);
                        break;
                    case GEModification.LeadingOnWholeMatrix:
                        int MaxIndexI, MaxIndexJ;
                        int[] max = MatrixCopy.GetMaxAbsIndex();
                        MaxIndexI = max[0];
                        MaxIndexJ = max[1];
                        MatrixCopy.SwapLines(i, MaxIndexI);
                        SwapDict(offset, i, MaxIndexI);

                        MatrixCopy.SwapColumns(i, MaxIndexJ);
                        SwapDict(offset, i, MaxIndexJ);
                        break;
                    default:
                        break;

                }
                for (int j = i + 1; j < MatrixCopy.LengthY; j++)
                {
                    double Multiplier = -(MatrixCopy[j][i] / MatrixCopy.Values[i][i]);
                    MatrixCopy[j] = MatrixCopy[j].Select((val, index) => val + MatrixCopy.Values[i][index] * Multiplier).ToArray();
                }
                //У всех остальных строчек обнулили i-ый столбец
            }
            //Обратный ход
            double[] Result = new double[MatrixCopy.Values.Length];
            Result[Result.Length - 1] = MatrixCopy.Values[MatrixCopy.Values.Length - 1].Last();
            for (int i = Result.Length - 2; i >= 0; i--)
                Result[i] = MatrixCopy.Values[i].Last() - CalcSum(MatrixCopy, i, Result);
            return offset.OrderBy(row => row.Value).Select(row => Result[row.Key]).ToArray();

        }

        private static void SwapDict(Dictionary<int, int> Source, int i, int j)
        {
            int buffer = Source[i];
            Source[i] = Source[j];
            Source[j] = buffer;
        }

        /// <summary>
        /// Модификации метода Гаусса.
        /// </summary>
        public enum GEModification
        {
            /// <summary>
            /// Стандартный алгоритм.
            /// </summary>
            Standart,
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

        private static double CalcSum(Matrix matrix, int index, double[] ResultArray)
        {
            double Sum = 0;
            for (int i = index + 1; i < matrix.LengthY; i++)
                Sum += matrix[index][i] * ResultArray[i];
            return Sum;
        }
    }
}
