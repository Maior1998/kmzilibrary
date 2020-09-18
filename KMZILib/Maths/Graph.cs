using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMZILib.Maths
{
    /// <summary>
    /// Представляет собой граф.
    /// </summary>
    public class Graph
    {
        private readonly Matrix matrix;
        /// <summary>
        /// Число вершин этого графа.
        /// </summary>
        public int VertexesCount { get; private set; }
        /// <summary>
        /// Инициализирует новый граф при помощи заданной матрицы смежности.
        /// </summary>
        /// <param name="matrix">Матрица смежности, при помощи которой будет инициализирован граф.</param>
        public Graph(Matrix matrix)
        {
            if (!matrix.IsSquare)
                throw new InvalidOperationException("Матрица смежности не квадратная");
            this.matrix = matrix;
            VertexesCount = this.matrix.LengthX;
        }

        /// <summary>
        /// Представляет тип графа по направленности его ребер или дуг.
        /// </summary>
        public enum GraphType
        {
            /// <summary>
            /// Ориентированный
            /// </summary>
            [Description("Ориентированный")]
            Directed,
            /// <summary>
            /// Неориентированный
            /// </summary>
            [Description("Неориентированный")]
            Undirected,
            /// <summary>
            /// Смешанный
            /// </summary>
            [Description("Смешанный")]
            Mixed
        }

        /// <summary>
        /// Тип данного графа.
        /// </summary>
        public GraphType Type
        {
            get
            {
                //определяет, является ли граф ориентированным
                bool IsSymmetric = true;
                //определяет, является ли граф неориентированным
                bool HasSymetric = false;

                for (int i = 0; i < VertexesCount; i++)
                {
                    for (int j = 0; j < VertexesCount; j++)
                    {
                        if (i == j)
                        {
                            if (matrix[i, j] == 0) continue;
                            if (matrix[i, j] >= 2)
                                HasSymetric = true;
                            else
                                IsSymmetric = false;
                        }
                        else
                        {
                            if (matrix[j, i] != matrix[i, j])
                                IsSymmetric = false;
                            else
                                HasSymetric = true;
                        }
                    }
                }

                if (!IsSymmetric && HasSymetric)
                    return GraphType.Mixed;
                if (IsSymmetric)
                    return GraphType.Undirected;
                return GraphType.Directed;
            }
        }
    }
}
