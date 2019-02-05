using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace KMZILib
{
    /// <summary>
    ///     Random Distribution - класс, отвечающий за генерацию случайных наборов чисел
    /// </summary>
    public static class RD
    {
        /// <summary>
        ///     Статический генератор случайных чисел.
        /// </summary>
        public static readonly Random Rand = new Random(DateTime.Now.Millisecond);

        /// <summary>
        ///     Генерирует равномерно распределенные случайные числа в указанном размере и диапазоне. Без повторений. В порядке
        ///     возрастания. Если в количесто послать 0, выведет весь диапазон.
        /// </summary>
        /// <param name="Count">
        ///     Число генерируемых чисел. Если опускается, возвращается весь диапазон от нижнейграницы до верхней
        ///     включительно.
        /// </param>
        /// <param name="Min">Нижняя граница. Включается в диапазон</param>
        /// <param name="Max">Верхняя граница. Включается в диапазон</param>
        /// <returns>Массив сгенерированных случайных чисел в заданном отрезке.</returns>
        /// <exception cref="InvalidOperationException">
        ///     Число генерируемых чисел в диапазоне не может быть больше длины самого
        ///     диапазона.
        /// </exception>
        public static BigInteger[] UniformDistribution(BigInteger Min, BigInteger Max, BigInteger Count)
        {
            SortedSet<BigInteger> numbersenum = new SortedSet<BigInteger>();
            if (Count == 0)
                Count = Max - Min + 1;
            if (Count > Max - Min + 1)
                throw new ArgumentException("Число переменных не может превышать длину диапазона", nameof(Count));

            if (Count < (Max - Min + 1) / 2)
            {
                while (numbersenum.Count != Count)
                {
                    BigInteger MinCopy = Min;
                    BigInteger MaxCopy = Max;

                    while (MaxCopy - MinCopy > 1)
                    {
                        if (Rand.Next(2) == 0)
                            MaxCopy = (MaxCopy - MinCopy + 1) / 2;
                        else
                            MinCopy = (MaxCopy - MinCopy + 1) / 2 + (MaxCopy - MinCopy + 1) % 2;
                    }

                    BigInteger randomResult = Rand.Next(2) == 0 ? MinCopy : MaxCopy;
                    if(!numbersenum.Contains(randomResult))
                        numbersenum.Add(randomResult);
                }
            }
            else
            {
                List<BigInteger> buffer = new List<BigInteger>();
                for (BigInteger i = Min; i <= Max; i++)
                    buffer.Add(i);
                while (buffer.Count != Count)
                    buffer.RemoveAt(Rand.Next(numbersenum.Count));
                numbersenum = new SortedSet<BigInteger>(buffer);
            }
            return numbersenum.ToArray();
        }
    }
}