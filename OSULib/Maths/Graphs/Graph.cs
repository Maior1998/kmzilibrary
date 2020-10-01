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

        /// <summary>
        /// Выполняет кольцевую сумму графов и возвращает результат.
        /// </summary>
        /// <param name="other">Граф, с которым необходимо выполнить кольцевую сумму.</param>
        /// <returns>Граф - результат</returns>
        public Graph AnnularSum(Graph other)
        {
            if (VertexesCount != other.VertexesCount)
                throw new InvalidOperationException("Кольцевая сумма графов с разным набором вершин не поддерживается");
            if (IsMultiGraph || other.IsMultiGraph)
                throw new InvalidOperationException("Кольцевая сумма мультиграфов не поддерживается");
            Matrix resultMatrix = new Matrix(VertexesCount);
            for (int i = 0; i < resultMatrix.LengthY; i++)
                for (int j = 0; j < resultMatrix.LengthY; j++)
                    resultMatrix[i, j] = (AdjacencyMatrix[i, j] + other.AdjacencyMatrix[i, j]) % 2;
            return GetGraph(resultMatrix);
        }

        /// <summary>
        /// Выполняет соединение двух графов и возвращает результат.
        /// </summary>
        /// <param name="other">Граф, с которым необходимо выполнить соединение.</param>
        /// <returns>Граф - результат</returns>
        public Graph Join(Graph other)
        {
            //TODO: доделать
            //необходимо создать полный набор вершин, а самих их соединить ребрами
            if (IsMultiGraph || other.IsMultiGraph)
                throw new InvalidOperationException("Соединение мультиграфов не поддерживается");

            Matrix result = new Matrix(VertexesCount + other.VertexesCount);

            for (int i = 0; i < VertexesCount; i++)
                for (int j = 0; j < VertexesCount; j++)
                    result[i, j] = AdjacencyMatrix[i, j];

            for (int i = VertexesCount; i < result.LengthY; i++)
            {
                for (int j = 0; j < VertexesCount; j++)
                    result[i, j] = result[j, i] = 1;
                for (int j = VertexesCount; j < result.LengthY; j++)
                {
                    result[i, j] = other.AdjacencyMatrix[i, j];
                    result[j, i] = other.AdjacencyMatrix[j, i];
                }
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
            return Graph.GetGraph(AdjacencyMatrix.GetSubmatrix(vertexId, vertexId));
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
                if(i == secondVertexId) continue;
                result[firstVertexId, i] = Math.Min(1, result[firstVertexId, i] + result[secondVertexId, i]);
                result[i, firstVertexId] = Math.Min(1, result[i, firstVertexId] + result[i, secondVertexId]);
            }

            result = result.GetSubmatrix(secondVertexId, secondVertexId);
            return GetGraph(result);
        }

    }

    public static class GraphOperations
    {

    }
}
