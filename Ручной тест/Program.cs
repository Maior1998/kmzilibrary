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
using DataCompressionCodes=KMZILib.CodingTheory.DataCompressionCodes;

namespace Ручной_тест
{
    class Program
    {
        
        static void Main(string[] args)
        {
            Console.OutputEncoding=Encoding.Default;
            Console.WriteLine(DataCompressionCodes.ShannonCoding.GetL(0.0217391304347826));
        }


        public static KeyValuePair<char, double>[] GetStatisticOnegram(string SourceText, bool sort)
        {
            SourceText = SourceText.ToUpper();
            Dictionary<char, double> Result = new Dictionary<char, double>();
            foreach (char symbol in SourceText)
            {
                if (!Result.ContainsKey(symbol))
                {
                    Result.Add(symbol, 0.0);
                }

                Result[symbol]++;
            }

            double Sum = Result.Values.Sum();
            foreach (char resultKey in Result.Keys.ToList())
                Result[resultKey] /= Sum;
            return sort ? Result.OrderByDescending(a => a.Value).ToArray() : Result.ToArray();
        }
    }
}


