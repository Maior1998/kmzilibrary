using System;
using System.Collections.Generic;
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
            if (IsMultiGraph || other.IsMultiGraph)
                throw new InvalidOperationException("Объединение мультиграфов не поддерживается");

            return GetGraph(GetGraphUnionMatrix(this, other));
        }

        private static Matrix GetGraphUnionMatrix(Graph first, Graph second)
        {
            Matrix resultMatrix = null;
            if (first.VertexesCount > second.VertexesCount)
            {
                resultMatrix = first.AdjacencyMatrix.Copy();
                for (int i = 0; i < second.VertexesCount; i++)
                    for (int j = 0; j < second.VertexesCount; j++)
                        resultMatrix[i, j] = Math.Max(first.AdjacencyMatrix[i, j], second.AdjacencyMatrix[i, j]);
            }
            else
            {
                resultMatrix = second.AdjacencyMatrix.Copy();
                for (int i = 0; i < first.VertexesCount; i++)
                    for (int j = 0; j < first.VertexesCount; j++)
                        resultMatrix[i, j] = Math.Max(first.AdjacencyMatrix[i, j], second.AdjacencyMatrix[i, j]);
            }

            return resultMatrix;
        }

        /// <summary>
        /// Выполняет операцию пересечения графов и возвращает результат.
        /// </summary>
        /// <param name="other">Граф, с которым необходимо выполнить пересечение.</param>
        /// <returns>Граф - результат</returns>
        public Graph Intersection(Graph other)
        {
            if (IsMultiGraph || other.IsMultiGraph)
                throw new InvalidOperationException("Пересечение мультиграфов не поддерживается");
            Matrix resultMatrix = null;
            resultMatrix = VertexesCount < other.VertexesCount
                ? new Matrix(VertexesCount)
                : new Matrix(other.VertexesCount);
            for (int i = 0; i < resultMatrix.LengthY; i++)
                for (int j = 0; j < resultMatrix.LengthX; j++)
                    resultMatrix[i, j] = Math.Min(AdjacencyMatrix[i, j], other.AdjacencyMatrix[i, j]);
            return GetGraph(resultMatrix);
        }

        /// <summary>
        /// Выполняет кольцевую сумму графов и возвращает результат.
        /// </summary>
        /// <param name="other">Граф, с которым необходимо выполнить кольцевую сумму.</param>
        /// <returns>Граф - результат</returns>
        public Graph AnnularSum(Graph other)
        {
            if (IsMultiGraph || other.IsMultiGraph)
                throw new InvalidOperationException("Кольцевая сумма мультиграфов не поддерживается");
            Matrix resultMatrix = null;
            if (VertexesCount > other.VertexesCount)
            {
                resultMatrix = AdjacencyMatrix.Copy();
                for (int i = 0; i < other.VertexesCount; i++)
                    for (int j = 0; j < other.VertexesCount; j++)
                        resultMatrix[i, j] = (AdjacencyMatrix[i, j] + other.AdjacencyMatrix[i, j]) % 2;
            }
            else
            {
                resultMatrix = other.AdjacencyMatrix.Copy();
                for (int i = 0; i < VertexesCount; i++)
                    for (int j = 0; j < VertexesCount; j++)
                        resultMatrix[i, j] = (AdjacencyMatrix[i, j] + other.AdjacencyMatrix[i, j]) % 2;
            }
            return GetGraph(resultMatrix);
        }

        /// <summary>
        /// Выполняет соединение двух графов и возвращает результат.
        /// </summary>
        /// <param name="other">Граф, с которым необходимо выполнить соединение.</param>
        /// <returns>Граф - результат</returns>
        public Graph Join(Graph other)
        {
            //необходимо создать полный набор вершин, а самих их соединить ребрами
            if (IsMultiGraph || other.IsMultiGraph)
                throw new InvalidOperationException("Соединение мультиграфов не поддерживается");

            Matrix result = GetGraphUnionMatrix(this, other);
            for (int i = 0; i < VertexesCount; i++)
                for (int j = 0; j < other.VertexesCount; j++)
                {
                    if (i == j) continue;
                    result[i, j] = Math.Min(1, result[i, j] + 1);
                }
            return GetGraph(result);
        }

        /// <summary>
        /// Возвращает новый граф, в котором удалена заданная вершина.
        /// </summary>
        /// <param name="vertexId">Номер удаляемой вершины.</param>
        /// <returns>Новый граф, в котором удалена заданная вершина.</returns>
        public Graph RemoveVertex(int vertexId)
        {
            return GetGraph(AdjacencyMatrix.GetSubmatrix(vertexId, vertexId));
        }

        /// <summary>
        /// Возвращает новый граф, в котором удалена дуга или ребро, если таковые существуют.
        /// </summary>
        /// <param name="firstVertexId">Номер вершины, из которой выходит дуга (ребро)</param>
        /// <param name="secondVertexId">Номер вершины, в которую заходит дуга (ребро).</param>
        /// <returns>Новый граф, в котором удалена дуга или ребро, если таковые существуют.</returns>
        public abstract Graph RemoveEdge(int firstVertexId, int secondVertexId);

        /// <summary>
        /// Осуществляет операцию замыкания или отождествелния двух вершин.
        /// </summary>
        /// <param name="firstVertexId">Номер первой отождествляемой вершины.</param>
        /// <param name="secondVertexId">Номер второй отождествляемой вершины.</param>
        /// <returns>Новый граф, в котором осуществлена операция замыкания или отождествелния двух вершин.</returns>
        public Graph GetVertexesClosure(int firstVertexId, int secondVertexId)
        {
            if (IsMultiGraph)
                throw new InvalidOperationException("Замыкание мультиграфа не поддерживается");
            Matrix result = AdjacencyMatrix.Copy();

            for (int i = 0; i < VertexesCount; i++)
            {
                result[firstVertexId, i] = Math.Min(1, result[firstVertexId, i] + result[secondVertexId, i]);
                result[i, firstVertexId] = Math.Min(1, result[i, firstVertexId] + result[i, secondVertexId]);
            }

            result = result.GetSubmatrix(secondVertexId, secondVertexId);
            return GetGraph(result);
        }

        /// <summary>
        /// Получает декартового (прямое) произвдеение двух графов и возвращает результат.
        /// </summary>
        /// <param name="other">Граф. с котором необходимо выполнить декартово произведение.</param>
        /// <returns>Граф - результат декартового произвдения.</returns>
        public Graph GetCartesianProduct(Graph other)
        {
            return GetGraph(AdjacencyMatrix.KroneckerProduct(Matrix.GetUnitMatrix(other.VertexesCount)) +
                            Matrix.GetUnitMatrix(VertexesCount).KroneckerProduct(other.AdjacencyMatrix));
        }

        /// <summary>
        /// Осуществляет операцию стягивания двух вершин.
        /// </summary>
        /// <param name="firstVertexId">Номер первой стягивания вершины.</param>
        /// <param name="secondVertexId">Номер второй стягивания вершины.</param>
        /// <returns>Новый граф, в котором осуществлена операция стягивания двух вершин.</returns>
        public Graph GetVertexesContraction(int firstVertexId, int secondVertexId)
        {
            if (IsMultiGraph)
                throw new InvalidOperationException("Стягивание мультиграфа не поддерживается");
            Matrix result = AdjacencyMatrix.Copy();

            for (int i = 0; i < VertexesCount; i++)
            {
                if (i == secondVertexId) continue;
                result[firstVertexId, i] = Math.Min(1, result[firstVertexId, i] + result[secondVertexId, i]);
                result[i, firstVertexId] = Math.Min(1, result[i, firstVertexId] + result[i, secondVertexId]);
            }

            result = result.GetSubmatrix(secondVertexId, secondVertexId);
            return GetGraph(result);
        }

        private Matrix reachabilityMatrix;
        /// <summary>
        /// Матрица достижимости для орграфа и связности для неорграфа.
        /// </summary>
        public Matrix ReachabilityMatrix
        {
            get
            {
                if (reachabilityMatrix != null) return reachabilityMatrix;
                reachabilityMatrix = Matrix.GetUnitMatrix(VertexesCount);
                for (uint i = 1; i < VertexesCount; i++)
                    reachabilityMatrix += AdjacencyMatrix.Pow(i);
                for (int i = 0; i < VertexesCount; i++)
                    for (int j = 0; j < VertexesCount; j++)
                        reachabilityMatrix[i, j] = reachabilityMatrix[i, j] != 0 ? 1 : 0;
                return reachabilityMatrix;
            }
        }

        private Matrix tightlyCoupledMatrix;

        /// <summary>
        /// Матрица сильной связности.
        /// </summary>
        public Matrix TightlyCoupledMatrix => tightlyCoupledMatrix ??= ReachabilityMatrix.ElementMult(ReachabilityMatrix.TransposedCopy());

        private Matrix[] connectivityComponents;
        public Matrix[] ConnectivityComponents
        {
            get
            {
                if (connectivityComponents != null) return connectivityComponents;
                Matrix tightlyCoupledMatrix = TightlyCoupledMatrix;
                List<Matrix> result = new List<Matrix>();
                while (tightlyCoupledMatrix.LengthY != 0)
                {
                    List<int> connectivityIndexes = new List<int>();
                    for (int columnIndex = 0; columnIndex < tightlyCoupledMatrix.LengthX; columnIndex++)
                        if (tightlyCoupledMatrix[0, columnIndex] == 1)
                            connectivityIndexes.Add(columnIndex);
                    Matrix buffer = new Matrix(connectivityIndexes.Count);
                    for (int i = 0; i < connectivityIndexes.Count; i++)
                    for (int j = 0; j < connectivityIndexes.Count; j++)
                        buffer[i, j] = AdjacencyMatrix[connectivityIndexes[i], connectivityIndexes[j]];
                    result.Add(buffer);

                    for (int i = 0; i < connectivityIndexes.Count; i++)
                    {
                        tightlyCoupledMatrix = tightlyCoupledMatrix.RemoveRow(connectivityIndexes[i] - i);
                        tightlyCoupledMatrix = tightlyCoupledMatrix.RemoveColumn(connectivityIndexes[i] - i);
                    }


                }
                return connectivityComponents = result.ToArray();
            }
        }

    }

    public static class GraphOperations
    {

    }
}
