using System;
using System.Collections.Generic;
using System.Linq;

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
        ///     возрастания.
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
        public static int[] UniformDistribution(int Min, int Max, int Count = -1)
        {
            SortedSet<int> numbersenum = new SortedSet<int>();
            if (Count == -1)
                Count = Max - Min + 1;
            if (Count > Max - Min + 1)
                throw new ArgumentException("Число переменных не может превышать длину диапазона", nameof(Count));

            if (Count < (Max - Min + 1) / 2)
            {
                while (numbersenum.Count != Count)
                    numbersenum.Add(Rand.Next(Min, Max + 1));
            }
            else
            {
                List<int> buffer = new List<int>();
                for (int i = Min; i <= Max; i++)
                    buffer.Add(i);
                while (buffer.Count != Count)
                    buffer.RemoveAt(Rand.Next(numbersenum.Count));
                numbersenum = new SortedSet<int>(buffer);
            }


            return numbersenum.ToArray();
        }
    }
}