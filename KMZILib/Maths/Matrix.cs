using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMZILib.Maths
{
    /// <summary>
    /// Представляет собой матрицу. Необязательно прямоугольную.
    /// </summary>
    public class Matrix
    {
        /// <summary>
        /// Массив коэффициентов матрицы в виде вещественных чисел.
        /// </summary>
        public double[][] Values;

        /// <summary>
        /// Число строк матрицы.
        /// </summary>
        public int LengthY => Values.Length;

        /// <summary>
        /// Число столбцов матрицы. Вернёт -1, если нет строчек или если матрица не прямоугольная.
        /// </summary>
        public int LengthX
        {
            get
            {
                if (LengthY == 0)
                    return -1;
                int Length = Values.First().Length;
                if (Values.Any(Doubles => Doubles.Length != Length))
                    return -1;
                return Length;
            }
        }

        /// <summary>
        /// Инициализирует новую матрицу с заданным массивом коэффициентов.
        /// </summary>
        /// <param name="Source"></param>
        public Matrix(double[][] Source)
        {
            Values = new double[Source.Length][];
            for (int i = 0; i < Values.Length; i++)
            {
                Values[i] = new double[Source[i].Length];
                Source[i].CopyTo(Values[i], 0);
            }
        }

        /// <summary>
        /// Осуществляет умножение двух матриц и возвращает результат - новую матрицу.
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static Matrix operator *(Matrix First, Matrix Second)
        {
            if (First.LengthY != Second.LengthX)
                throw new InvalidOperationException("Число строк первой матрицы и число столбцов второй не совпадают!");
            double[][] Result = new double[First.LengthX][];
            for (int i = 0; i < Result.Length; i++)
                Result[i] = new double[Second.LengthY];
            for (int i = 0; i < Result.Length; i++)
                for (int j = 0; j < Result.First().Length; j++)
                    Result[i][j] = Second.Values.Select(row => row[j])
                        .Select((element, index) => element * First.Values[i][index]).Sum();
            return new Matrix(Result);
        }

        /// <summary>
        /// Осуществляет сложение двух матриц и возвращает результат - новую матрицу.
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static Matrix operator +(Matrix First, Matrix Second)
        {
            if (First.LengthX != Second.LengthX ||
               First.LengthY != Second.LengthY)
                throw new InvalidOperationException("Размеры матриц должны быть одинаковыми.");
            double[][] Result = new double[First.LengthY][];
            for (int i = 0; i < Result.Length; i++)
                Result[i] = First.Values[i].Select((element, index) => element + Second.Values[i][index]).ToArray();
            return new Matrix(Result);
        }

        /// <summary>
        /// Осуществляет разность двух матриц и возвращает результат - новую матрицу.
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static Matrix operator -(Matrix First, Matrix Second)
        {
            if (First.LengthX != Second.LengthX ||
                First.LengthY != Second.LengthY)
                throw new InvalidOperationException("Размеры матриц должны быть одинаковыми.");
            double[][] Result = new double[First.LengthY][];
            for (int i = 0; i < Result.Length; i++)
                Result[i] = First.Values[i].Select((element, index) => element - Second.Values[i][index]).ToArray();
            return new Matrix(Result);
        }
        /// <summary>
        /// Возвращает строковое представление матрицы.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Join("\n", Values.Select(row => string.Join("\t", row)));
        }
    }
}
