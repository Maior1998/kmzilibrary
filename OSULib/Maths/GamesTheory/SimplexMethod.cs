using System;
using Microsoft.SolverFoundation.Services;

namespace OSULib.Maths.GamesTheory
{
    public class SimplexMethod
    {
        public SimplexMethod()
        {
            _solver = SolverContext.GetContext();
            _model = _solver.CreateModel();
        }

        public SimplexMethod(double[,] table) : this()
        {
            M = table.GetLength(0);
            N = table.GetLength(1);
            var decisionsA = new Decision[M];
            ResultsA = new double[M];
            for (int i = 0; i < M; i++)
            {
                decisionsA[i] = new Decision(Domain.RealNonnegative, $"A{i}");
            }
            _model.AddDecisions(decisionsA);
            var decisionsB = new Decision[N];
            ResultsB = new double[N];
            for (int j = 0; j < N; j++)
            {
                decisionsB[j] = new Decision(Domain.RealNonnegative, $"B{j}");
            }
            _model.AddDecisions(decisionsB);
            for (int j = 0; j < N; j++)
            {
                var sum = new SumTermBuilder(M);
                for (int i = 0; i < M; i++)
                {
                    sum.Add(decisionsA[i] * table[i, j]);
                }
                _model.AddConstraint($"C_A{j}", sum.ToTerm() >= 1);
            }
            for (int i = 0; i < M; i++)
            {
                var sum = new SumTermBuilder(N);
                for (int j = 0; j < N; j++)
                {
                    sum.Add(decisionsB[j] * table[i, j]);
                }
                _model.AddConstraint($"C_B{i}", sum.ToTerm() <= 1);
            }
            var summinA = new SumTermBuilder(N);
            for (int i = 0; i < M; i++)
            {
                summinA.Add(decisionsA[i]);
            }
            _model.AddGoal("resA", GoalKind.Minimize, summinA.ToTerm());
            var summinB = new SumTermBuilder(N);
            for (int j = 0; j < N; j++)
            {
                summinB.Add(decisionsB[j]);
            }
            _model.AddGoal("resB", GoalKind.Maximize, summinB.ToTerm());
            _solver.Solve();
            var gpsumA = 0.0;
            for (int i = 0; i < M; i++)
            {
                ResultsA[i] = decisionsA[i].GetDouble();
                gpsumA += ResultsA[i];
            }
            GamePriceA = 1 / gpsumA;
            var gpsumB = 0.0;
            for (int j = 0; j < N; j++)
            {
                ResultsB[j] = decisionsB[j].GetDouble();
                gpsumB += ResultsB[j];
            }
            GamePriceB = 1 / gpsumB;
            _solver.ClearModel();
        }

        public double[] ResultsA;
        public double[] ResultsB;

        public int M { get; set; }
        public int N { get; set; }

        public double GamePriceA { get; set; }
        public double GamePriceB { get; set; }

        private readonly SolverContext _solver;
        private readonly Model _model;
    }
}
