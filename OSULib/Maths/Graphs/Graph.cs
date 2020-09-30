using System;
using System.Linq;

namespace OSULib.Maths.Graphs
{
    /// <summary>
    /// Представляет собой граф.
    /// </summary>
    public abstract class Graph
    {
        /// <summary>
        /// Матрица смежности этого графа.
        /// </summary>
        public Matrix AdjacencyMatrix { get; }

        /// <summary>
        /// Число вершин этого графа.
        /// </summary>
        public int VertexesCount => AdjacencyMatrix.LengthY;

        /// <summary>
        /// Инициализирует новый граф при помощи заданной матрицы смежности.
        /// </summary>
        /// <param name="adjacencyMatrix">Матрица смежности, при помощи которой будет инициализирован граф.</param>
        protected Graph(Matrix adjacencyMatrix)
        {
            if (!adjacencyMatrix.IsSquare)
                throw new InvalidOperationException("Матрица смежности не квадратная");
            AdjacencyMatrix = adjacencyMatrix;
        }

        /// <summary>
        /// Получает граф по матрице смежности.
        /// </summary>
        /// <param name="adjacencyMatrix">Матрица смежности, по которой нужно построить граф.</param>
        /// <returns>Граф нужного типа по матрице смежности.</returns>
        public static Graph GetGraph(Matrix adjacencyMatrix)
        {
            if (!adjacencyMatrix.IsSquare)
                throw new InvalidOperationException("Матрица смежности должна быть квадратной");

            for (int i = 0; i < adjacencyMatrix.LengthY; i++)
                for (int j = i + 1; j < adjacencyMatrix.LengthX; j++)
                    if (adjacencyMatrix[i, j] != adjacencyMatrix[j, i])
                        return new DirectedGraph(adjacencyMatrix);
            return new UndirectedGraph(adjacencyMatrix);
        }

        /// <summary>
        /// Получает число маршрутов (путей) в данном графе заданной длины, соединяющих две указанные вершины.
        /// </summary>
        /// <param name="firstVertexId">Номер первой вершины.</param>
        /// <param name="secondVertexId">Номер второй вершины.</param>
        /// <param name="length">Длина маршрута.</param>
        /// <returns>Число маршрутов (путей) в данном графе заданной длины, соединяющих две указанные вершины.</returns>
        public int GetNumberOfRoutesBetweenVertexes(int firstVertexId, int secondVertexId, uint length)
        {
            return (int)GetRouteMatrix(length)[firstVertexId, secondVertexId];
        }

        /// <summary>
        /// Возращает матрицу С маршрутов этого графа.
        /// </summary>
        /// <returns>Матрица С маршрутов этого графа.</returns>
        public Matrix GetCRouteMatrix()
        {
            Matrix result = Matrix.GetZeroMatrix(VertexesCount);
            for (uint i = 1; i < VertexesCount; i++)
                result += AdjacencyMatrix.Pow(i);
            return result;
        }

        /// <summary>
        /// Возращает матрицу С замкнутых маршрутов этого графа.
        /// </summary>
        /// <returns>Матрица С замкнутых маршрутов этого графа.</returns>
        public Matrix GetCRouteClosedMatrix()
        {
            Matrix result = Matrix.GetZeroMatrix(VertexesCount);
            for (uint i = 1; i <= VertexesCount; i++)
                result += AdjacencyMatrix.Pow(i);
            return result;
        }

        /// <summary>
        /// Возвращает матрицу для нахождения числа маршрутов между двумя вершинами указанной длины
        /// </summary>
        /// <returns></returns>
        public Matrix GetRouteMatrix(uint length)
        {
            return AdjacencyMatrix.Pow(length);
        }

        /// <summary>
        /// Определяет, является ли данный граф мультиграфом. (когда две вершины соединены несколькими дугами\ребрами
        /// </summary>
        public bool IsMultiGraph
        {

            get
            {
                for (int i = 0; i < VertexesCount; i++)
                {
                    for (int j = i; j < VertexesCount; j++)
                        if (AdjacencyMatrix[i, j] > 1 || AdjacencyMatrix[j, i] > 1)
                            return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Определяет, является ли граф псевдо-графом. (когда есть хотя бы одна петля)
        /// </summary>
        public bool IsPseudoGragh
        {
            get
            {
                for (int i = 0; i < VertexesCount; i++)
                    if (AdjacencyMatrix[i, i] != 0)
                        return true;
                return false;
            }
        }

        /// <summary>
        /// Определяет, является ли граф простым (не содержащим петель или кратных ребер).
        /// </summary>
        public bool IsSimpleGragh => !IsPseudoGragh && !IsMultiGraph;

        /// <summary>
        /// Получает полустепень захода вершины с заданным номером.
        /// </summary>
        /// <param name="vertexId">Номер вершины, для которой нужно получить полустепень захода.</param>
        /// <returns>Полустепень захода вершины с заданным номером.</returns>
        public int GetSemiApproach(int vertexId)
        {
            return (int)AdjacencyMatrix.GetColumn(vertexId).Sum();
        }

        /// <summary>
        /// Получает полустепень исхода вершины с заданным номером.
        /// </summary>
        /// <param name="vertexId">Номер вершины, для которой нужно получить полустепень исхода.</param>
        /// <returns>Полустепень исхода вершины с заданным номером.</returns>
        public int GetSemiExodus(int vertexId)
        {
            return (int)AdjacencyMatrix.GetRow(vertexId).Sum();
        }

        /// <summary>
        /// Матрица инцидентности данного графа.
        /// </summary>
        public abstract Matrix IncidenceMatrix { get; }

        /// <summary>
        /// Возвращает строковое представление названия типа этого графа.
        /// </summary>
        /// <returns>Строковое представление названия типа этого графа.</returns>
        public override string ToString()
        {
            return GraphTypeName;
        }

        /// <summary>
        /// Название типа этого графа.
        /// </summary>
        public abstract string GraphTypeName { get; }

        /// <summary>
        /// Выполняет операцию объединения графов и возвращает результат.
        /// </summary>
        /// <param name="other">Граф, с которым необходимо выполнить объединение.</param>
        /// <returns>Граф - результат</returns>
        public Graph Union(Graph other)
        {
            if (VertexesCount != other.VertexesCount)
                throw new InvalidOperationException("Объединение графов с разным набором вершин не поддерживается");
            if (IsMultiGraph || other.IsMultiGraph)
                throw new InvalidOperationException("Объединение мультиграфов не поддерживается");
            Matrix resultMatrix = new Matrix(VertexesCount);
            for (int i = 0; i < resultMatrix.LengthY; i++)
                for (int j = 0; j < resultMatrix.LengthY; j++)
                    resultMatrix[i, j] = Math.Max(AdjacencyMatrix[i, j], other.AdjacencyMatrix[i, j]);
            return GetGraph(resultMatrix);
        }

        /// <summary>
        /// Выполняет операцию пересечения графов и возвращает результат.
        /// </summary>
        /// <param name="other">Граф, с которым необходимо выполнить пересечение.</param>
        /// <returns>Граф - результат</returns>
        public Graph Intersection(Graph other)
        {
            if (VertexesCount != other.VertexesCount)
                throw new InvalidOperationException("Пересечение графов с разным набором вершин не поддерживается");
            if (IsMultiGraph || other.IsMultiGraph)
                throw new InvalidOperationException("Пересечение мультиграфов не поддерживается");
            Matrix resultMatrix = new Matrix(VertexesCount);
            for (int i = 0; i < resultMatrix.LengthY; i++)
                for (int j = 0; j < resultMatrix.LengthY; j++)
                    resultMatrix[i, j] = Math.Min(AdjacencyMatrix[i, j], other.AdjacencyMatrix[i, j]);
            return GetGraph(resultMatrix);
        }


    }

    public static class GraphOperations
    {

    }
}
