using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using OSULib.Maths;
using OSULib.Misc;
using static OSULib.Maths.Comparison;
using static OSULib.Maths.Polynoms;
using static OSULib.Maths.Primes.PrimalityTests;

namespace OSULib.Ciphers
{
    /// <summary>
    ///     Класс, предоставляющий методы для работы с разделением секрета
    /// </summary>
    public static class SecretSharing
    {
        private static BigInteger GetBiggerRandomPrime(BigInteger current)
        {
            if (BigInteger.Abs(current) < Misc.Misc.Data.PrimeNumbers.Last())
                return Misc.Misc.Data.PrimeNumbers[
                    RD.Rand.Next(
                        Misc.Misc.Data.PrimeNumbers.ToList().IndexOf(
                            Misc.Misc.Data.PrimeNumbers.First(
                                num => num > BigInteger.Abs(current))),
                        Misc.Misc.Data.PrimeNumbers.Length)
                ];

            BigInteger find = current % 2 == 0 ? BigInteger.Abs(current) + 1 : BigInteger.Abs(current) + 2;
            while (SSPTFull(find) == Primes.PrimalityTests.PrimalityTestResult.Composite)
                find += 2;
            return find;
        }

        /// <summary>
        ///     Разделение секрета схемой Шамира (Shamir's Secret Sharing)
        /// </summary>
        public static class SSS
        {
            /// <summary>
            ///     Осуществляет разделение ключа на несколько частей
            /// </summary>
            /// <param name="Key">Ключ, который необходимо разделить</param>
            /// <param name="CountOfFragments">Число фрагментов, на которые необходимо разделить ключ</param>
            /// <param name="module">Модуль, который будет определен исходя из ключа</param>
            /// <param name="Limit">Порог количества фрагментов, начиная с которого можно будет восстановить ключ</param>
            /// <returns>Массив наборов (x,y). x можно публиковать. y должен находиться в секрете.</returns>
            public static KeyValuePair<int, BigInteger>[] Share(BigInteger Key, int CountOfFragments,
                out BigInteger module,
                int Limit = -1)
            {
                if (Limit == -1)
                    Limit = CountOfFragments;
                module = GetBiggerRandomPrime(Key);

                //Вычислили модуль многочлена и знаем порог - пора генерировать многочлен.
                int[] coefs = new int[Limit];
                for (int i = 0; i < coefs.Length - 1; i++)
                    coefs[i] = (int) RD.UniformDistribution(1, module - 1, 1)[0];
                coefs[coefs.Length - 1] = (int) Key;
                Polynoms.ModularPolynom sharepolynom = new Polynoms.ModularPolynom(coefs, (int) module);
                KeyValuePair<int, BigInteger>[] Keys = new KeyValuePair<int, BigInteger>[CountOfFragments];
                for (int i = 1; i <= CountOfFragments; i++)
                    Keys[i - 1] = new KeyValuePair<int, BigInteger>(i, (BigInteger) sharepolynom.GetValue(i));
                return Keys;
            }

            /// <summary>
            ///     Осуществляет процесс восстановления ключа из его частей при помощи решения СЛУ методом Гаусса
            /// </summary>
            /// <param name="Fragments">Набор частей ключа</param>
            /// <param name="module">Модуль, по которому необходимо производить вычисления</param>
            /// <param name="key">Ключ, который будет вычислен в процессе</param>
            /// <returns>Получилось ли восстановить ключ</returns>
            public static bool Restore(KeyValuePair<int, BigInteger>[] Fragments, BigInteger module, out BigInteger key)
            {
                key = SolveByGaussianMethod(Fragments, module);
                //Limit - степень предполагаемого многочлена?
                return key != int.MaxValue;
            }

            private static BigInteger SolveByGaussianMethod(KeyValuePair<int, BigInteger>[] Fragments,
                BigInteger module)
            {
                int polynomdegree = Fragments.Length;
                Comparison.LinearComparison[][] Matrix = new Comparison.LinearComparison[polynomdegree][];
                for (int i = 0; i < polynomdegree; i++)
                {
                    Matrix[i] = new Comparison.LinearComparison[polynomdegree + 1].Select(_ => new Comparison.LinearComparison(0, module))
                        .ToArray();
                    Matrix[i][polynomdegree - 1].A = 1;
                    for (int j = polynomdegree - 2; j >= 0; j--)
                        Matrix[i][j].A = Matrix[i][j + 1].A * Fragments[i].Key;
                    Matrix[i][Matrix[i].Length - 1].A = Fragments[i].Value;
                }
                //Матрица проинициализирована

                //Фиксируем i-ую строку и начинаем у всех остальных создавать нули в столбце i
                for (int i = 0; i < polynomdegree - 1; i++)
                {
                    if (Matrix.Any(row =>
                        row.Skip(row.Length - 1).All(element => element.A != 0) &&
                        row.Take(row.Length - 1).All(element => element.A == 0)))
                    {
                        Comparison.LinearComparison[] target = Matrix.First(row =>
                            row.Skip(row.Length - 2).All(element => element.A != 0) &&
                            row.Take(row.Length - 2).All(element => element.A == 0)).ToArray();
                        Solve(target[target.Length - 2].A,
                            target[target.Length - 1].A,
                            module, out Comparison.LinearComparison result);
                        return result.A;
                    }

                    int FirstNonZeroIndex = i;
                    while (FirstNonZeroIndex < Matrix.Length && Matrix[FirstNonZeroIndex][i].A == 0)
                        FirstNonZeroIndex++;
                    if (FirstNonZeroIndex == Matrix.Length)
                        continue;
                    SwapLines(Matrix, i, FirstNonZeroIndex);
                    //перебираем все остальные j-ые строки и производим вычитание
                    for (int j = i + 1; j < polynomdegree; j++)
                    {
                        if (Matrix[j][i].A == 0)
                            continue;

                        //высчитываем НОД и коэффициенты
                        BigInteger firstmultiplier = AdvancedEuclidsalgorithm.LCM(Matrix[i][i].A, Matrix[j][i].A) /
                                                     Matrix[i][i].A;
                        BigInteger secondmultiplier = firstmultiplier * Matrix[i][i].A / Matrix[j][i].A;
                        Matrix[j][i].A = 0;
                        //пробегаем по всей строке и задаем значения после вычитания
                        for (int k = i + 1; k < polynomdegree + 1; k++)
                            Matrix[j][k].A = Matrix[j][k].A * secondmultiplier - Matrix[i][k].A * firstmultiplier;
                    }
                }

                //получили выражение вида ak=b(mod m)

                return Solve(Matrix[polynomdegree - 1][polynomdegree - 1].A,
                    Matrix[polynomdegree - 1][polynomdegree].A,
                    module, out Comparison.LinearComparison Result)
                    ? Result.A
                    : int.MaxValue;
            }

            /// <summary>
            ///     Осуществляет процесс восстановления ключа из его частей при помощи многочлена Лагранжа
            /// </summary>
            /// <param name="Fragments">Набор частей ключа</param>
            /// <param name="module">Модуль, по которому необходимо производить вычисления</param>
            /// <returns>Значение ключа</returns>
            public static BigInteger RestoreByLagrangePolynomial(KeyValuePair<int, BigInteger>[] Fragments,
                BigInteger module)
            {
                Comparison.LinearComparison Result = new Comparison.LinearComparison(0, module);
                Comparison.LinearComparison buffer = new Comparison.LinearComparison(1, module);

                foreach (KeyValuePair<int, BigInteger> fragment1 in Fragments)
                {
                    buffer.A = 1;
                    foreach (KeyValuePair<int, BigInteger> fragment2 in Fragments)
                    {
                        Comparison.LinearComparison fract = new Comparison.LinearComparison(fragment2.Key - fragment1.Key,
                            module);
                        if (fract.A == 0)
                            continue;
                        buffer.A *= fragment2.Key * Comparison.MultiplicativeInverse.Solve(fract.A, module);
                    }

                    Result.A += buffer.A * fragment1.Value;
                }

                return Result.A;
            }

            private static void SwapLines(Comparison.LinearComparison[][] Matrix, int row1, int row2)
            {
                if (row1 == row2)
                    return;
                Comparison.LinearComparison[] buffer = Matrix[row1];
                Matrix[row1] = Matrix[row2];
                Matrix[row2] = buffer;
            }
        }

        /// <summary>
        ///     Разделение секрета на основе Греко-Китайской теореме об остатках.
        /// </summary>
        public static class CRT
        {
            /// <summary>
            ///     Схема Асмута-Блума.
            /// </summary>
            public static class AsmuthBloomScheme
            {
                /// <summary>
                ///     Возвращает результат разбиения ключа с заданным числом долей и порогом. Если порог не указать, он будет равен числу
                ///     долей.
                /// </summary>
                /// <param name="Key">Ключ, который необходимо разбить. Натуральное число.</param>
                /// <param name="Count">Число долей.</param>
                /// <param name="Limit">Порог.</param>
                /// <returns></returns>
                public static IEnumerable<CRTPart> Share(BigInteger Key, int Count, int Limit = 0)
                {
                    /*
                     * M - секрет
                     * p - простое, большее M
                     * m - порог
                     * n - число долей
                     * d1, d2, d3, d4, ... dm - взаимнопростые числа (простые как костыль?)
                     * di > p (!!!!)
                     * d1 * d2 * d3 * d4 * ... * dm > p * d(n-m+2) * d(n-m+3) * ... * dn
                    */
                    if (Limit == 0)
                        Limit = Count;
                    BigInteger p = GetBiggerRandomPrime(Key);

                    BigInteger[] primes = new BigInteger[Count];
                    primes[0] = GetBiggerRandomPrime(p - 1);
                    while (true)
                    {
                        primes[0] = GetBiggerRandomPrime(primes[0]);
                        for (int i = 1; i < primes.Length; i++)
                            primes[i] = GetBiggerRandomPrime(primes[i - 1]);


                        BigInteger FirstMul = primes.Take(Limit)
                            .Aggregate<BigInteger, BigInteger>(1, (Current, val) => Current * val);
                        BigInteger SecondMul = p;
                        for (int i = Count - Limit + 1; i < Count; i++)
                            SecondMul *= primes[i];
                        if (FirstMul <= SecondMul) continue;
                        BigInteger NewM = Key + RD.Rand.Next() * p;
                        List<CRTPart> Result = primes.Select(prime => new CRTPart
                            {Comparison = new Comparison.LinearComparison(NewM, prime), P = p}).ToList();
                        return Result;
                    }
                }

                /// <summary>
                ///     Возвращает результат восстановления секрета по имеющимся частям и пороговому значению.
                /// </summary>
                /// <param name="Parts"></param>
                /// <param name="Limit"></param>
                /// <returns></returns>
                public static BigInteger Restore(List<CRTPart> Parts, int Limit)
                {
                    if (Parts.Count < Limit)
                        throw new InvalidOperationException(
                            "Невозможно восстановить секрет: число долей меньше порогового значения");
                    Comparison.LinearComparison NewM = Maths.CRT.Solve(Parts.Select(part => part.Comparison));
                    return new Comparison.LinearComparison(NewM.A, Parts.First().P).A;
                }

                /// <summary>
                ///     Часть секрета в схеме Асмута-Блума.
                /// </summary>
                public class CRTPart
                {
                    /// <summary>
                    ///     Втория и третья части секрета - r и d.
                    /// </summary>
                    public Comparison.LinearComparison Comparison;

                    /// <summary>
                    ///     Первая часть секрета - p.
                    /// </summary>
                    public BigInteger P;
                }
            }
        }
    }
}