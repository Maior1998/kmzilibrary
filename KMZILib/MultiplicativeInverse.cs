using System;
using System.Collections;
using System.Linq;
using System.Numerics;
using LinearComparison=KMZILib.Comparison.LinearComparison;

namespace KMZILib
{
    /// <summary>
    ///     Представляет статический класс для вычисления мультипликативного обратного элемента.
    /// </summary>
    public static class MultiplicativeInverse
    {
        /// <summary>
        ///     Метод, выполняющий нахождение мультипликативного обратного, если последний существует, с помощью РАЕ.  x ≡ b * a^-1
        ///     (mod m)
        /// </summary>
        /// <returns>Сущесвует ли мультипликативный обратный?</returns>
        public static bool TrySolveByGCD(BigInteger a, BigInteger m, out LinearComparison Result, BigInteger[] GCD = null)
        {
            Result = null;
            if (GCD == null) GCD = AdvancedEuclidsalgorithm.GCD(a, m);
            if (GCD == null||GCD[0] != 1) return false;
            Result = new LinearComparison(GCD[1], m);
            return true;
        }

        /// <summary>
        ///     Метод, выполняющий нахождение мультипликативного обратного, если последний существует, с помощью РАЕ.  x ≡ b * a^-1
        ///     (mod m)
        /// </summary>
        /// <returns>Целое число - мультипликативный обратный</returns>
        /// <exception cref="InvalidOperationException">Решений нет</exception>
        public static BigInteger Solve(BigInteger a, BigInteger m)
        {
            if (!TrySolveByGCD(a, m, out LinearComparison Result))
                throw new InvalidOperationException();
            return Result.A;
        }

        /// <summary>
        ///     Нахождение мультипликативного обратного для элемента сравнения
        /// </summary>
        /// <param name="comparison">Сравнение, в котором необходимо найти мультипликативный обратный к остатку</param>
        /// <returns></returns>
        public static BigInteger Solve(LinearComparison comparison)
        {
            return Solve(comparison.A, comparison.M);
        }

        /// <summary>
        ///     Внутренний метод, осуществляющий проверку, я вляется ли число простым.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        private static bool IsPrimeNumber(int a)
        {
            if (a < 0) a *= -1;
            if (a <= 1) return false;
            for (int i = 2; i < (int) Math.Sqrt(a) + 1; i++)
                if (a % i == 0)
                    return false;
            return true;
        }

        /// <summary>
        ///     Метод, выполняющий нахождение мультипликативного обратного, если последний существует, с помощью Бинарного метода.
        ///     x ≡ b * a^-1 (mod m)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="m"></param>
        /// <param name="Result"></param>
        /// <param name="GCD"></param>
        /// <returns></returns>
        public static bool TrySolveByBinaryMethod(BigInteger a, BigInteger m, out LinearComparison Result,
            BigInteger[] GCD = null)
        {
            Result = null;
            if (AdvancedEuclidsalgorithm.GCDResult(a, m) != 1) return false;
            Result = new LinearComparison(a, m);
            int degree = Comparison.EulersFunction((int)m) - 1;
            BitArray sma = new BitArray(new[] {degree});
            int i = sma.Length - 1;
            while (!sma[i]) i--;
            for (i--; i >= 0; i--)
                if (sma[i])
                {
                    Result.A *= Result.A;
                    Result.A *= a;
                }
                else
                {
                    Result.A *= Result.A;
                }

            return true;
        }

        /// <summary>
        ///     Осуществляет возведение остатка сравнения в указанную степень бинарным способом
        /// </summary>
        /// <param name="source"></param>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static LinearComparison BinaryPowLinearComparison(LinearComparison source, int degree)
        {
            BigInteger a = 5;
            LinearComparison Result = new LinearComparison(source.A, source.M);
            bool[] sma = BinaryFunction.GetBinaryArray(degree).Skip(1).ToArray();
            foreach (bool bit in sma)
            {
                Result.A *= Result.A;
                if (bit)
                    Result.A *= source.A;
            }

            return Result;
        }
    }
}