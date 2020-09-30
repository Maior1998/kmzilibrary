using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static OSULib.Maths.Comparison;

namespace OSULib.Maths
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
            BigInteger M = Comparisons.Select(comp => comp.M)
                .Aggregate<BigInteger, BigInteger>(1, (Current, line) => Current * line);
            BigInteger[] Mi = Comparisons.Select(Line => M / Line.M).ToArray();

            return new Comparison.LinearComparison(
                Comparisons.Select((comp, miindex) =>
                        comp.A * Mi[miindex] * Comparison.MultiplicativeInverse.Solve(Mi[miindex], comp.M))
                    .Aggregate<BigInteger, BigInteger>(0, (Current, line) => Current + line), M);

        }


    }
}