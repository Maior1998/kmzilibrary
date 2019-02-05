﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using static KMZILib.Comparison;
using static KMZILib.Polynoms;
using static KMZILib.PrimalityTests;

namespace KMZILib
{
    /// <summary>
    ///     Класс, предоставляющий методы для работы с криптографическими задачами
    /// </summary>
    public static partial class Ciphers
    {
        /// <summary>
        ///     Класс, предоставляющий методы для работы с разделением секрета
        /// </summary>
        public static class SecretSharing
        {
            private static BigInteger GetBiggerRandomPrime(BigInteger current)
            {
                if (BigInteger.Abs(current) < PrimeNumbers.Last())
                {
                    return PrimeNumbers[
                        RD.Rand.Next(
                            PrimeNumbers.ToList().IndexOf(
                                PrimeNumbers.First(
                                    num => num > BigInteger.Abs(current))),
                            PrimeNumbers.Length)
                    ];
                }

                BigInteger find = current % 2 == 0 ? BigInteger.Abs(current) + 1 : BigInteger.Abs(current) + 2;
                while (SSPTFull(find) == PrimalityTestResult.Composite) find += 2;
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
                public static KeyValuePair<int, BigInteger>[] Share(BigInteger Key, int CountOfFragments, out BigInteger module,
                    int Limit = -1)
                {
                    if (Limit == -1) Limit = CountOfFragments;
                    module = GetBiggerRandomPrime(Key);

                    //Вычислили модуль многочлена и знаем порог - пора генерировать многочлен.
                    BigInteger[] coefs = new BigInteger[Limit];
                    for (int i = 0; i < coefs.Length - 1; i++)
                        coefs[i] = RD.UniformDistribution(1, module - 1, 1)[0];
                    coefs[coefs.Length - 1] = Key;
                    Polynom sharepolynom = new Polynom(coefs);
                    KeyValuePair<int, BigInteger>[] Keys = new KeyValuePair<int, BigInteger>[CountOfFragments];
                    for (int i = 1; i <= CountOfFragments; i++)
                        Keys[i - 1] = new KeyValuePair<int, BigInteger>(i, sharepolynom.GetValue(i, module));
                    return Keys;
                }

                /// <summary>
                ///     Осуществляет процесс восстановления ключа из его частей при помощи решения СЛУ методом Гаусса
                /// </summary>
                /// <param name="Fragments">Набор частей ключа</param>
                /// <param name="module">Модуль, по которому необходимо производить вычисления</param>
                /// <param name="key">Ключ, который будет вычислен в процессе</param>
                /// <returns>Получилось ли восстановить ключ</returns>
                public static bool Restore(KeyValuePair<int, BigInteger>[] Fragments, int module, out BigInteger key)
                {
                    key = SolveByGaussianMethod(Fragments, module);
                    //Limit - степень предполагаемого многочлена?
                    return key != int.MaxValue;
                }

                private static BigInteger SolveByGaussianMethod(KeyValuePair<int, BigInteger>[] Fragments, int module)
                {
                    int polynomdegree = Fragments.Length;
                    LinearComparison[][] Matrix = new LinearComparison[polynomdegree][];
                    for (int i = 0; i < polynomdegree; i++)
                    {
                        Matrix[i] = new LinearComparison[polynomdegree + 1].Select(_ => new LinearComparison(0, module))
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
                            LinearComparison[] target = Matrix.First(row =>
                                row.Skip(row.Length - 2).All(element => element.A != 0) &&
                                row.Take(row.Length - 2).All(element => element.A == 0)).ToArray();
                            Solve(target[target.Length - 2].A,
                                target[target.Length - 1].A,
                                module, out LinearComparison result);
                            return result.A;
                        }

                        int FirstNonZeroIndex = i;
                        while (FirstNonZeroIndex < Matrix.Length && Matrix[FirstNonZeroIndex][i].A == 0)
                            FirstNonZeroIndex++;
                        if (FirstNonZeroIndex == Matrix.Length) continue;
                        SwapLines(Matrix, i, FirstNonZeroIndex);
                        //перебираем все остальные j-ые строки и производим вычитание
                        for (int j = i + 1; j < polynomdegree; j++)
                        {
                            if (Matrix[j][i].A == 0) continue;

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
                        module, out LinearComparison Result)
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
                    int module)
                {
                    LinearComparison Result = new LinearComparison(0, module);
                    LinearComparison buffer = new LinearComparison(1, module);

                    foreach (KeyValuePair<int, BigInteger> fragment1 in Fragments)
                    {
                        buffer.A = 1;
                        foreach (KeyValuePair<int, BigInteger> fragment2 in Fragments)
                        {
                            LinearComparison fract = new LinearComparison(fragment2.Key - fragment1.Key,
                                module);
                            if (fract.A == 0) continue;
                            buffer.A *= fragment2.Key * MultiplicativeInverse.Solve(fract.A, module);
                        }

                        Result.A += buffer.A * fragment1.Value;
                    }

                    return Result.A;
                }

                private static void SwapLines(LinearComparison[][] Matrix, int row1, int row2)
                {
                    if (row1 == row2) return;
                    LinearComparison[] buffer = Matrix[row1];
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
                /// Схема Асмута-Блума.
                /// </summary>
                public static class AsmuthBloomScheme
                {
                    public static IEnumerable<LinearComparison> Share(BigInteger Key,int Count, int Limit=0)
                    {
                        if (Limit == 0) Limit = Count;
                        BigInteger p = GetBiggerRandomPrime(Key);
                        return null;
                    }
                }
            }
        }
    }
}