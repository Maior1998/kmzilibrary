using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using KMZILib;
using static KMZILib.Ciphers.Languages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static KMZILib.Ciphers.SecretSharing.CRT;

namespace Tests
{
    [TestClass]
    public class MainTests
    {
        [TestClass]
        public class Languages
        {
            [TestMethod]
            public void LanguageOneInstance()
            {
                ALanguage firstrus = RussianLanguage.GetInstanse();
                ALanguage secondrus = RussianLanguage.GetInstanse();

                ALanguage firsteng = EnglishLanguage.GetInstanse();
                ALanguage secondeng = EnglishLanguage.GetInstanse();

                Assert.AreEqual(firstrus, secondrus);
                Assert.AreEqual(firsteng, secondeng);
            }

            [TestMethod]
            public void LanguageSearchBySymbol()
            {
                foreach (ALanguage language in LanguagesList)
                    foreach (char symbol in language.Alphabet)
                        Assert.AreEqual(LangByChar(RD.Rand.Next(0, 2) == 0 ? symbol : char.ToLower(symbol)), language);
            }
        }

        [TestClass]
        public class GCD
        {
            [TestMethod]
            public void BothPositive()
            {
                BigInteger[][] Sources =
                {
                    new BigInteger[] {2, 6},
                    new BigInteger[] {14, 8657},
                    new BigInteger[] {46, 99},
                    new BigInteger[] {13, 4674},
                    new BigInteger[] {1, 6},
                    new BigInteger[] {7, 3},
                    new BigInteger[] {1234, 2468},
                    new BigInteger[] {87943566, 7658366}
                };
                BigInteger[] Answers = { 2, 1, 1, 1, 1, 1, 1234, 2 };
                for (int i = 0; i < Sources.Length; i++)
                    Assert.AreEqual(Answers[i], AdvancedEuclidsalgorithm.GCDResult(Sources[i][0], Sources[i][1]));
            }

            [TestMethod]
            public void OneNegative()
            {
                BigInteger[][] Sources =
                {
                    new BigInteger[] {-2, 6},
                    new BigInteger[] {14, -8657},
                    new BigInteger[] {46, -99},
                    new BigInteger[] {-13, 4674},
                    new BigInteger[] {1, -6},
                    new BigInteger[] {-7, 3},
                    new BigInteger[] {1234, -2468},
                    new BigInteger[] {-87943566, 7658366}
                };
                BigInteger[] Answers = { 2, 1, 1, 1, 1, 1, 1234, 2 };
                for (int i = 0; i < Sources.Length; i++)
                    Assert.AreEqual(Answers[i], AdvancedEuclidsalgorithm.GCDResult(Sources[i][0], Sources[i][1]));
            }

            [TestMethod]
            public void BothNegative()
            {
                BigInteger[][] Sources =
                {
                    new BigInteger[] {-2, -6},
                    new BigInteger[] {-14, -8657},
                    new BigInteger[] {-46, -99},
                    new BigInteger[] {-13, -4674},
                    new BigInteger[] {-1, -6},
                    new BigInteger[] {-7, -3},
                    new BigInteger[] {-1234, -2468},
                    new BigInteger[] {-87943566, -7658366}
                };
                BigInteger[] Answers = { 2, 1, 1, 1, 1, 1, 1234, 2 };
                for (int i = 0; i < Sources.Length; i++)
                    Assert.AreEqual(Answers[i], AdvancedEuclidsalgorithm.GCDResult(Sources[i][0], Sources[i][1]));
            }
        }

        [TestClass]
        public class DiophantieEquation
        {
            [TestMethod]
            public void DioEquation()
            {
                BigInteger[][] Sources =
                {
                    new BigInteger[] {3, 4, 5},
                    new BigInteger[] {3, 6, -2},
                    new BigInteger[] {364, 245, 0}
                };
                BigInteger[][] Answers =
                {
                    new BigInteger[] {-1, 4, 2, -3},
                    new BigInteger[] { },
                    new BigInteger[] {0, 35, 0, -52}
                };
                for (int i = 0; i < Sources.Length; i++)
                {
                    if (!DiophantineEquation.TrySolve(Sources[i],
                            out BigInteger[] Resultx,
                            out BigInteger[] Resulty) &&
                        Answers[i].Length != 0)
                        Assert.Fail("Функция не нашла ответа, а он есть.");
                    else if (Answers[i].Length == 0)
                        continue;
                    Assert.AreEqual(Resultx[0], Answers[i][0]);
                    Assert.AreEqual(Resultx[1], Answers[i][1]);
                    Assert.AreEqual(Resulty[0], Answers[i][2]);
                    Assert.AreEqual(Resulty[1], Answers[i][3]);
                }
            }
        }

        [TestClass]
        public class SecretSharing
        {
            [TestMethod]
            public void GaussMethodTest()
            {
                int Key = RD.Rand.Next(5, 2000);
                KeyValuePair<int, BigInteger>[] a = Ciphers.SecretSharing.SSS.Share(Key, 5, out BigInteger module, 3);
                Ciphers.SecretSharing.SSS.Restore(a, module, out BigInteger restoredkey);
                Assert.AreEqual(Key, (int)restoredkey);
            }

            [TestMethod]
            public void LagrangePolynomialTest()
            {
                int Key = RD.Rand.Next(5, 2000);
                KeyValuePair<int, BigInteger>[] a = Ciphers.SecretSharing.SSS.Share(Key, 5, out BigInteger module, 3);
                BigInteger restoredkey = Ciphers.SecretSharing.SSS.RestoreByLagrangePolynomial(a, module);
                Assert.AreEqual(Key, (int)restoredkey);
            }

            [TestMethod]
            public void CRTMethodTest()
            {
                BigInteger Key = RD.Rand.Next(0, 1000);
                int Count = RD.Rand.Next(3, 16);
                int Limit = RD.Rand.Next(2, Count + 1);
                Assert.AreEqual(Key,
                    AsmuthBloomScheme.Restore(AsmuthBloomScheme.Share(Key, Count, Limit).ToList(), Limit));

            }
        }

        [TestClass]
        public class JacobiSymbol
        {
            [TestMethod]
            public void BothPositive()
            {
                BigInteger[][] Sources =
                {
                    new BigInteger[] {219, 383},
                    new BigInteger[] {5, 19},
                    new BigInteger[] {3, 31},
                    new BigInteger[] {7, 12}
                };
                BigInteger[] Answers = { 1, 1, -1, 2 };
                for (int i = 0; i < Sources.Length; i++)
                    Assert.AreEqual(Answers[i], KMZILib.JacobiSymbol.Get(Sources[i][0], Sources[i][1]));
            }
        }

        [TestClass]
        public class MMA
        {
            [TestMethod]
            public void CheckSign()
            {
                Comparison.LinearComparison[][] Sources =
                {
                    new[]
                    {
                        new Comparison.LinearComparison(1, 2),
                        new Comparison.LinearComparison(1, 3),
                        new Comparison.LinearComparison(2, 5)
                    },
                    new[]
                    {
                        new Comparison.LinearComparison(3, 7),
                        new Comparison.LinearComparison(1, 3),
                        new Comparison.LinearComparison(0, 5),
                        new Comparison.LinearComparison(0, 2)
                    },
                    new[]
                    {
                        new Comparison.LinearComparison(6, 7),
                        new Comparison.LinearComparison(0, 3),
                        new Comparison.LinearComparison(0, 5),
                        new Comparison.LinearComparison(0, 2)
                    },
                    new[]
                    {
                        new Comparison.LinearComparison(6, 7),
                        new Comparison.LinearComparison(8, 11),
                        new Comparison.LinearComparison(0, 2)
                    }
                };

                int[] Answers = { 0, 0, 0, 1 };
                for (int i = 0; i < Sources.Length; i++)
                    Assert.AreEqual(Answers[i], new KMZILib.MMA(Sources[i]).Sign);
            }
        }

        [TestClass]
        public class PrimlityTests
        {
            [TestClass]
            public class FPT
            {
                [TestMethod]
                public void Composites()
                {
                    int[] Sources = { 6, 45, 78, 986, 1235, 9875, 19356395 };
                    foreach (int num in Sources)
                        Assert.AreEqual(KMZILib.Primes.PrimalityTests.FPTFull(num), KMZILib.Primes.PrimalityTests.PrimalityTestResult.Composite);
                }

                [TestMethod]
                public void Primes()
                {
                    int[] Sources = { 7, 641, 2417, 3061, 3607, 15277, 65437 };
                    foreach (int num in Sources)
                        Assert.AreEqual(KMZILib.Primes.PrimalityTests.FPTFull(num), KMZILib.Primes.PrimalityTests.PrimalityTestResult.Unknown);
                }
            }

            [TestClass]
            public class MRPT
            {
                [TestMethod]
                public void Composites()
                {
                    int[] Sources = { 6, 45, 78, 986, 1235, 9875, 19356395 };
                    foreach (int num in Sources)
                        Assert.AreEqual(KMZILib.Primes.PrimalityTests.MRPTFull(num), KMZILib.Primes.PrimalityTests.PrimalityTestResult.Composite);
                }

                [TestMethod]
                public void Primes()
                {
                    int[] Sources = { 7, 641, 2417, 3061, 3607, 15277, 65437 };
                    foreach (int num in Sources)
                        Assert.AreEqual(KMZILib.Primes.PrimalityTests.MRPTFull(num), KMZILib.Primes.PrimalityTests.PrimalityTestResult.Unknown);
                }
            }

            [TestClass]
            public class SSPT
            {
                [TestMethod]
                public void Composites()
                {
                    int[] Sources = { 6, 45, 78, 986, 1235, 9875, 19356395 };
                    foreach (int num in Sources)
                        Assert.AreEqual(KMZILib.Primes.PrimalityTests.SSPTFull(num), KMZILib.Primes.PrimalityTests.PrimalityTestResult.Composite);
                }

                [TestMethod]
                public void Primes()
                {
                    int[] Sources = { 7, 641, 2417, 3061, 3607, 15277, 65437 };
                    foreach (int num in Sources)
                        Assert.AreEqual(KMZILib.Primes.PrimalityTests.SSPTFull(num), KMZILib.Primes.PrimalityTests.PrimalityTestResult.Unknown);
                }
            }
        }
    }
}
