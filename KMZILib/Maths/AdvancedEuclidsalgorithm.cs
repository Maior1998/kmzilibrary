using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

namespace KMZILib
{
    /// <summary>
    ///     Представляет статический класс для работы с Расширенным Алгоритмом Евклида.
    /// </summary>
    public static class AdvancedEuclidsalgorithm
    {
        /// <summary>
        ///     Алгоритм Евклида для двух целых чисел типа <see cref="int" />, отличных от нуля. Возвращает массив из 3 элементов -
        ///     НОД, X0, Y0.
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static BigInteger[] GCD(BigInteger First, BigInteger Second)
        {
            if (First == 0 || Second == 0)
                return null;
            BigInteger Max = BigInteger.Abs(First) >= BigInteger.Abs(Second) ? First : Second;
            BigInteger Min = BigInteger.Abs(First) >= BigInteger.Abs(Second) ? Second : First;
            BigInteger[] answer = gcd(BigInteger.Abs(Max), BigInteger.Abs(Min), 0, 1, 1, 0);
            if (BigInteger.Abs(First) < BigInteger.Abs(Second))
            {
                BigInteger buffer = answer[1];
                answer[1] = answer[2];
                answer[2] = buffer;
            }

            if (First == Max)
            {
                answer[1] *= Max.Sign;
                answer[2] *= Min.Sign;
            }
            else
            {
                answer[1] *= Min.Sign;
                answer[2] *= Max.Sign;
            }

            return answer;
        }

        /// <summary>
        ///     Алгоритм Евклида для двух целых чисел типа <see cref="int" />. Возвращает массив , представляющий собой таблицу РАЕ
        ///     для этих чисел.
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static BigInteger[,] GCDTable(BigInteger First, BigInteger Second)
        {
            if (First == 0 || Second == 0)
                return null;
            BigInteger Max = BigInteger.Abs(First) >= BigInteger.Abs(Second) ? First : Second;
            BigInteger Min = BigInteger.Abs(First) >= BigInteger.Abs(Second) ? Second : First;

            BigInteger a = BigInteger.Abs(Max), b = BigInteger.Abs(Min);
            List<BigInteger[]> Table = new List<BigInteger[]>
            {
                new[] {a, new BigInteger(1), new BigInteger(0)},
                new[] {b, new BigInteger(0), new BigInteger(1), new BigInteger(0)}
            };
            int i = 1;
            while (b != 0)
            {
                Table[i][3] = a / b;
                Table.Add(new[]
                {
                    //остаток от деления
                    a % b,
                    //x0[i-1] - x0[i]*q
                    Table[i - 1][1] - Table[i][1] * Table[i][3],
                    //y0[i-1] - y0[i]*q
                    Table[i - 1][2] - Table[i][2] * Table[i][3],
                    //q
                    0
                });
                a = b;
                b = Table[i + 1][0];
                i++;
            }

            BigInteger[,] answer = new BigInteger[Table.Count, 4];
            for (i = 0; i < Table.Count - 1; i++)
            for (int j = 0; j < 4; j++)
                answer[i, j] = j < Table[i].Length ? Table[i][j] : 0;
            answer[answer.GetLength(0) - 1, 1] = answer[answer.GetLength(0) - 2, 1];
            answer[answer.GetLength(0) - 1, 2] = answer[answer.GetLength(0) - 2, 2];
            if (BigInteger.Abs(First) < BigInteger.Abs(Second))
            {
                BigInteger buffer = answer[answer.GetLength(0) - 1, 1];
                answer[answer.GetLength(0) - 1, 1] = answer[answer.GetLength(0) - 2, 2];
                answer[answer.GetLength(0) - 1, 2] = buffer;
            }

            if (First == Max)
            {
                answer[answer.GetLength(0) - 1, 1] *= Max.Sign;
                answer[answer.GetLength(0) - 1, 2] *= Min.Sign;
            }
            else
            {
                answer[answer.GetLength(0) - 1, 1] *= Min.Sign;
                answer[answer.GetLength(0) - 1, 2] *= Max.Sign;
            }

            return answer;
        }

        private static BigInteger[] gcd(BigInteger First, BigInteger Second, BigInteger a, BigInteger b,
            BigInteger olda, BigInteger oldb)
        {
            if (Second == 0) return new[] {First, olda, oldb};
            BigInteger q = First / Second;
            return gcd(Second, First % Second, olda - a * q, oldb - b * q, a, b);
        }


        private static Polynoms.ModularPolynom[] gcd(Polynoms.ModularPolynom First, Polynoms.ModularPolynom Second, Polynoms.ModularPolynom x, Polynoms.ModularPolynom y,
            Polynoms.ModularPolynom oldx, Polynoms.ModularPolynom oldy)
        {
            if (Second.Degree == 0)
                return new[] { Second[0]==0?First: Second, x, y };
            Console.WriteLine($"{Second}\t\t\t{x}\t\t\t{y}\t\t\t{First/Second}");
            Polynoms.ModularPolynom q = First / Second;
            return gcd(Second, First % Second, oldx - x * q, oldy - y * q, x, y);
        }

        /// <summary>
        /// Вычисляет НОД для многочленов.
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static Polynoms.ModularPolynom GCDResult(Polynoms.ModularPolynom First, Polynoms.ModularPolynom Second)
        {
            Console.WriteLine($"{First}\t\t\t{new Polynoms.ModularPolynom(new[] { 1 }, First.Module)}\t\t\t{new Polynoms.ModularPolynom(0, First.Module)}\t\t\t-");
            return gcd(First, Second,new Polynoms.ModularPolynom(0, First.Module) , new Polynoms.ModularPolynom(new[] { 1 }, First.Module), new Polynoms.ModularPolynom(new[] { 1 }, First.Module), new Polynoms.ModularPolynom(0, First.Module))[0];
        }

        public static Polynoms.ModularPolynom[] GCD(Polynoms.ModularPolynom First, Polynoms.ModularPolynom Second)
        {
            return gcd(First, Second,new Polynoms.ModularPolynom(new[] { 1 }, First.Module) , new Polynoms.ModularPolynom(0, First.Module), new Polynoms.ModularPolynom(0, First.Module), new Polynoms.ModularPolynom(new[] { 1 }, First.Module));
        }

        /// <summary>
        ///     Алгоритм Евклида для двух целых чисел типа <see cref="int" />. Возвращает одно число - НОД исходных чисел.
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static BigInteger GCDResult(BigInteger First, BigInteger Second)
        {
            BigInteger[] res = GCD(First, Second);
            if (res == null)
                return -1;
            return res[0];
        }

        /// <summary>
        /// Вычисляет НОД массива чисел.
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static BigInteger GCDResult(BigInteger[] Source)
        {
            if (Source.Length == 0)
                return 0;
            if (Source.Length == 1)
                return Source[0];
            BigInteger Result = GCDResult(Source[0], Source[1]);
            for (int i = 2; i < Source.Length; i++)
                Result = GCDResult(Result, Source[i]);
            return Result;
        }

        /// <summary>
        /// Вычисляет НОК массива чисел.
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static BigInteger LCM(BigInteger[] Source)
        {
            if (Source.Length == 0) return 0;
            if (Source.Length == 1) return Source[0];
            BigInteger Result = LCM(Source[0], Source[1]);
            for (int i = 2; i < Source.Length; i++)
                Result = LCM(Result, Source[i]);
            return Result;
        }

        /// <summary>
        ///     Подсчет наибольшего общего делителя для списка чисел.
        /// </summary>
        /// <param name="Array"></param>
        /// <returns></returns>
        public static BigInteger GCDResult(IList<BigInteger> Array)
        {
            if (Array.Count == 0) return 0;
            if (Array.Count == 1)
                return Array[0];
            while (true)
            {
                if (Array.Count == 1)
                    return Array[0];
                Array[1] = GCDResult(Array[0], Array[1]);
                Array = Array.Skip(1).ToList();
            }
        }

        /// <summary>
        ///     Возвращает наименьшее общее кратное для двух чисел. Вычисляется с помощью НОД.
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static BigInteger LCM(BigInteger First, BigInteger Second)
        {
            BigInteger[] res = GCD(First, Second);
            if (res == null)
                return -1;
            return First * Second / res[0];
        }
    }
}