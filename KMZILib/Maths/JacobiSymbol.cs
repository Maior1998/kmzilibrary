using System.Numerics;

namespace KMZILib.Maths
{
    /// <summary>
    ///     Представляет методы для вычисления символа Лежандра/Якоби
    /// </summary>
    public static class JacobiSymbol
    {
        /// <summary>
        ///     Возвращает значение символа Лежандра/Якоби (-1/0/1)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="m"></param>
        /// <returns>
        ///     (-1/0/1)
        ///     2 - m четное
        ///     -3 - m меньше либо равно 1
        /// </returns>
        public static BigInteger Get(BigInteger a, BigInteger m)
        {
            if (m.IsEven) return 2;
            if (m <= 1) return -3;
            int sign = 1;

            a %= m;
            if (a == 0) return 0;
            int t = 0;
            while (a % 2 == 0)
            {
                t++;
                a /= 2;
            }

            sign *= t % 2 == 0 ? 1 : ((BigInteger.Pow(m, 2) - 1) / 8).IsEven ? 1 : -1;
            if (a == 1) return sign;

            sign *= ((a - 1) / 2 * ((m - 1) / 2)).IsEven ? 1 : -1;
            return sign * Get(m, a);
        }
    }
}