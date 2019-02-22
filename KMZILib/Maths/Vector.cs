using System;
using System.Collections.Generic;
using System.Linq;

namespace KMZILib
{
    /// <summary>
    ///     Представляет вектор с вещественными координатами.
    /// </summary>
    public class Vector
    {
        /// <summary>
        ///     Инициализирует новый вектор по заданному набору координат.
        /// </summary>
        /// <param name="coords"></param>
        public Vector(double[] coords)
        {
            Coordinates = new double[coords.Length];
            coords.CopyTo(Coordinates, 0);
        }

        /// <summary>
        /// Инициализирует новый пустой вектор заданной длины.
        /// </summary>
        /// <param name="n"></param>
        public Vector (int n)
        {
            Coordinates = new double[n];
        }

        /// <summary>
        ///     Инициализирует новый вектор по заданной матрице.
        /// </summary>
        /// <param name="Source"></param>
        public Vector(Matrix Source)
        {
            List<double> VectorArray = new List<double>();
            foreach (double[] row in Source.Values)
                VectorArray.AddRange(row);
            Coordinates = VectorArray.ToArray();
        }

        /// <summary>
        ///     Координаты в виде списка вещественных чисел.
        /// </summary>
        public double[] Coordinates { get; }

        /// <summary>
        ///     Число координат вектора.
        /// </summary>
        public int CoordinatesLength => Coordinates.Length;

        /// <summary>
        ///     Длина вектора.
        /// </summary>
        public double Length => Math.Sqrt(Coordinates.Select(val => Math.Pow(val, 2)).Sum());

        /// <summary>
        ///     Доступ к координатам вектора по индексам.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double this[int index]
        {
            get => Coordinates[index];
            set => Coordinates[index] = value;
        }

        /// <summary>
        ///     Возвращает координаты вектора в виде массива.
        /// </summary>
        /// <returns></returns>
        public double[] ToArray()
        {
            double[] Result = new double[CoordinatesLength];
            Coordinates.CopyTo(Result, 0);
            return Result;
        }

        /// <summary>
        ///     Сложение двух векторов. Длины координат вектором не обязательно должны быть равными.
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static Vector operator +(Vector First, Vector Second)
        {
            double[] MinArray = First.CoordinatesLength < Second.CoordinatesLength
                ? First.Coordinates
                : Second.Coordinates;
            double[] MaxArray = First.CoordinatesLength > Second.CoordinatesLength
                ? First.Coordinates
                : Second.Coordinates;
            int offset = MaxArray.Length - MinArray.Length;
            double[] Result = new double[MaxArray.Length];
            for (int i = 0; i < offset; i++)
                Result[i] = MaxArray[i];
            for (int i = offset; i < Result.Length; i++)
                Result[i] = MaxArray[i] + MinArray[i - offset];
            return new Vector(Result);
        }

        /// <summary>
        ///     Разность двух векторов. Длины координат вектором не обязательно должны быть равными.
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static Vector operator -(Vector First, Vector Second)
        {
            return First + -Second;
        }

        /// <summary>
        ///     Возвращает новый вектор с отрицаниями всех координат.
        /// </summary>
        /// <param name="First"></param>
        /// <returns></returns>
        public static Vector operator -(Vector First)
        {
            return new Vector(First.Coordinates.Select(val => -val).ToArray());
        }

        /// <summary>
        ///     Возвращает матрицу, заданную по текущему вектору.
        /// </summary>
        /// <returns></returns>
        public Matrix ToMatrix()
        {
            return new Matrix(this);
        }

        /// <summary>
        ///     Возвращает строковое представление вектора.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return
                $"({string.Join(", ", Coordinates.Select(coord => Math.Round(coord) == coord ? coord.ToString() : $"{coord:F3}"))})";
        }
    }
}