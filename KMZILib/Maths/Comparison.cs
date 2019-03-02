using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace KMZILib
{
    /// <summary>
    ///     Представляет статический класс для работы со сравнениями первой степени.
    /// </summary>
    public static class Comparison
    {
        /// <summary>
        ///     Возвращает значение функции Эйлера для целого числа.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static int EulersFunction(int a)
        {
            //TODO: все еще работает с интами ибо нет реализованных функций взятия корня и логарифма
            int[] Dividers = GetUniqueNumberDividers(a);
            int EulersFunction = Dividers.Aggregate(a, (current, divider) => current * (divider - 1));
            int fract = Dividers.Aggregate(1, (current, divider) => current * divider);
            return EulersFunction / fract;
        }

        /// <summary>
        ///     Получает набор уникальных делителей числа и возвращает в виде массива.
        /// </summary>
        /// <param name="Number" />
        /// <returns></returns>
        public static int[] GetUniqueNumberDividers(int Number)
        {
            if (Number < 0)
                Number *= -1;
            int Length = (int)Math.Sqrt(Number) + 1;
            List<int> dividers = new List<int>() { Number };
            while (Number != 1)
                for (int i = 2; i <= Length; i++)
                {
                    if (Number % i != 0)
                    {
                        if (i != Length)
                            continue;
                        if (!dividers.Contains(Number))
                            dividers.Add(Number);
                        Number /= Number;
                        break;
                    }

                    if (!dividers.Contains(i))
                        dividers.Add(i);
                    Number /= i;
                    break;
                }

            return dividers.ToArray();
        }

        /// <summary>
        ///     Получает набор уникальных делителей числа при помощи факторизации и возвращает в виде массива.
        /// </summary>
        /// <param name="Number" />
        /// <returns></returns>
        public static int[] GetUniqueNumberDividersF(int Number)
        {
            if (Number < 0)
                Number *= -1;
            List<int> dividers = new List<int>();
            int i = 1;
            while (++i < (int)Math.Sqrt(Number))
                if (Number % i == 0)
                    dividers.Add(i);
            dividers.Add(Number);
            return dividers.ToArray();
        }

        /// <summary>
        ///     Получает набор делителей числа и возвращает в виде массива.
        /// </summary>
        /// <param name="Number" />
        /// <returns></returns>
        public static int[] GetNumberDividers(int Number)
        {
            int Length = (int)Math.Sqrt(Number) + 1;
            if (Number < 0)
                Number *= -1;
            List<int> dividers = new List<int>();
            while (Number != 1)
                for (int i = 2; i <= Length; i++)
                {
                    if (Number % i != 0)
                    {
                        if (i != Length)
                            continue;
                        dividers.Add(Number);
                        Number /= Number;
                        break;
                    }

                    dividers.Add(i);
                    Number /= i;
                    break;
                }

            return dividers.ToArray();
        }

        /// <summary>
        ///     Осуществляет решение линейного сравнения.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="m"></param>
        /// <param name="Result"></param>
        /// <param name="GCD"></param>
        /// <returns></returns>
        public static bool Solve(BigInteger a, BigInteger b, BigInteger m, out LinearComparison Result,
            BigInteger[] GCD = null)
        {
            //ax=b(mod m)

            Result = null;
            if (GCD == null)
                GCD = AdvancedEuclidsalgorithm.GCD(a, m);
            if (GCD == null)
                return false;
            if (b % GCD[0] != 0)
                return false;
            //a*x0=d(mod m)
            BigInteger x_ = GCD[1] * (b / GCD[0]);
            while (x_ >= m / GCD[0])
                x_ -= m / GCD[0];
            while (x_ < 0)
                x_ += m / GCD[0];
            Result = new LinearComparison(x_, m / GCD[0]);
            return true;
        }

        /// <summary>
        ///     Представляет класс сравнения. Содержит данные об остатке и моделе сравнения.
        /// </summary>
        public class LinearComparison
        {
            //x=a(mod m)

            /// <summary>
            ///     Число, стоящее после знака "сравнимо".
            /// </summary>
            private BigInteger a;

            /// <summary>
            ///     Инициализирует новое линейное сравнение с заданным остатоком и модулем
            /// </summary>
            /// <param name="a">Остаток линейного сравнения</param>
            /// <param name="m">Модуль линейного сравнения</param>
            public LinearComparison(BigInteger a, BigInteger m)
            {
                M = m;
                A = a;
            }

            /// <summary>
            ///     Инициализирует новое линейное сравнение копией заданного сравнения
            /// </summary>
            /// <param name="Other">Сравнение, для которого требуется создать копию</param>
            public LinearComparison(LinearComparison Other)
            {
                M = Other.M;
                A = Other.A;
            }

            /// <summary>
            ///     Остаток сравнения. Нормализуется по модулю <see cref="M" />.
            /// </summary>
            public BigInteger A
            {
                get => a;
                set
                {
                    if (M <= 0)
                        throw new InvalidOperationException("Модуль должен быть больше нуля.");
                    a = value;
                    while (a < 0)
                        a += M;
                    a %= M;
                }
            }

            /// <summary>
            ///     Возвращает наименьшее неотрицательное значение <see cref="A" />
            /// </summary>
            public BigInteger SmallestNonnegative => A;

            /// <summary>
            ///     Возвращает наименьшее по модулю значение <see cref="A" />
            /// </summary>
            public BigInteger LeastModulo => BigInteger.Abs(A - M) < A ? A - M : A;

            /// <summary>
            ///     Модуль сравнения.
            /// </summary>
            public BigInteger M { get; }

            /// <summary>
            ///     Возвращает результат сравненения с другим объектом <see cref="LinearComparison"/>
            /// </summary>
            /// <param name="other">Сравнение, с котором необходимо сравнить данное сравнение</param>
            /// <returns>true - остаток и модуль совпадают. false - что-то отличается</returns>
            protected bool Equals(LinearComparison other)
            {
                return a == other.a && M == other.M;
            }

            /// <summary>
            ///     Определяет, является ли заданный <see cref="object" /> линейным сравнением, совпадающим с данным
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                return obj.GetType() == GetType() && Equals((LinearComparison)obj);
            }

            /// <summary>
            ///     Возвращает хэш-код данного линейного сравнения
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return (int)(a * 397) ^ (int)M;
            }

            /// <summary>
            ///     Сравнивает остаток и модуль двух линейных сравнений
            /// </summary>
            /// <param name="First">Первое сравнение</param>
            /// <param name="Second">Второе сравнение</param>
            /// <returns>true - Остаток и модуль совпадают. false- что-то отличается</returns>
            public static bool operator ==(LinearComparison First, LinearComparison Second)
            {
                return First.A == Second.A && First.M == Second.M;
            }

            /// <summary>
            ///     Сравнивает остаток и модуль двух линейных сравнений
            /// </summary>
            /// <param name="First">Первое сравнение</param>
            /// <param name="Second">Второе сравнение</param>
            /// <returns>true - Что-то отличается. false- Остаток и модуль совпадают</returns>
            public static bool operator !=(LinearComparison First, LinearComparison Second)
            {
                return First.A != Second.A || First.M != Second.M;
            }

            /// <summary>
            ///     Возвращает строковое представление данного сравнения
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return $"x ≡ {A} (mod {M})";
            }
        }

        /// <summary>
        ///     Представляет статический класс для вычисления мультипликативного обратного элемента.
        /// </summary>
        public static class MultiplicativeInverse
        {
            /// <summary>
            ///     Возвращает результат нахождения мультипликативного обратного, если он существует, с помощью РАЕ.
            ///     (mod m)
            /// </summary>
            /// <returns>Сущесвует ли мультипликативный обратный?</returns>
            public static bool TrySolveByGCD(BigInteger a, BigInteger m, out LinearComparison Result,
                BigInteger[] GCD = null)
            {
                Result = null;
                if (GCD == null)
                    GCD = AdvancedEuclidsalgorithm.GCD(a, m);
                if (GCD == null || GCD[0] != 1)
                    return false;
                Result = new LinearComparison(GCD[1], m);
                return true;
            }

            /// <summary>
            ///     Возвращает результат нахождения мультипликативного обратного, если последний существует, с помощью РАЕ.
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
            ///     Возвращает результат нахождения мультипликативного обратного для элемента сравнения.
            /// </summary>
            /// <param name="comparison">Сравнение, в котором необходимо найти мультипликативный обратный к остатку</param>
            /// <returns></returns>
            public static BigInteger Solve(LinearComparison comparison)
            {
                return Solve(comparison.A, comparison.M);
            }

            /// <summary>
            ///     Внутренний метод, осуществляющий проверку, является ли число простым. Метод пробных делений.
            /// </summary>
            /// <param name="a"></param>
            /// <returns></returns>
            private static bool IsPrimeNumber(int a)
            {
                if (a < 0)
                    a *= -1;
                if (a <= 1)
                    return false;
                for (int i = 2; i < (int)Math.Sqrt(a) + 1; i++)
                    if (a % i == 0)
                        return false;
                return true;
            }

            /// <summary>
            ///     Возвращает результат поиска мультипликативного обратного, если последний существует, с помощью Бинарного метода.
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
                if (AdvancedEuclidsalgorithm.GCDResult(a, m) != 1)
                    return false;
                Result = new LinearComparison(a, m);
                int degree = EulersFunction((int)m) - 1;
                BitArray sma = new BitArray(new[] { degree });
                int i = sma.Length - 1;
                while (!sma[i])
                    i--;
                for (i--; i >= 0; i--)
                    if (sma[i])
                    {
                        Result.A *= Result.A;
                        Result.A *= a;
                    }
                    else
                        Result.A *= Result.A;

                return true;
            }

            /// <summary>
            ///     Возвращает результат возведения остатка сравнения в указанную степень бинарным способом.
            /// </summary>
            /// <param name="source"></param>
            /// <param name="degree"></param>
            /// <returns></returns>
            public static LinearComparison BinaryPowLinearComparison(LinearComparison source, BigInteger degree)
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
}