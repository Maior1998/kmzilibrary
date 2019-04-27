using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static KMZILib.Comparison;

namespace KMZILib
{
    /// <summary>
    /// Статический класс для работы с простыми числами.
    /// </summary>
    public static class Primes {
        /// <summary>
        /// Статический класс, предоставляющий методы генерации простых чисел.
        /// </summary>
        public static class Factory
        {
            private static List<BigInteger> GeneratedNums = new List<BigInteger>();

            /// <summary>
            /// Возвращает следующее простое число.
            /// </summary>
            /// <returns></returns>
            public static BigInteger GetNext()
            {
                BigInteger[] CurrentSet = new BigInteger[Misc.Data.PrimeNumbers.Length + GeneratedNums.Count];
                Array.Copy(Misc.Data.PrimeNumbers.Select(num => (BigInteger) num).ToArray(), CurrentSet,
                    Misc.Data.PrimeNumbers.Length);
                Array.Copy(GeneratedNums.ToArray(),0, CurrentSet,Misc.Data.PrimeNumbers.Length, GeneratedNums.Count);
                while (true)
                {
                    BigInteger ElementF = CurrentSet.Aggregate<BigInteger, BigInteger>(1, (Current, KnownPrime) => Current * BigInteger.Pow(KnownPrime, RD.Rand.Next(1, 11)));
                }

                return 0;
            }
        }
    /// <summary>
    ///     Статический класс, осуществляющий алгоритмы тестов простоты
    /// </summary>
    public static class PrimalityTests
    {
        /// <summary>
        ///     Перечисление возможных результатов теста простоты
        /// </summary>
        public enum PrimalityTestResult
        {
            /// <summary>
            ///     Неизвестно. Тест прошел успешно, но этого недостаточно для того, чтобы сделать вывод о простоте числа
            /// </summary>
            Unknown,

            /// <summary>
            ///     Число составное.
            /// </summary>
            Composite,

            /// <summary>
            ///     Число простое.
            /// </summary>
            Prime
        }

        /// <summary>
        ///     Fermat Primality Test - тест на основе теоремы Ферма. Имеет псевдопростые числа
        /// </summary>
        /// <param name="source">Число, которое необходимо проверить на простотоу</param>
        /// ///
        /// <param name="count">Число прогонов теста. Чем больше, тем точнее ответ.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static PrimalityTestResult FPT(int source, int count)
        {
            if (count >= source)
                throw new InvalidOperationException("Число прогонов теста должно быть меньше тестируемого числа.");
            switch (source)
            {
                case 0:
                    return PrimalityTestResult.Composite;
                case 1:
                    throw new InvalidOperationException("Единица не является ни простым, ни составным числом.");
            }

            if (source < 0 || count <= 0)
                throw new InvalidOperationException(
                    "Тестируемое число и количество прогонов должны быть положительными числами!");

            BigInteger[] RestVariants = RD.UniformDistribution(2, source - 1, count);
            //отрезок [2,n-1]
            for (int i = 1; i <= count; i++)
            {
                BigInteger CurrentValue = RestVariants[i - 1];
                if (AdvancedEuclidsalgorithm.GCDResult(CurrentValue, source) != 1) return PrimalityTestResult.Composite;
                LinearComparison comparison = new LinearComparison(CurrentValue, source);
                if (MultiplicativeInverse.BinaryPowLinearComparison(comparison, source - 1).A != 1)
                    return PrimalityTestResult.Composite;
            }

            return PrimalityTestResult.Unknown;
        }

        /// <summary>
        ///     Выполняет полный прогон тестов по Малой теореме Ферма для указанного числа
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static PrimalityTestResult FPTFull(int source)
        {
            return FPT(source, source - 2);
        }

        /// <summary>
        ///     Тест Соловея-Штрассена. Имеет числа КарлМайкла
        /// </summary>
        /// <param name="source">Число, которое необходмио протестировать</param>
        /// <param name="count">Число прогонов теста</param>
        /// <returns></returns>
        public static PrimalityTestResult SSPT(BigInteger source, BigInteger count)
        {
            if (count >= source)
                throw new InvalidOperationException("Число прогонов теста не должно быть меньше тестируемого числа.");
            if(source==0)
                return PrimalityTestResult.Composite;
            if (source == 1)
                return PrimalityTestResult.Unknown;
            
            if (source < 0 || count <= 0)
            {
                throw new InvalidOperationException(
                    "Тестируемое число и число его прогонов должны быть положительными числами!");
            }

            //нам необходимо, чтобы число было нечетным, поэтому мы отсеиваем все четные числа.
            if (source % 2 == 0) return PrimalityTestResult.Composite;
            BigInteger[] RestVariants = RD.UniformDistribution(2, source - 1, count);
            //отрезок [2,n-1]
            for (int i = 0; i < count; i++)
            {
                BigInteger CurrentValue = RestVariants[i];
                if (AdvancedEuclidsalgorithm.GCDResult(CurrentValue, source) != 1) return PrimalityTestResult.Composite;
                //значение символа якоби
                BigInteger jacobi = JacobiSymbol.Get(CurrentValue, source);
                LinearComparison comparison = new LinearComparison(CurrentValue, source);
                if (MultiplicativeInverse.BinaryPowLinearComparison(comparison, (source - 1) / 2).LeastModulo != jacobi)
                    return PrimalityTestResult.Composite;
            }

            return PrimalityTestResult.Unknown;
        }

        /// <summary>
        ///     Выполняет полный прогон тестов Соловея-Штрассена для указанного числа
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static PrimalityTestResult SSPTFull(BigInteger source)
        {
            return SSPT(source, source - 2);
        }

        /// <summary>
        ///     Тест Рабина Миллера. Имеет сильно псевдопростые числа
        /// </summary>
        /// <param name="source"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static PrimalityTestResult MRPT(int source, int count)
        {
            if (count >= source)
                throw new InvalidOperationException("Число прогонов теста не должно быть меньше тестируемого числа.");
            switch (source)
            {
                case 0:
                    return PrimalityTestResult.Composite;
                case 1:
                    throw new InvalidOperationException("Единица не является ни простым, ни составным числом.");
            }

            if (source < 0 || count <= 0)
            {
                throw new InvalidOperationException(
                    "Тестируемое число и число его прогонов должны быть положительными числами!");
            }

            //нам необходимо, чтобы число было нечетным, поэтому мы отсеиваем все четные числа.
            if (source % 2 == 0) return PrimalityTestResult.Composite;
            int t = source - 1;
            int s = 0;
            while (t % 2 == 0)
            {
                t /= 2;
                s++;
            }
            //n-1 = (2^s) * t


            BigInteger[] RestVariants = RD.UniformDistribution(2, source - 1, count);
            //отрезок [2,n-1]
            for (int i = 0; i < count; i++)
            {
                BigInteger CurrentValue = RestVariants[i];
                if (AdvancedEuclidsalgorithm.GCDResult(CurrentValue, source) != 1) return PrimalityTestResult.Composite;
                //значение символа якоби
                LinearComparison comparison = new LinearComparison(CurrentValue, source);
                if (BigInteger.Abs((comparison = MultiplicativeInverse.BinaryPowLinearComparison(comparison, t))
                        .LeastModulo) == 1)
                    continue;

                while (s != 1)
                {
                    comparison = MultiplicativeInverse.BinaryPowLinearComparison(comparison, 2);
                    if (comparison.LeastModulo == -1) break;
                    if (--s == 0) return PrimalityTestResult.Composite;
                }
            }

            return PrimalityTestResult.Unknown;
        }

        /// <summary>
        ///     Выполняет полный прогон тестов Рабина-Миллера для указанного числа
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static PrimalityTestResult MRPTFull(int source)
        {
            return MRPT(source, source - 2);
        }
    }
    }
}