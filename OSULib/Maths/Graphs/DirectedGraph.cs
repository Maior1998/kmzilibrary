﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSULib.Maths.Graphs
{
    public class DirectedGraph : Graph
    {
        public DirectedGraph(Matrix adjacencyMatrix) : base(adjacencyMatrix)
        {
        }


        public override Matrix IncidenceMatrix
        {
            get
            {
                List<int>[] result = Enumerable.Range(0, VertexesCount).Select(x => new List<int>()).ToArray();
                for (int i = 0; i < VertexesCount; i++)
                for (int j = 0; j < VertexesCount; j++)
                {
                    if (AdjacencyMatrix[i, j] == 0) continue;
                    if (i == j)
                    {
                        for (int k = 0; k < VertexesCount; k++)
                            result[k].Add(0);
                        result[i][result[i].Count - 1] = (int)AdjacencyMatrix[i, j] * 2;
                    }
                    else
                    {

                        for (int edgesCount = 0; edgesCount < AdjacencyMatrix[i, j]; edgesCount++)
                        {
                            for (int k = 0; k < VertexesCount; k++)
                                result[k].Add(0);
                            result[i][result[i].Count - 1] = -1;
                            result[j][result[j].Count - 1] = 1;
                        }
                    }

                }
                return new Matrix(result.Select(x => x.ToArray()).ToArray());
            }
        }

        public override string GraphTypeName => "Ориентированный граф";

        public override Graph RemoveEdge(int firstVertexId, int secondVertexId)
        {
            Matrix result = AdjacencyMatrix.Copy();
            result[firstVertexId, secondVertexId] = 0;
            return GetGraph(result);
        }
    }
}
