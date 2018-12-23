using System;
using System.Collections.Generic;
using System.Text;

namespace KMZILib
{
    public partial class BinaryFunction
    {
        /// <summary>
        ///     Представляет класс для работы с Бинарными отношениями.
        /// </summary>
        public class BinaryRelationMatrix
        {
            /// <summary>
            ///     Матрица бинарного отношения в виде массива массивов <see cref="bool" />
            /// </summary>
            private readonly bool[][] matrix;

            /// <summary>
            ///     Конструктор Бинарного отношения, принимающий на вход матрицу в виде массив <see cref="bool" />
            /// </summary>
            /// <param name="Source"></param>
            public BinaryRelationMatrix(bool[,] Source)
            {
                if (Source == null)
                    throw new NullReferenceException("Исходный массив должен быть проинициализирован!");

                matrix = new bool[Source.GetLength(0)][];
                for (int i = 0; i < Source.GetLength(0); i++)
                {
                    matrix[i] = new bool[Source.GetLength(1)];
                    for (int j = 0; j < Source.GetLength(1); j++)
                        matrix[i][j] = Source[i, j];
                }
            }

            /// <summary>
            ///     Конструктор Бинарного отношения, принимающий на вход матрицу в виде массив <see cref="byte" />
            /// </summary>
            /// <param name="Source"></param>
            public BinaryRelationMatrix(byte[,] Source) //инициализация массивом
            {
                if (Source == null)
                    throw new NullReferenceException("Исходный массив должен быть проинициализирован!");

                matrix = new bool[Source.GetLength(0)][];
                for (int i = 0; i < Source.GetLength(0); i++)
                {
                    matrix[i] = new bool[Source.GetLength(1)];
                    for (int j = 0; j < Source.GetLength(1); j++)
                        matrix[i][j] = Source[i, j] == 1;
                }
            }

            /// <summary>
            ///     Определяет, является ли отношение рефлексивным
            /// </summary>
            public bool IsReflective
            {
                get
                {
                    for (int i = 0; i < Size; i++)
                        if (!this[i, i])
                            return false;
                    return true;
                }
            }

            /// <summary>
            ///     Определяет, является ли отношение антирефлексивным
            /// </summary>
            public bool IsAntireflective
            {
                get
                {
                    for (int i = 0; i < Size; i++)
                        if (this[i, i])
                            return false;
                    return true;
                }
            }

            /// <summary>
            ///     Определяет, является ли отношение симметричным
            /// </summary>
            public bool IsSymmetrical
            {
                get
                {
                    for (int i = 0; i < Size; i++)
                    for (int j = 0; j < Size; j++)
                        if (this[i, j] != this[j, i])
                            return false;
                    return true;
                }
            }

            /// <summary>
            ///     Определяет, является ли отношение антисимметричным
            /// </summary>
            public bool IsAntisymmetrical
            {
                get
                {
                    for (int i = 0; i < Size; i++)
                    for (int j = 0; j < Size; j++)
                        if (j != i && this[i, j] && this[j, i])
                            return false;
                    return true;
                }
            }

            /// <summary>
            ///     Определяет, является ли отношение транзитивным
            /// </summary>
            public bool IsTransitively
            {
                get
                {
                    BinaryRelationMatrix MultipplyResult = Multiply(this, this);
                    for (int i = 0; i < matrix.Length; i++)
                    for (int j = 0; j < matrix[0].Length; j++)
                        if (MultipplyResult[i, j] && !matrix[i][j])
                            return false;
                    return true;
                }
            }

            /// <summary>
            ///     Определяет, является ли отношение отношение эквивалентности
            /// </summary>
            public bool IsEquivalence => IsReflective && IsSymmetrical && IsTransitively;

            /// <summary>
            ///     Определяет, является ли отношение отношение нестрогого порядка
            /// </summary>
            public bool IsNonStrictRatio => IsReflective && IsAntisymmetrical && IsTransitively;

            /// <summary>
            ///     Определяет, является ли отношение отношение строгого порядка
            /// </summary>
            public bool IsStrictRatio => IsAntireflective && IsAntisymmetrical && IsTransitively;

            /// <summary>
            ///     Матрица обратного отношения. Получается транспонированием.
            /// </summary>
            public BinaryRelationMatrix Reverse
            {
                get
                {
                    bool[,] answer = new bool[Size, Size];
                    for (int i = 0; i < Size; i++)
                    for (int j = 0; j < Size; j++)
                        answer[i, j] = this[j, i];
                    return new BinaryRelationMatrix(answer);
                }
            }

            /// <summary>
            ///     Возвращает ранг матрицы
            /// </summary>
            public byte Size => (byte) matrix.GetLength(0);

            /// <summary>
            ///     Получает доступ к матрице бинарного отношения
            /// </summary>
            /// <param name="index1">Индекс вертикального измерения</param>
            /// <param name="index2">Индекс горизонтального измерения</param>
            /// <returns>Значение в матрице бинарного отношения. Равно 0 или 1</returns>
            public bool this[int index1, int index2]
            {
                get => matrix[index1][index2];
                set => matrix[index1][index2] = value;
            }

            /// <summary>
            ///     Опеределяет, совпадают ли значения двух матриц <see cref="BinaryRelationMatrix" />
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            protected bool Equals(BinaryRelationMatrix other)
            {
                return Equals(matrix, other.matrix);
            }

            /// <summary>
            ///     Опеределяет, являются ли два объекта <see cref="object" /> матрицами <see cref="BinaryRelationMatrix" />,
            ///     совпадающими по значению
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                if (obj is null) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == GetType() && Equals((BinaryRelationMatrix) obj);
            }

            /// <summary>
            ///     Возвращает хэш-код данной матрицы
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return matrix != null ? matrix.GetHashCode() : 0;
            }

            /// <summary>
            ///     Возвращает строковое представление матрицы бинарного отношения
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                List<string> answer = new List<string>();
                foreach (bool[] bools in matrix)
                {
                    List<byte> buffer = new List<byte>();
                    foreach (bool boolelement in bools)
                        buffer.Add((byte) (boolelement ? 1 : 0));

                    answer.Add(string.Join("  ", buffer));
                }

                return string.Join("\n", answer);
            }

            /// <summary>
            ///     Осуществляет поэлементное умножение элементов матриц.
            /// </summary>
            /// <param name="A">Матрица-множитель</param>
            /// <param name="B">Матрица-множитель</param>
            /// <returns>Результат поэлементного умножения матриц</returns>
            public static BinaryRelationMatrix operator *(BinaryRelationMatrix A, BinaryRelationMatrix B)
            {
                byte Length = A.Size;
                bool[,] answer = new bool[Length, Length];
                for (int i = 0; i < Length; i++)
                for (int j = 0; j < Length; j++)
                    answer[i, j] = A[i, j] && B[i, j];
                return new BinaryRelationMatrix(answer);
            }

            /// <summary>
            ///     Сравнение матриц бинарных отношений
            /// </summary>
            /// <param name="A">Сравниваемая матрица</param>
            /// <param name="B">Сравниваемая матрица</param>
            /// <returns>true - Матрицы совпадают. false - отличаются.</returns>
            public static bool operator ==(BinaryRelationMatrix A, BinaryRelationMatrix B)
            {
                for (int i = 0; i < A.Size; i++)
                for (int j = 0; j < B.Size; j++)
                    if (A[i, j] != B[i, j])
                        return false;
                return true;
            }

            /// <summary>
            ///     Сравнение матриц бинарных отношений
            /// </summary>
            /// <param name="A">Сравниваемая матрица</param>
            /// <param name="B">Сравниваемая матрица</param>
            /// <returns>true - Матрицы отличаются. false - совпадают.</returns>
            public static bool operator !=(BinaryRelationMatrix A, BinaryRelationMatrix B)
            {
                for (int i = 0; i < A.Size; i++)
                for (int j = 0; j < B.Size; j++)
                    if (A[i, j] != B[i, j])
                        return true;
                return false;
            }

            /// <summary>
            ///     Осуществляет объединение двух матриц
            /// </summary>
            /// <param name="A">Матрица-слагаемое</param>
            /// <param name="B">Матрица-слагаемое</param>
            /// <returns>Результат объединения двух матриц</returns>
            public static BinaryRelationMatrix operator +(BinaryRelationMatrix A, BinaryRelationMatrix B)
            {
                byte Length = A.Size;
                bool[,] answer = new bool[Length, Length];
                for (int i = 0; i < Length; i++)
                for (int j = 0; j < Length; j++)
                    answer[i, j] = A[i, j] || B[i, j];

                return new BinaryRelationMatrix(answer);
            }

            /// <summary>
            ///     Разность двух матриц
            /// </summary>
            /// <param name="A"></param>
            /// <param name="B"></param>
            /// <returns></returns>
            public static BinaryRelationMatrix operator -(BinaryRelationMatrix A, BinaryRelationMatrix B)
            {
                byte Length = A.Size;
                bool[,] answer = new bool[Length, Length];
                for (int i = 0; i < Length; i++)
                for (int j = 0; j < Length; j++)
                    answer[i, j] = A[i, j] && !B[i, j];
                return new BinaryRelationMatrix(answer);
            }

            /// <summary>
            ///     Осуществляет алгебраическое произведение двух матриц
            /// </summary>
            /// <param name="First"></param>
            /// <param name="Second"></param>
            /// <returns></returns>
            public static BinaryRelationMatrix Multiply(BinaryRelationMatrix First, BinaryRelationMatrix Second)
            {
                byte Length = First.Size;
                bool[,] answer = new bool[Length, Length];
                for (int i = 0; i < Length; i++)
                for (int j = 0; j < Length; j++)
                for (int k = 0; k < Length && !answer[i, j]; k++)
                    answer[i, j] = First[i, k] && Second[k, j];

                return new BinaryRelationMatrix(answer);
            }

            /// <summary>
            ///     Осуществляет анализ матрицы бинарного отношения и возвращает краткий отчет.
            /// </summary>
            /// <returns></returns>
            public string Analize()
            {
                StringBuilder answer = new StringBuilder();

                if (IsReflective)
                    answer.Append("Рефлексивно");
                else if (IsAntireflective)
                    answer.Append("Антирефлексивно");
                else answer.Append("Не рефлексивно, не антирефлексивно");

                if (IsSymmetrical)
                    answer.Append(", симметрично");
                else if (IsAntisymmetrical)
                    answer.Append(", антисимметрично");
                else answer.Append(", не симметрично, не антисимметрично");

                answer.Append(IsTransitively ? ", транзитивно. " : ", не транзитивно. ");

                if (IsEquivalence)
                    answer.Append("Это отношение эквивалентности.");
                else if (IsNonStrictRatio)
                    answer.Append("Это отношение нестрогого порядка.");
                else if (IsStrictRatio)
                    answer.Append("Это отношение строгого порядка.");

                return answer.ToString();
            }
        }
    }
}