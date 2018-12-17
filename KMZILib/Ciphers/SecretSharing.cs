using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static KMZILib.Comparison;
using LinearComparison=KMZILib.Comparison.LinearComparison;
using Polynom=KMZILib.Polynoms.Polynom;

namespace KMZILib
{
    /// <summary>
    /// Класс, предоставляющий методы для работы с криптографическими задачами
    /// </summary>
    public static partial class Ciphers
    {
        /// <summary>
        /// Класс, предоставляющий методы для работы с разделением секрета
        /// </summary>
        public static class SecretSharing
        {
            /// <summary>
            /// Разделение секрета схемой Шамира (Shamir's Secret Sharing)
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
                public static KeyValuePair<int, BigInteger>[] Share(int Key, int CountOfFragments, out int module,
                    int Limit = -1)
                {
                    if (Limit == -1) Limit = CountOfFragments;
                    module = GetBiggerRandomPrime(Key);

                    //Вычислили модуль многочлена и знаем порог - пора генерировать многочлен.
                    int[] coefs = new int[Limit];
                    for (int i = 0; i < coefs.Length - 1; i++)
                        coefs[i] = RD.Rand.Next(1,module);
                    coefs[coefs.Length - 1] = Key;
                    Polynom sharepolynom = new Polynom(coefs);
                    KeyValuePair<int, BigInteger>[] Keys = new KeyValuePair<int, BigInteger>[CountOfFragments];
                    for (int i = 1; i <= CountOfFragments; i++)
                        Keys[i - 1] = new KeyValuePair<int, BigInteger>(i, sharepolynom.GetValue(i, module));
                    return Keys;
                }

                /// <summary>
                /// Осуществляет процесс восстановления ключа из его частей при помощи решения СЛУ методом Гаусса
                /// </summary>
                /// <param name="Fragments">Набор частей ключа</param>
                /// <param name="module">Модуль, по которому необходимо производить вычисления</param>
                /// <param name="key">Ключ, который будет вычислен в процессе</param>
                /// <returns>Получилось ли восстановить ключ</returns>
                public static bool Restore(KeyValuePair<int, BigInteger>[] Fragments, int module, out BigInteger key)
                {
                    key = SolveByGaussianMethod(Fragments, module);
                    //Limit - степень предполагаемого многочлена?
                    return key!=int.MaxValue;
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
                        for (int j = polynomdegree-2; j >= 0; j--)
                            Matrix[i][j].A = Matrix[i][j+1].A*Fragments[i].Key;
                        Matrix[i][Matrix[i].Length - 1].A = Fragments[i].Value;
                    }
                    //Матрица проинициализирована

                    //Фиксируем i-ую строку и начинаем у всех остальных создавать нули в столбце i
                    for (int i = 0; i < polynomdegree-1; i++)
                    {

                        if (Matrix.Any(row =>
                            row.Skip(row.Length - 1).All(element => element.A != 0) &&
                            row.Take(row.Length - 1).All(element => element.A == 0)))
                        {
                            LinearComparison[] target = Matrix.First(row =>
                                row.Skip(row.Length - 2).All(element => element.A != 0) &&
                                row.Take(row.Length - 2).All(element => element.A == 0)).ToArray();
                            Comparison.Solve(target[target.Length-2].A,
                                target[target.Length - 1].A,
                                module, out LinearComparison result);
                            return result.A;
                        }
                        int FirstNonZeroIndex = i;
                        while (FirstNonZeroIndex<Matrix.Length&&Matrix[FirstNonZeroIndex][i].A == 0) FirstNonZeroIndex++;
                        if(FirstNonZeroIndex==Matrix.Length) continue;
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
                                Matrix[j][k].A = Matrix[j][k].A *secondmultiplier  - Matrix[i][k].A * firstmultiplier;
                        }
                    }

                    //получили выражение вида ak=b(mod m)
                    
                    return Comparison.Solve(Matrix[polynomdegree-1][polynomdegree-1].A, Matrix[polynomdegree-1][polynomdegree].A,
                        module, out LinearComparison Result) ? Result.A : int.MaxValue;
                }

                /// <summary>
                /// Осуществляет процесс восстановления ключа из его частей при помощи многочлена Лагранжа
                /// </summary>
                /// <param name="Fragments">Набор частей ключа</param>
                /// <param name="module">Модуль, по которому необходимо производить вычисления</param>
                /// <returns>Значение ключа</returns>
                public static BigInteger RestoreByLagrangePolynomial(KeyValuePair<int, BigInteger>[] Fragments, int module)
                {
                    LinearComparison Result=new LinearComparison(0,module);
                    LinearComparison buffer = new LinearComparison(1, module);

                    foreach (KeyValuePair<int, BigInteger> fragment1 in Fragments)
                    {
                        buffer.A = 1;
                        foreach (KeyValuePair<int, BigInteger> fragment2 in Fragments)
                        {
                            LinearComparison fract = new LinearComparison(fragment2.Key - fragment1.Key,
                                module);
                            if (fract.A==0) continue;
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
            /// Разделение секрета на основе Греко-Китайской теореме об остатках. Основана на схеме Асмута — Блума.
            /// </summary>
            public static class CRT
            {
                /// <summary>
                /// Разделение секрета при помощи Греко-Китайской теоремы об остатках
                /// </summary>
                /// <param name="Key"></param>
                /// <param name="Count"></param>
                /// <param name="Limit"></param>
                /// <returns></returns>
                public static int[][] Share(int Key,int Count,int Limit)
                {
                    int p = GetBiggerRandomPrime(Key);
                    //p - первое простое число ,первосходящее Key
                    List<int> NumEnum=new List<int>(new []{GetBiggerRandomPrime(p)});
                    while(NumEnum.Count!=Count)
                        NumEnum.Add(GetBiggerRandomPrime(NumEnum.Last()));
                    while (!IsGoodEnum(NumEnum, Limit, p))
                    {
                        NumEnum.RemoveAt(0);
                        NumEnum.Add(GetBiggerRandomPrime(NumEnum.Last()));
                    }

                    int Key_ = Key + p * RD.Rand.Next(2, 101);
                    int[][]Result = new int[Count][];
                    for (int i = 0; i < Count; i++)
                        Result[i] = new[] {p, NumEnum[i], Key_ % NumEnum[i]};
                    return Result;
                }

                /// <summary>
                /// Восстановление ключа при помощи Греко-Китайской теоремы об остатках
                /// </summary>
                /// <param name="comparisons"></param>
                /// <returns></returns>
                public static int RestoreKey(int[][] comparisons)
                {
                    //{p, m, x}
                    int p = comparisons.First()[0];
                    LinearComparison[] comparisonsarray =
                        comparisons.Select(comp => new LinearComparison(comp[2], comp[1])).ToArray();
                    BigInteger supres = KMZILib.CRT.SolveBigInteger(comparisonsarray);
                    return (int)(supres% new BigInteger(p));

                }



                private static bool IsGoodEnum(List<int> numenum, int m,int p)
                {
                    int leftmult= numenum.Take(m).Aggregate(1, (current, VARIABLE) => current * VARIABLE);
                    int rightmult = numenum.Skip(m-1).Aggregate(p, (current, VARIABLE) => current * VARIABLE);
                    return leftmult > rightmult;
                }

                
            }
            private static int GetBiggerRandomPrime(int current)
            {
                if (Math.Abs(current) < PrimalityTests.PrimeNumbers.Last())
                    return PrimalityTests.PrimeNumbers[
                        RD.Rand.Next(
                            PrimalityTests.PrimeNumbers.ToList().IndexOf(
                                PrimalityTests.PrimeNumbers.First(
                                    num => num > Math.Abs(current))),
                                PrimalityTests.PrimeNumbers.Length)
                    ];
                int find = Math.Abs(current) + 1;
                while (PrimalityTests.SSPTFull(find) == PrimalityTests.PrimalityTestResult.Composite) find++;
                return find;
            }

        }
    }
}
