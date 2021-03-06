﻿using System;
using System.Collections.Generic;
using System.Linq;

using Google.Protobuf;

namespace OSULib.Maths
{
    /// <summary>
    ///     Представляет собой матрицу. Необязательно прямоугольную.
    /// </summary>
    public class Matrix : ICloneable
    {
        #region Поля
        /// <summary>
        ///     Обозначает, имеет ли матрица столбец свободных членов.
        /// </summary>
        public bool HasFreeCoefficient;
        #endregion

        #region Конструкторы
        /// <summary>
        ///     Инициализирует новую матрицу с заданным массивом вещественных коэффициентов.
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
        ///     Инициализирует новую матрицу с заданным массивом целочисленных коэффициентов.
        /// </summary>
        /// <param name="Source"></param>
        public Matrix(int[][] Source) : this(Source.Select(row => row.Select(element => (double)element).ToArray())
            .ToArray())
        {
        }

        /// <summary>
        ///     Инициализирует новую матрицу, которая является копией заданной матрицы.
        /// </summary>
        /// <param name="Source"></param>
        public Matrix(Matrix Source) : this(Source.Values)
        {
            HasFreeCoefficient = Source.HasFreeCoefficient;
        }

        /// <summary>
        ///     Инициализирует новую матрицу по заданному вектору значений.
        /// </summary>
        /// <param name="Source"></param>
        public Matrix(Vector Source) : this(new[] { Source.ToArray() })
        {
        }

        /// <summary>
        ///     Инициализирует новую пустую матрицу заданной размерности.
        /// </summary>
        /// <param name="n">Число строк матрицы.</param>
        /// <param name="m">Число столбцов матрицы.</param>
        public Matrix(int n, int m)
        {
            if (n < 0 || m < 0)
                throw new InvalidOperationException($"Число строк({n}) или столбцов({m}) не может быть отрицательным!");
            Values = new double[n][].Select(row => new double[m]).ToArray();
        }

        /// <summary>
        ///     Инициализирует новую пустую квадратную матрицу заданной размерности.
        /// </summary>
        /// <param name="n"></param>
        public Matrix(int n) : this(n, n)
        {

        }

        #endregion

        #region Свойства
        /// <summary>
        ///     Массив коэффициентов матрицы в виде вещественных чисел.
        /// </summary>
        public double[][] Values { get; private set; }

        /// <summary>
        ///     Число строк матрицы.
        /// </summary>
        public int LengthY => Values.Length;

        /// <summary>
        ///     Число столбцов матрицы. Вернёт -1, если нет строчек или если матрица не прямоугольная.
        /// </summary>
        public int LengthX
        {
            get
            {
                if (LengthY == 0)
                    return 0;
                int Length = Values.First().Length;
                if (Values.Any(Doubles => Doubles.Length != Length))
                    return -1;
                return HasFreeCoefficient ? Length - 1 : Length;
            }
        }

        /// <summary>
        ///     Возвращает копию данной матрицы без столбца свободных членов.
        /// </summary>
        public Matrix WithoutFreeCoefficients
        {
            get
            {
                return !HasFreeCoefficient
                    ? Copy()
                    : new Matrix(Values.Select(row => row.Take(row.Length - 1).ToArray()).ToArray());
            }
        }

        /// <summary>
        ///     Определяет, является ли матрица квадратной. Зависит от стобца свободных членов <see cref="HasFreeCoefficient" />.
        /// </summary>
        public bool IsSquare => LengthY == LengthX;

        /// <summary>
        ///     Определитель данной матрицы.
        /// </summary>
        public double Definite
        {
            get
            {
                Matrix MatrixCopy = new Matrix(this);
                //Прямой ход
                if (!MatrixCopy.IsSquare)
                {
                    throw new InvalidOperationException(
                        "Нахождение определителя возможно только для квадратной матрицы.");
                }

                double Result = 1;
                for (int i = 0; i < MatrixCopy.LengthY; i++)
                {
                    Result *= MatrixCopy[i][i];
                    MatrixCopy[i] = MatrixCopy[i].Select(val => val / MatrixCopy[i][i]).ToArray();
                    //Сделали первый ненулевой элемент единицей
                    for (int j = i + 1; j < MatrixCopy.LengthY; j++)
                    {
                        double Multiplier = -(MatrixCopy[j][i] / MatrixCopy[i][i]);
                        MatrixCopy[j] = MatrixCopy[j].Select((val, index) => val + MatrixCopy[i][index] * Multiplier)
                            .ToArray();
                    }

                    //У всех остальных строчек обнулили i-ый столбец
                }

                return Result;
            }
        }

        /// <summary>
        ///     Обратная матрица для данной.
        /// </summary>
        public Matrix Reverse
        {
            get
            {
                Matrix Result = GetZeroMatrix(LengthY);


                Matrix buffer = new Matrix(LengthY, LengthY + 1);
                for (int i = 0; i < buffer.LengthY; i++)
                    for (int j = 0; j < buffer.LengthY; j++)
                        buffer[i][j] = Values[i][j];
                //Создали копию матрицы. теперь можно генерить столбцы свободных членов.
                //Столбцы обратной матрицы - столбцы b в системах линейных уравнений.
                for (int i = 0; i < LengthY; i++)
                {
                    for (int j = 0; j < LengthY; j++)
                        buffer[j][buffer.LengthX - 1] = j == i ? 1 : 0;
                    Vector BufferResult = LinearEquations.GaussMethod.Solve(buffer);
                    for (int j = 0; j < LengthY; j++)
                        Result[j][i] = BufferResult[j];
                }

                return Result;
            }
        }

        /// <summary>
        ///     Норма данной матрицы.
        /// </summary>
        public double Norm => Math.Sqrt(!HasFreeCoefficient
            ? Values.Select(row => row.Select(element => Math.Pow(element, 2)).Sum()).Sum()
            : Values.Select(row => row.Take(row.Length - 1).Select(element => Math.Pow(element, 2)).Sum()).Sum());

        /// <summary>
        /// Определеяет, имеет ли данная матрица диагональное преобладание.
        /// </summary>
        public bool IsDiagonallyDominant
        {
            get
            {
                for (int i = 0; i < LengthY; i++)
                {
                    double Sum = Values[i].Where((element, index) => index != i).Select(Math.Abs).Sum();
                    if (Math.Abs(Values[i][i]) < Sum)
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        ///     Доступ к строкам матрицы по заданным индексам.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double[] this[int index]
        {
            get => Values[index];
            set => Values[index] = value;
        }

        /// <summary>
        ///     Поэлементный доступ к матрице по заданным индексам.
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <returns></returns>
        public double this[int index1, int index2]
        {
            get => Values[index1][index2];
            set => Values[index1][index2] = value;
        }

        #endregion

        #region Методы
        /// <summary>
        ///     Осуществляет транспонирование текущей матрицы.
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
        ///     Возвращает транспонированную копию текущей матрицы.
        /// </summary>
        /// <returns></returns>
        public Matrix TransposedCopy()
        {
            Matrix Copy = new Matrix(this);
            Copy.Transpose();
            return Copy;
        }

        /// <summary>
        ///     Возвращает копию текущей матрицы.
        /// </summary>
        /// <returns></returns>
        public Matrix Copy()
        {
            return new Matrix(this);
        }

        /// <summary>
        ///     Переводит матрицу в формат вектора.
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
        ///     Меняет местами две строки в матрице.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public void SwapLines(int first, int second)
        {
            if (first == second)
                return;
            if (first < 0 || first >= Values.Length ||
                second < 0 || second >= Values.Length)
            {
                throw new InvalidOperationException(
                    $"Один из индексов лежит вне допустимого диапазона! (Min = 0, Max = {Values.Length - 1})");
            }

            double[] bufferrow = Values[first];
            Values[first] = Values[second];
            Values[second] = bufferrow;
        }

        /// <summary>
        ///     Меняет местами два столбца в матрице.
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
                {
                    throw new InvalidOperationException(
                        $"Один из индексов лежит вне допустимого диапазона! (Min = 0, Max = {row.Length - 1})");
                }

            foreach (double[] row in Values)
            {
                double Copy = row[first];
                row[first] = row[second];
                row[second] = Copy;
            }
        }

        /// <summary>
        ///     Получает строку в матрице по заданному индексу.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double[] GetRow(int index)
        {
            if (index < 0 || index >= Values.Length)
            {
                throw new InvalidOperationException(
                    $"Индекс лежит вне допустимого диапазона! (Min = 0, Max = {Values.Length - 1})");
            }

            return HasFreeCoefficient ? Values[index].Take(Values[index].Length - 1).ToArray() : Values[index];
        }

        /// <summary>
        ///     Получает столбец в матрице по заданному индексу.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double[] GetColumn(int index)
        {
            foreach (double[] row in Values)
                if (index < 0 || index >= row.Length)
                {
                    throw new InvalidOperationException(
                        $"Один из индексов лежит вне допустимого диапазона! (Min = 0, Max = {row.Length - 1})");
                }

            return Values.Select(row => row[index]).ToArray();
        }

        /// <summary>
        ///     Возвращает максимальный элемент в строке с заданным индексом.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double GetMaxInRow(int index)
        {
            return GetRow(index).Max();
        }

        /// <summary>
        ///     Возвращает максимальный по модулю элемент в строке с заданным индексом.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double GetMaxAbsInRow(int index)
        {
            return GetRow(index).Select(Math.Abs).Max();
        }

        /// <summary>
        ///     Возвращает максимальный по модулю элемент в столбце с заданным индексом.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double GetMaxAbsInColumn(int index)
        {
            return GetColumn(index).Select(Math.Abs).Max();
        }

        /// <summary>
        ///     Возвращает максимальный элемент в столбце с заданным индексом.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double GetMaxInColumn(int index)
        {
            return GetColumn(index).Max();
        }

        /// <summary>
        ///     Возвращает индекс максимального элемента в строке с заданным индексом.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetMaxInRowIndex(int index)
        {
            List<double> TargetRow = GetRow(index).ToList();
            return TargetRow.IndexOf(TargetRow.Max());
        }

        /// <summary>
        ///     Возвращает индекс максимального по модулю элемента в строке с заданным индексом.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetMaxAbsInRowIndex(int index)
        {
            List<double> TargetRow = GetRow(index).Select(Math.Abs).ToList();
            return TargetRow.IndexOf(TargetRow.Max());
        }

        /// <summary>
        ///     Возвращает индекс максимального элемента в столбце с заданным индексом.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetMaxInColumnIndex(int index)
        {
            List<double> TargetColumn = GetColumn(index).ToList();
            return TargetColumn.IndexOf(TargetColumn.Max());
        }

        /// <summary>
        ///     Возвращает индекс максимального по модулю элемента в столбце с заданным индексом.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetMaxAbsInColumnIndex(int index)
        {
            List<double> TargetColumn = GetColumn(index).Select(Math.Abs).ToList();
            return TargetColumn.IndexOf(TargetColumn.Max());
        }

        /// <summary>
        ///     Возвращает максимальный элемент в матрице.
        /// </summary>
        /// <returns></returns>
        public double GetMax()
        {
            return HasFreeCoefficient
                ? Values.Select(row => row.Take(row.Length - 1).Max()).Max()
                : Values.Select(row => row.Max()).Max();
        }

        /// <summary>
        ///     Возвращает минимальный элемент в матрице.
        /// </summary>
        /// <returns></returns>
        public double GetMin()
        {
            return HasFreeCoefficient
                ? Values.Select(row => row.Take(row.Length - 1).Min()).Min()
                : Values.Select(row => row.Min()).Min();
        }

        /// <summary>
        ///     Возвращает максимальный по модулю элемент в матрице.
        /// </summary>
        /// <returns></returns>
        public double GetMaxAbs()
        {
            return HasFreeCoefficient
                ? Values.Select(row => row.Take(row.Length - 1).Select(Math.Abs).Max()).Max()
                : Values.Select(row => row.Select(Math.Abs).Max()).Max();
        }

        /// <summary>
        ///     Возвращает максимальный по модулю элемент в матрице.
        /// </summary>
        /// <returns></returns>
        public double GetMinAbs()
        {
            return HasFreeCoefficient
                ? Values.Select(row => row.Take(row.Length - 1).Select(Math.Abs).Min()).Min()
                : Values.Select(row => row.Select(Math.Abs).Min()).Min();
        }

        /// <summary>
        ///     Возвращает индекс максимального элемента в матрице.
        /// </summary>
        /// <returns></returns>
        public int[] GetMaxIndex()
        {
            double Max = GetMax();
            for (int i = 0; i < LengthY; i++)
            {
                List<double> arr = GetRow(i).ToList();
                int index = arr.IndexOf(Max);
                if (index != -1)
                    return new[] { i, index };
            }

            return new[] { -1, -1 };
        }

        /// <summary>
        ///     Возвращает индекс максимального по модулю элемента в матрице.
        /// </summary>
        /// <returns></returns>
        public int[] GetMaxAbsIndex()
        {
            double Max = GetMaxAbs();
            for (int i = 0; i < Values.Length; i++)
            {
                int index = GetRow(i).Take(Values[i].Length - 1).Select(Math.Abs).ToList().IndexOf(Max);
                if (index != -1)
                    return new[] { i, index };
            }

            return new[] { -1, -1 };
        }

        /// <summary>
        /// Вовзвращает матрицу, в которой удалена указанная строка.
        /// </summary>
        /// <param name="rowIndex">Индекс строки, которую нужно удалить. (отсчет с 0)</param>
        /// <returns>Матрица, в которой удалена указанная строка.</returns>
        public Matrix RemoveRow(int rowIndex)
        {
            if (LengthY <= 1) return new Matrix(0);
            Matrix result = new Matrix(LengthY - 1, LengthX);
            int row = 0;
            while (row != rowIndex)
            {
                for (int j = 0; j < result.LengthX; j++)
                    result[row, j] = this[row, j];
                row++;
            }

            row++;
            while (row != LengthY)
            {
                for (int j = 0; j < result.LengthX; j++)
                    result[row - 1, j] = this[row, j];
                row++;
            }

            return result;
        }

        /// <summary>
        /// Вовзвращает матрицу, в которой удален указанный столбец.
        /// </summary>
        /// <param name="columnIndex">Индекс столбца, который нужно удалить. (отсчет с 0)</param>
        /// <returns>Матрица, в которой удален указанный столбец.</returns>
        public Matrix RemoveColumn(int columnIndex)
        {
            if (LengthX <= 1) return new Matrix(0);
            Matrix result = new Matrix(LengthY, LengthX - 1);
            int column = 0;
            while (column != columnIndex)
            {
                for (int i = 0; i < result.LengthY; i++)
                    result[i, column] = this[i, column];
                column++;
            }

            column++;
            while (column != LengthX)
            {
                for (int i = 0; i < result.LengthY; i++)
                    result[i, column - 1] = this[i, column];
                column++;
            }

            return result;
        }

        /// <summary>
        /// Производит инкрементацию всех значений в матрице на указанное число.
        /// </summary>
        /// <param name="value">Число, которое необходимо прибавить ко всем элементам матрицы.</param>
        public void IncrementBy(double value)
        {
            for (int i = 0; i < Values.Length; i++)
                for (int j = 0; j < Values[i].Length; j++)
                    Values[i][j] += value;
        }
        /// <summary>
        /// Производит декрементацию всех значений в матрице на указанное число.
        /// </summary>
        /// <param name="value">Число, которое необходимо отнять от всех элементов матрицы.</param>
        public void DecrementBy(double value)
        {
            IncrementBy(-value);
        }

        /// <summary>
        /// Производит умножение всех элементов данной матрицы на указанный коэффициент.
        /// </summary>
        /// <param name="value">Коэффициент, на который необходимо умножить все элементы матрицы.</param>
        public void MultiplyBy(double value)
        {
            for (int i = 0; i < Values.Length; i++)
                for (int j = 0; j < Values[i].Length; j++)
                    Values[i][j] *= value;
        }

        /// <summary>
        /// Возвращает копию данной матрицы, в которой все элементы умножены на указанный коэффициент.
        /// </summary>
        /// <param name="multiplier">Коэффициент, на который необходимо умножить все элементы матрицы.</param>
        /// <returns>Матрица-копия данной, в которой все элементы умножены на указанный коэффициент.</returns>
        public Matrix GetMultipliedCopy(double multiplier)
        {
            Matrix result = this.Copy();
            result.MultiplyBy(multiplier);
            return result;
        }

        /// <summary>
        ///     Возвращает единичную матрицу заданной размерности.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Matrix GetUnitMatrix(int n)
        {
            Matrix Result = new Matrix(n, n);
            for (int i = 0; i < n; i++)
                Result[i][i] = 1;
            return Result;
        }

        /// <summary>
        ///     Возвращает нулевую матрицу заданной размерности.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Matrix GetZeroMatrix(int n)
        {
            return new Matrix(n, n);
        }

        /// <summary>
        /// Возвращает новую матрицу, в которой вырезаны i cтрока и j столбец.
        /// </summary>
        /// <param name="i">Номер строки, которую нужно вырезать.</param>
        /// <param name="j">Номер столбца, который нужно вырезать.</param>
        /// <returns>Новая матрица, в которой вырезаны i cтрока и j столбец.</returns>
        public Matrix GetSubmatrix(int i, int j)
        {
            double[][] Result = Values
                .Select(row => row.Where((elem, ind) => ind != j).ToArray())
                .Where((row, ind) => ind != i).ToArray();
            return new Matrix(Result);
        }

        /// <summary>
        /// Возвращает дополнительный минор данной матрицы
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public double GetMinor(int i, int j) => GetSubmatrix(i, j).Definite;

        /// <summary>
        /// Возвращает алгебраическое дополнение для заданных матрицы и координат.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns>Алгебраическое дополнение для заданных матрицы и координат.</returns>
        public double GetAlgebraicAddition(int i, int j)
        {
            return Math.Pow(-1, i + j) * GetMinor(i, j);
        }

        /// <summary>
        ///     Возвращает матрицу в виде массива коэффициентов.
        /// </summary>
        /// <returns>Матрица в виде массива коэффициентов.</returns>
        public double[][] ToArray()
        {
            double[][] Result = new double[Values.Length][];
            for (int i = 0; i < Values.Length; i++)
            {
                Values[i] = new double[Values[i].Length];
                Values[i].CopyTo(Values[i], 0);
            }

            return Result;
        }

        #endregion

        #region Операторы

        /// <summary>
        ///     Осуществляет сложение двух матриц и возвращает результат - новую матрицу.
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns>Матрица - результат сложения.</returns>
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
        ///     Осуществляет разность двух матриц и возвращает результат - новую матрицу.
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns>Матрица - результат разности.</returns>
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
        ///     Осуществляет умножение двух матриц и возвращает результат - новую матрицу.
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns>Матрица - результат произведения.</returns>
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


        #endregion




        /// <summary>
        ///     Возвращает строковое представление матрицы.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Join("\n", Values.Select(row => string.Join("\t", row.Select(
                element => $"{(Math.Round(element) == element ? element.ToString() : $"{element:F3}")}"))));
        }

        /// <summary>
        /// Выполняет поэлементное умножение матрицы и возвращает результат.
        /// </summary>
        /// <param name="other">Матрица, с которой необходимо выполнить поэлементное умножение.</param>
        /// <returns>Матрица - результат поэлементного умножения.</returns>
        public Matrix ElementMult(Matrix other)
        {
            if (LengthX != other.LengthX || LengthY != other.LengthY)
                throw new InvalidOperationException("ПОэлементное умножение матриц разной размерности не поддерживается");
            Matrix result = new Matrix(LengthY);
            for (int i = 0; i < LengthY; i++)
                for (int j = 0; j < LengthX; j++)
                    result[i, j] = this[i, j] * other[i, j];
            return result;
        }

        /// <summary>
        /// Выполняет произведение Кронокера с указанной матрицей и возвращает результат.
        /// </summary>
        /// <param name="other">Матрица, с которой необходимо выполнить произведение Кронокера.</param>
        /// <returns>Матрица - результат умножения Кронокера.</returns>
        public Matrix KroneckerProduct(Matrix other)
        {
            Matrix Result = new Matrix(LengthY * other.LengthY, LengthX * other.LengthX);

            for (int i = 0; i < Result.LengthY; i++)
            {
                for (int j = 0; j < Result.LengthX; j++)
                    Result[i, j] = this[i / other.LengthY, j / other.LengthX] *
                                   other[i % other.LengthY, j % other.LengthX];
                Console.WriteLine();
            }

            return Result;
        }

        /// <summary>
        /// Возвдеение матрицы в указанную степень.
        /// </summary>
        /// <param name="source">Основание-матрица.</param>
        /// <param name="degree">Степень, в которую необходимо возвести матрицу.</param>
        /// <returns>Матрица, возведенная в указанную степень.</returns>
        public Matrix Pow(uint degree)
        {

            bool[] maAltorithm = Misc.Misc.GetBinaryArray(degree).Skip(1).ToArray();

            Matrix A = Copy();
            Matrix result = A.Copy();

            foreach (bool bit in maAltorithm)
            {
                result *= result;
                if (bit)
                    result *= A;
            }
            return result;
        }

        /// <summary>
        /// Возвращает копию текущей матрицы. Реализация <see cref="ICloneable"/>.
        /// </summary>
        /// <returns>Копия текущей матрицы.</returns>
        public object Clone()
        {
            return this.Copy();
        }
    }

    /// <summary>
    /// Представляет меотды расширения для класса <see cref="Matrix"/>
    /// </summary>
    public static class MatrixExtensions
    {

    }
}