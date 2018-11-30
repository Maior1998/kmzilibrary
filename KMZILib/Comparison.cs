using System;
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
        ///     Функция Эйлера для целого числа.
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
        ///     Факторизация числа. Получает набор уникальных делителей числа и возвращает в виде массива.
        /// </summary>
        /// <param name="Number"/>
        /// <returns></returns>
        public static int[] GetUniqueNumberDividers(int Number)
        {
            int Length = (int) Math.Sqrt(Number) + 1;
            if (Number < 0) Number *= -1;
            List<int> dividers = new List<int>();
            while (Number != 1)
                for (int i = 2; i <= Length; i++)
                {
                    if (Number % i != 0)
                    {
                        if (i != Length)
                            continue;
                        if (!dividers.Contains(Number)) dividers.Add(Number);
                        Number /= Number;
                        break;
                    }

                    if (!dividers.Contains(i)) dividers.Add(i);
                    Number /= i;
                    break;
                }

            return dividers.ToArray();
        }

        /// <summary>
        ///     Факторизация числа. Получает набор уникальных делителей числа при помощи факторизации и возвращает в виде массива.
        /// </summary>
        /// <param name="Number"/>
        /// <returns></returns>
        public static int[] GetUniqueNumberDividersF(int Number)
        {
            if (Number < 0) Number *= -1;
            List<int> dividers = new List<int>();
            int i = 1;
            while (++i < (int) Math.Sqrt(Number))
                if(Number%i==0) dividers.Add(i);
            dividers.Add(Number);
            return dividers.ToArray();
        }


        /// <summary>
        ///     Факторизация числа. Получает набор делителей числа и возвращает в виде массива.
        /// </summary>
        /// <param name="Number"/>
        /// <returns></returns>
        public static int[] GetNumberDividers(int Number)
        {
            int Length = (int)Math.Sqrt(Number) + 1;
            if (Number < 0) Number *= -1;
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
        public static bool Solve(BigInteger a, BigInteger b, BigInteger m, out LinearComparison Result, BigInteger[] GCD = null)
        {
            //ax=b(mod m)

            Result = null;
            if (GCD == null) GCD = AdvancedEuclidsalgorithm.GCD(a, m);
            if (GCD == null) return false;
            if (b % GCD[0] != 0) return false;
            //a*x0=d(mod m)
            BigInteger x_ = GCD[1] * (b / GCD[0]);
            while (x_ >= m / GCD[0]) x_ -= m / GCD[0];
            while (x_ < 0) x_ += m / GCD[0];
            Result = new LinearComparison(x_, m / GCD[0]);
            return true;
        }

        /// <summary>
        ///     Представляет класс сравнения. Содержит данные об остатке и моделе сравнения.
        /// </summary>
        public class LinearComparison
        {
            /// <summary>
            /// Возвращает результат сравненения с другим линейным сравнением
            /// </summary>
            /// <param name="other">Сравнение, с котором необходимо сравнить данное сравнение</param>
            /// <returns>true - остаток и модуль совпадают. false - что-то отличается</returns>
            protected bool Equals(LinearComparison other)
            {
                return a == other.a && M == other.M;
            }

            /// <summary>
            /// Определяет, является ли <see cref="object"/> obj линейным сравнением, совпадающим с данным
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == GetType() && Equals((LinearComparison) obj);
            }

            /// <summary>
            /// Возвращает хэш-код данного линейного сравнения
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    return (int)(a * 397) ^ (int)M;
                }
            }
            //x=a(mod m)

            /// <summary>
            ///     Число, стоящее после знака "сравнимо".
            /// </summary>
            private BigInteger a;

            /// <summary>
            /// Инициализирует новое линейное сравнение с заданным остатоком и модулем
            /// </summary>
            /// <param name="a">Остаток линейного сравнения</param>
            /// <param name="m">Модуль линейного сравнения</param>
            public LinearComparison(BigInteger a, BigInteger m)
            {
                M = m;
                A = a;
            }

            /// <summary>
            /// Инициализирует новое линейное сравнение копией заданного сравнения
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
                    if (M <= 0) throw new InvalidOperationException("Модуль должен быть больше нуля.");
                    a = value;
                    while (a < 0) a += M;
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
            ///     Модуль сравнения
            /// </summary>
            public BigInteger M { get; }

            /// <summary>
            /// Сравнивает остаток и модуль двух линейных сравнений
            /// </summary>
            /// <param name="First">Первое сравнение</param>
            /// <param name="Second">Второе сравнение</param>
            /// <returns>true - Остаток и модуль совпадают. false- что-то отличается</returns>
            public static bool operator ==(LinearComparison First, LinearComparison Second)
            {
                return First.A == Second.A && First.M == Second.M;
            }

            /// <summary>
            /// Сравнивает остаток и модуль двух линейных сравнений
            /// </summary>
            /// <param name="First">Первое сравнение</param>
            /// <param name="Second">Второе сравнение</param>
            /// <returns>true - Что-то отличается. false- Остаток и модуль совпадают</returns>
            public static bool operator !=(LinearComparison First, LinearComparison Second)
            {
                return First.A != Second.A || First.M != Second.M;
            }

            /// <summary>
            /// Возвращает строковое представление данного сравнения
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return $"x ≡ {A} (mod {M})";
            }
        }
    }
}