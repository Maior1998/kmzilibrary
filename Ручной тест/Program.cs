using KMZILib;

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Ручной_тест
{
    class Program
    {
        
        static void Main(string[] args)
        {
            Matrix a = new Matrix(new double[][]{new []{3.0,4,-2}, new[] {-2, 1.0, 0 }, new[] { 2, 3.0, 0 } });
            Console.WriteLine(a.Reverse);

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
