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


        public string GetBellmanFordShortestPath()
        {
            double[,] Fb = new double[VertexesCount, VertexesCount];
            Matrix mcon = AdjacencyMatrix.Copy();
            for (int i = 1; i < Fb.GetLength(0); i++)
            {
                Fb[i, 0] = int.MaxValue / 2 - 1;
                mcon[i, i] = int.MaxValue / 2 - 1;
            }
            List<int>[] vs = new List<int>[VertexesCount];
            for (int i = 0; i < vs.Length; i++)
            {
                vs[i] = new List<int>();
            }
            for (int i = 1; i < Fb.GetLength(1); i++)
            {
                for (int j = 1; j < Fb.GetLength(0); j++)
                {
                    double[] sum = new double[Fb.GetLength(0)];
                    for (int k = 0; k < Fb.GetLength(0); k++)
                    {
                        if (mcon[k, j] >= int.MaxValue / 2 - 1 || Fb[k, i - 1] >= int.MaxValue / 2 - 1)
                            sum[k] = int.MaxValue / 2 - 1;
                        else
                            sum[k] = mcon[k, j] + Fb[k, i - 1];
                    }
                    int v;
                    Fb[j, i] = MinEl(sum, out v);
                    if (i > 1)
                    {
                        if (v != j)
                        {
                            for (int k = 1; k < Fb.GetLength(0); k++)
                            {
                                if (v == k)
                                {
                                    int[] temp = new int[vs[k].Count];
                                    vs[k].CopyTo(temp);
                                    vs[j] = new List<int>();
                                    for (int q = 0; q < temp.Length; q++)
                                    {
                                        vs[j].Add(temp[q]);
                                    }
                                    break;
                                }
                            }
                        }
                        if (vs[j][vs[j].Count - 1] == v)
                            vs[j].Add(j);
                        else if (vs[j][vs[j].Count - 1] != j)
                            vs[j].Add(v);
                    }
                    else
                        vs[j].Add(v);
                }
            }
            string way = "";
            for (int i = 1; i < vs.Length; i++)
            {
                way += "v" + vs[i][0];
                for (int j = 1; j < vs[i].Count; j++)
                {
                    way += "--v" + vs[i][j];
                }
                way += "\n";
            }
            return way;
        }

        private double MinEl(double[] mas, out int num)
        {
            double min = mas[0];
            num = 0;
            for (int i = 1; i < mas.Length; i++)
            {
                if (min > mas[i])
                {
                    min = mas[i];
                    num = i;
                }
            }
            return min;
        }

        /// <summary>
        /// Получает матрицу кротчайших расстояний по алгоритму Флойда-Уоршелла.
        /// </summary>
        /// <returns>Матрица кротчайших расстояний по алгоритму Флойда-Уоршелла.</returns>
        public Matrix GetFloydWarshallShortestPath()
        {
            Matrix current = AdjacencyMatrix.Copy();
            //Обнуляем главную диагональ, чтобы убрать петли, так как алгоритм Флойда-Уоршалла применим только для таких графов.
            for (int k = 0; k < current.LengthY; k++)
                current[k, k] = 0;
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
