using KMZILib;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Ciphers = KMZILib.Ciphers;
using Languages = KMZILib.Ciphers.Languages;
using Polynoms = KMZILib.Polynoms;
using ModularPolynom=KMZILib.Polynoms.ModularPolynom;

namespace Ручной_тест
{
    class Program
    {
        
        static void Main(string[] args)
        {
            string test_str = "Съешь же ещё этих мягких французских булочек да выпей чаю.";
            KeyValuePair<char, double>[] Stats = GetStatisticOnegram(test_str, false);
            CodingTheory.ByteSet[] res = CodingTheory.DataCompressionCodes.GilbertCoding.GetCodes(Stats.Select(row => row.Value).ToArray(), out double AverageLength);
            Console.WriteLine();
        }


        public static KeyValuePair<char, double>[] GetStatisticOnegram(string SourceText, bool sort = true)
        {
            SourceText = SourceText.ToUpper();
            Dictionary<char, double> Result = new Dictionary<char, double>();
            foreach (char symbol in SourceText)
            {
                if (!Result.ContainsKey(symbol))
                    Result.Add(symbol, 0.0);

                Result[symbol]++;
            }

            double Sum = Result.Values.Sum();
            foreach (char resultKey in Result.Keys.ToList())
                Result[resultKey] /= Sum;
            return sort ? Result.OrderByDescending(a => a.Value).ToArray() : Result.ToArray();
        }

    }
}


