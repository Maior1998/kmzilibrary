using KMZILib;

using System;
using System.IO;
using System.Linq;
using System.Text;
using Ciphers = KMZILib.Ciphers;
using Polynoms = KMZILib.Polynoms;

namespace Ручной_тест
{
    class Program
    {

        static void Main(string[] args)
        {
            Polynoms.Polynom test1 = new Polynoms.Polynom(@"3x^3 + 3x^2 + 1");
            Polynoms.Polynom test2 = new Polynoms.Polynom(@"2x-2");
            Console.WriteLine(test1/test2);
            Console.WriteLine();
            return;
            string[] numbs = File.ReadAllLines(@"C:\Users\maior\Downloads\numbers.txt", Encoding.Default);

            double[] Test = numbs.Select(Convert.ToDouble).ToArray();

            MathStatistics.RandomValue value = new MathStatistics.RandomValue(Test);
            Console.WriteLine($"Среднее : {value.Average}\n" +
                              $"Стандартная ошибка : {value.StandartErrorGeneral}\n" +
                              $"Медиана : {value.Median}\n" +
                              $"Мода : {value.Mean}\n" +
                              $"Стандартное отклонение : {value.StandardDeviation}\n" +
                              $"Дисперсия выборки : {value.Dispersion}\n" +
                              $"Эксцесс : {value.Kurtosis}\n" +
                              $"Ассиметричность : {value.Skewness}\n" +
                              $"Интервал : {value.Interval}\n" +
                              $"Минимум : {value.Min}\n" +
                              $"Максимум : {value.Max}\n" +
                              $"Сумма : {value.Sum}\n" +
                              $"Счет : {value.Count}");
        }


    }
}

