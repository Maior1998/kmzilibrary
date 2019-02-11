using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace KMZILib
{
    /// <summary>
    /// Представляет собой матрицу. Необязательно прямоугольную.
    /// </summary>
    public class Matrix
    {
        /// <summary>
        /// Массив коэффициентов матрицы в виде вещественных чисел.
        /// </summary>
        public double[][] Values { get; private set; }

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
        /// Обозначает, имеет ли матрица столбец свободных членов.
        /// </summary>
        public bool HasFreeCoefficient;

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
        /// Инициализирует новую матрицу, которая является копией заданной матрицы.
        /// </summary>
        /// <param name="Source"></param>
        public Matrix(Matrix Source) : this(Source.Values) { }

        /// <summary>
        /// Инициализирует новую матрицу по заданному вектору значений.
        /// </summary>
        /// <param name="Source"></param>
        public Matrix(Vector Source) : this(new[] { Source.ToArray() }) { }

        /// <summary>
        /// Осуществляет умножение двух матриц и возвращает результат - новую матрицу.
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static Matrix operator *(Matrix First, Matrix Second)
        {
            if (First.LengthX != Second.LengthY)
                throw new InvalidOperationException("Число столбцов первой матрицы и число строк второй не совпадают!");
            double[][] Result = new double[First.LengthY][];
            for (int i = 0; i < Result.Length; i++)
                Result[i] = new double[Second.LengthX];
            for (int ResultRow = 0; ResultRow < Result.Length; ResultRow++)
                for (int ResultColumn = 0; ResultColumn < Result.First().Length; ResultColumn++)
                {
                    double Sum = 0;
                    for (int i = 0; i < First.LengthX; i++)
                        Sum += First.Values[ResultRow][i] * Second.Values[i][ResultColumn];
                    Result[ResultRow][ResultColumn] = Sum;
                }
            return new Matrix(Result);
        }

        /// <summary>
        /// Осуществляет транспонирование текущей матрицы
        /// </summary>
        public Matrix Transpose()
        {
            double[][] NewMatrix = new double[LengthX][];
            for (int i = 0; i < NewMatrix.Length; i++)
            {
                NewMatrix[i] = new double[LengthY];
                for (int j = 0; j < NewMatrix[i].Length; j++)
                    NewMatrix[i][j] = Values[j][i];
            }
            Values = NewMatrix;
            return this;
        }

        /// <summary>
        /// Возвращает транспонированную копию текущей матрицы.
        /// </summary>
        /// <returns></returns>
        public Matrix TransposedCopy()
        {
            Matrix Copy = new Matrix(this);
            Copy.Transpose();
            return Copy;
        }

        /// <summary>
        /// Возвращает копию текущей матрицы.
        /// </summary>
        /// <returns></returns>
        public Matrix Copy()
        {
            return new Matrix(this);
        }

        /// <summary>
        /// Переводит матрицу в формат вектора.
        /// </summary>
        /// <returns></returns>
        public Vector ToVector()
        {
            List<double> VectorArray = new List<double>();
            foreach (double[] row in Values)
                VectorArray.AddRange(row);
            return new Vector(VectorArray.ToArray());
        }

        /// <summary>
        /// Меняет местами две строки в матрице.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public void SwapLines(int first, int second)
        {
            if (first == second)
                return;
            if (first < 0 || first >= Values.Length ||
               second < 0 || second >= Values.Length)
                throw new InvalidOperationException($"Один из индексов лежит вне допустимого диапазона! (Min = 0, Max = {Values.Length - 1})");
            double[] bufferrow = Values[first];
            Values[first] = Values[second];
            Values[second] = bufferrow;
        }

        /// <summary>
        /// Меняет местами два столбца в матрице.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public void SwapColumns(int first, int second)
        {
            if (first == second)
                return;
            foreach (double[] row in Values)
                if (first < 0 || first >= row.Length ||
                    second < 0 || second >= row.Length)
                    throw new InvalidOperationException($"Один из индексов лежит вне допустимого диапазона! (Min = 0, Max = {row.Length - 1})");

            foreach (double[] row in Values)
            {
                double Copy = row[first];
                row[first] = row[second];
                row[second] = Copy;
            }
        }

        /// <summary>
        /// Получает строку в матрице по заданному индексу.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double[] GetRow(int index)
        {
            if (index < 0 || index >= Values.Length)
                throw new InvalidOperationException($"Индекс лежит вне допустимого диапазона! (Min = 0, Max = {Values.Length - 1})");

            return HasFreeCoefficient ? Values[index].Take(Values[index].Length - 1).ToArray() : Values[index];
        }

        /// <summary>
        /// Получает столбец в матрице по заданному индексу.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double[] GetColumn(int index)
        {
            foreach (double[] row in Values)
                if (index < 0 || index >= row.Length)
                    throw new InvalidOperationException($"Один из индексов лежит вне допустимого диапазона! (Min = 0, Max = {row.Length - 1})");
            return Values.Select(row => row[index]).ToArray();
        }

        /// <summary>
        /// Возвращает максимальный элемент в строке с заданным индексом.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double GetMaxInRow(int index)
        {
            return GetRow(index).Max();
        }

        /// <summary>
        /// Возвращает максимальный по модулю элемент в строке с заданным индексом.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double GetMaxAbsInRow(int index)
        {
            return GetRow(index).Select(Math.Abs).Max();
        }

        /// <summary>
        /// Возвращает максимальный по модулю элемент в столбце с заданным индексом.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double GetMaxAbsInColumn(int index)
        {
            return GetColumn(index).Select(Math.Abs).Max();
        }

        /// <summary>
        /// Возвращает максимальный элемент в столбце с заданным индексом.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double GetMaxInColumn(int index)
        {
            return GetColumn(index).Max();
        }

        /// <summary>
        /// Возвращает индекс максимального элемента в строке с заданным индексом.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetMaxInRowIndex(int index)
        {
            List<double> TargetRow = GetRow(index).ToList();
            return TargetRow.IndexOf(TargetRow.Max());
        }

        /// <summary>
        /// Возвращает индекс максимального по модулю элемента в строке с заданным индексом.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetMaxAbsInRowIndex(int index)
        {
            List<double> TargetRow = GetRow(index).Select(Math.Abs).ToList();
            return TargetRow.IndexOf(TargetRow.Max());
        }

        /// <summary>
        /// Возвращает индекс максимального элемента в столбце с заданным индексом.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetMaxInColumnIndex(int index)
        {
            List<double> TargetColumn= GetColumn(index).ToList();
            return TargetColumn.IndexOf(TargetColumn.Max());
        }

        /// <summary>
        /// Возвращает индекс максимального по модулю элемента в столбце с заданным индексом.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetMaxAbsInColumnIndex(int index)
        {
            List<double> TargetColumn = GetColumn(index).Select(Math.Abs).ToList();
            return TargetColumn.IndexOf(TargetColumn.Max());
        }

        /// <summary>
        /// Доступ к строкам матрицы по заданным индексам.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double[] this[int index]
        {
            get => Values[index];
            set => Values[index] = value;
        }

        /// <summary>
        /// Поэлементный доступ к матрице по заданным индексам.
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <returns></returns>
        public double this[int index1, int index2]
        {
            get => Values[index1][index2];
            set => Values[index1][index2] = value;
        }

        /// <summary>
        /// Возвращает максимальный элемент в матрице.
        /// </summary>
        /// <returns></returns>
        public double GetMax()
        {
            return Values.Select(row => row.Max()).Max();
        }

        /// <summary>
        /// Возвращает максимальный по модулю элемент в матрице.
        /// </summary>
        /// <returns></returns>
        public double GetMaxAbs()
        {
            return Values.Select(row => row.Select(Math.Abs).Max()).Max();
        }

        /// <summary>
        /// Возвращает индекс максимального элемента в матрице.
        /// </summary>
        /// <returns></returns>
        public int[] GetMaxIndex()
        {
            double Max = GetMax();
            for(int i=0;i<Values.Length;i++)
            {
                int index = Values[i].ToList().IndexOf(Max);
                if (index != -1) return new []{i,index};
            }

            return new[]{-1,-1};
        }

        /// <summary>
        /// Возвращает индекс максимального по модулю элемента в матрице.
        /// </summary>
        /// <returns></returns>
        public int[] GetMaxAbsIndex()
        {
            double Max = GetMaxAbs();
            for (int i = 0; i < Values.Length; i++)
            {
                int index = Values[i].Select(Math.Abs).ToList().IndexOf(Max);
                if (index != -1)
                    return new[] { i, index };
            }

            return new[] { -1, -1 };
        }

        /// <summary>
        /// Возвращает матрицу в виде массива коэффициентов.
        /// </summary>
        /// <returns></returns>
        public double[][] ToArray()
        {
            double[][]Result = new double[Values.Length][];
            for (int i = 0; i < Values.Length; i++)
            {
                Values[i] = new double[Values[i].Length];
                Values[i].CopyTo(Values[i], 0);
            }

            return Result;
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
