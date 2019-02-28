using KMZILib;

using System;
using System.IO;
using System.Linq;
using System.Text;
using Ciphers = KMZILib.Ciphers;

namespace Ручной_тест
{
    class Program
    {

        static void Main(string[] args)
        {
            CRS.LFSR test = new CRS.LFSR(@"an+2=an+1 + 2an",new []{0,1});
            Console.WriteLine(test);
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(test.CurrentState);
                test.GetNext();
            }
            return;
            Ciphers.Languages.CurrentLanguage = Ciphers.Languages.RussianLanguage.GetInstanse();

            for (int i = 0; i < 33; i++)
            {
                char f1 = Ciphers.Languages.CurrentLanguage.Alphabet[i];
                char f2 = Ciphers.CaesarsCipher.Encrypt($"{f1}", 6)[0];
                char f3 = Ciphers.CaesarsCipher.Encrypt($"{f1}", 15)[0];
                char f4 = Ciphers.CaesarsCipher.Encrypt($"{f1}", 12)[0];
                char f5 = Ciphers.CaesarsCipher.Encrypt($"{f1}", 22)[0];
                Console.WriteLine($"{f1}{f2}{f3}{f4}{f5}");
            }

            
            for (int i = 0; i < Ciphers.Languages.RussianLanguage.GetInstanse().Alphabet.Length; i++)
            {
                Console.WriteLine(Ciphers.CaesarsCipher.Encrypt(Ciphers.Languages.RussianLanguage.GetInstanse().Alphabet, i));
            }

            Console.WriteLine(LinearEquations.GaussMethod.Solve(new Matrix(new[]
            {
                new[] {1.0, 0, 0, -1, 4},new[] {-1, 1, 0, 0.0, 6} , new[] {0.0, 1, -1, 0, 17},new[] {0.0, 0, 1, -1, 18}
            })));

            Console.WriteLine(Ciphers.Languages.RussianLanguage.GetInstanse().Alphabet.IndexOf("Т"));
            Console.WriteLine("\n\n");
            Console.WriteLine(Ciphers.VigenereCipher.Encrypt("АРОЗАУПАЛАНАЛАПУАЗОРААРОЗАУПАЛАНАЛАПУАЗОРААРОЗАУПАЛАНАЛАПУАЗОРААРОЗАУПАЛАНАЛАПУАЗОРА","ГИРЯ"));


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
