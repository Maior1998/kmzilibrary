using System.Collections.Generic;
using System.Numerics;

namespace KMZILib
{
    /// <summary>
    ///     Chinese Remainder Theorem - Китайская теорема об остатках
    /// </summary>
    public static class CRT
    {
        /// <summary>
        ///     Осуществляет решение системы сравнений со взаимнопростыми модулями.
        /// </summary>
        /// <param name="Comparisons"></param>
        /// <returns></returns>
        public static Comparison.LinearComparison Solve(IEnumerable<Comparison.LinearComparison> Comparisons)
        {
            ExpressByPair buffer = new ExpressByPair(0, 1);
            foreach (Comparison.LinearComparison CurrentComparison in Comparisons)
            {
                int solution = 0;
                for (;
                    solution < CurrentComparison.M &&
                    (solution * buffer.Second + buffer.First) % CurrentComparison.M != CurrentComparison.A;
                    solution++) ;
                buffer.First += buffer.Second * solution;
                buffer.Second *= CurrentComparison.M;
            }

            return new Comparison.LinearComparison((int) buffer.First, (int) buffer.Second);
        }

        /// <summary>
        ///     Осуществляет решение системы сравнений со взаимнопростыми модулями.
        /// </summary>
        /// <param name="Comparisons"></param>
        /// <returns></returns>
        public static BigInteger SolveBigInteger(IEnumerable<Comparison.LinearComparison> Comparisons)
        {
            ExpressByPair buffer = new ExpressByPair(0, 1);
            foreach (Comparison.LinearComparison CurrentComparison in Comparisons)
            {
                int solution = 0;
                for (;
                    solution < CurrentComparison.M &&
                    (solution * buffer.Second + buffer.First) % CurrentComparison.M != CurrentComparison.A;
                    solution++)
                {
                }

                buffer.First += buffer.Second * solution;
                buffer.Second *= CurrentComparison.M;
            }

            return (int) buffer.First;
        }

        internal class ExpressByPair
        {
            internal BigInteger First;
            internal BigInteger Second;

            internal ExpressByPair(int first, int second)
            {
                First = first;
                Second = second;
            }

            public override string ToString()
            {
                return $"{First} + {Second}t";
            }
        }
    }
}