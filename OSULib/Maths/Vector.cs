using System;
using System.Collections.Generic;
using System.Linq;

namespace OSULib.Maths
{
    /// <summary>
    ///     Представляет вектор с вещественными координатами.
    /// </summary>
    public class Vector: IEquatable<Vector>
    {
        #region Свойства
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
        #endregion

        #region Конструкторы

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
        public Vector(int n)
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

        #endregion

        #region Методы

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
        /// Возвращает нормализованную версию данного вектора.
        /// </summary>
        /// <returns></returns>
        public Vector GetNormalized()
        {
            return new Vector(Coordinates.Select(val=>val/Length).ToArray());
        }

        /// <summary>
        /// Вычисляет скалярное произведение заданного вектора с текущим.
        /// </summary>
        /// <param name="Second"></param>
        /// <returns></returns>
        public double ScalarProduct(Vector Second)
        {
            return ScalarProduct(this, Second);
        }

        /// <summary>
        /// Вычисляет скалярное произведение двух векторов.
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static double ScalarProduct(Vector First, Vector Second)
        {
            if (First.CoordinatesLength!=Second.CoordinatesLength)
                throw new InvalidOperationException("Длины перемножаемых векторов не равны.");
            return First.Coordinates.Select((val,ind)=>val*Second[ind]).Sum();
        }

        /// <summary>
        ///     Возвращает матрицу, заданную по текущему вектору.
        /// </summary>
        /// <returns></returns>
        public Matrix ToMatrix()
        {
            return new Matrix(this);
        }


        #endregion

        #region Операторы

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

        public static bool operator ==(Vector First, Vector Second)
        {
            if (First.CoordinatesLength != Second.CoordinatesLength) return false;
            for(int i=0;i<First.CoordinatesLength;i++)
                if (First[i] != Second[i])
                    return false;
            return true;
        }

        public static bool operator !=(Vector First, Vector Second)
        {
            if (First.CoordinatesLength != Second.CoordinatesLength)
                return true;
            for (int i = 0; i < First.CoordinatesLength; i++)
                if (First[i] != Second[i])
                    return true;
            return false;
        }

        #endregion

        /// <summary>
        ///     Возвращает строковое представление вектора.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return
                $"({string.Join(", ", Coordinates.Select(coord => Math.Round(coord) == coord ? coord.ToString() : $"{coord:F3}"))})";
        }

        public override bool Equals(object obj)
        {
            return obj is Vector vector &&
                   EqualityComparer<double[]>.Default.Equals(Coordinates, vector.Coordinates);
        }

        public override int GetHashCode()
        {
            return -1484672504 + EqualityComparer<double[]>.Default.GetHashCode(Coordinates);
        }

        public bool Equals(Vector other)
        {
            return this == other;
        }
    }
}