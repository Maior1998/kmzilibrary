using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSULib.Maths.Graphs
{
    public class WeightedGraph : Graph
    {
        public WeightedGraph(Matrix adjacencyMatrix) : base(adjacencyMatrix)
        {
            for(int i=0;i<VertexesCount;i++)
                for(int j=0;j<VertexesCount;j++)
                    if (AdjacencyMatrix[i, j] == -1)
                        AdjacencyMatrix[i, j] = double.PositiveInfinity;
        }

        public override Matrix IncidenceMatrix => throw new NotImplementedException();
        public override string GraphTypeName => "Взвешенный граф";
        public override Graph RemoveEdge(int firstVertexId, int secondVertexId)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Получает матрицу кротчайших расстояний по алгоритму Флойда-Уоршелла.
        /// </summary>
        /// <returns>Матрица кротчайших расстояний по алгоритму Флойда-Уоршелла.</returns>
        public Matrix GetFloydWarshallShortestPath()
        {
            Matrix current = AdjacencyMatrix.Copy();

            for (int aloghorithmStepsCount = 0; aloghorithmStepsCount < VertexesCount; aloghorithmStepsCount++)
            {
                Matrix previous = current.Copy();
                for (int i = 0; i < VertexesCount; i++)
                {
                    for (int j = i + 1; j < VertexesCount; j++)
                    {
                        current[i, j] = Math.Min(previous[i, j], previous[i, aloghorithmStepsCount] + previous[aloghorithmStepsCount, j]);
                        current[j, i] = Math.Min(previous[j, i], previous[aloghorithmStepsCount, i] + previous[j, aloghorithmStepsCount]);
                    }
                }
            }

            return current;
        }
    }
}
