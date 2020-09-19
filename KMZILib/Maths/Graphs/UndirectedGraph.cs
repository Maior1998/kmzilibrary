using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMZILib.Maths.Graphs
{
    public class UndirectedGraph : Graph
    {
        /// <summary>
        /// Получает степень вершины с заданным номером.
        /// </summary>
        /// <param name="vertexId">Номер вершины.</param>
        /// <returns>Степень вершины с заданным номером.</returns>
        public int GetVertexDegree(int vertexId)
        {
            return (int)AdjacencyMatrix[vertexId].Sum();
        }

        public UndirectedGraph(Matrix adjacencyMatrix) : base(adjacencyMatrix)
        {
        }

        public override Matrix IncidenceMatrix
        {
            get
            {
                List<int>[] result = Enumerable.Range(0, VertexesCount).Select(x => new List<int>()).ToArray();
                for (int i = 0; i < VertexesCount; i++)
                    for (int j = i; j < VertexesCount; j++)
                    {
                        if (AdjacencyMatrix[i, j] == 0) continue;
                        if (i == j)
                        {
                            for (int k = 0; k < VertexesCount; k++)
                                result[i].Add(0);
                            result[i][result[i].Count - 1] = (int)AdjacencyMatrix[i, j] * 2;
                        }
                        else
                        {

                            for (int edgesCount = 0; edgesCount < AdjacencyMatrix[i, j]; edgesCount++)
                            {
                                for (int k = 0; k < VertexesCount; k++)
                                    result[i].Add(0);
                                result[i][result[i].Count - 1] = 1;
                                result[j][result[j].Count - 1] = 1;
                            }
                        }

                    }
                return new Matrix(result.Select(x => x.ToArray()).ToArray());
            }
        }

        public override string GraphTypeName => "Неориентированный граф";
    }
}
