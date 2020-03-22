using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KMZILib
{
    public partial class Ciphers
    {
        //SIC - Simple Permutation Cipher
        public static class SIC
        {
            public static string Encode(string source, int[] key)
            {
                source += string.Concat(Enumerable.Repeat(" ", (key.Length - source.Length % key.Length) % key.Length));

                StringBuilder result = new StringBuilder();
                for (int i = 0; i < source.Length / key.Length; i++)
                {
                    int multiplier = i;
                    result.Append(string.Concat(key.Select(elem => source[(elem - 1) + key.Length * multiplier])));
                }

                return result.ToString();
            }

            public static string Decode(string source, int[] key)
            {
                StringBuilder result = new StringBuilder(string.Concat(Enumerable.Repeat(' ',source.Length)));
                for (int i = 0; i < source.Length / key.Length; i++)
                    for (int j = 0; j < key.Length; j++)
                        result[key[j]+i*key.Length-1] = source[j+i*key.Length];
                return result.ToString().TrimEnd();
            }
        }
    }
}
