using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMZILib.Ciphers
{
    ////DKS - double key swap
    public static class DKS
    {

        public static string Encode(string source, string key1, string key2)
        {
            source += string.Concat(Enumerable.Repeat(' ', Math.Max(0, key1.Length * key2.Length - source.Length)));
            int rowscount = source.Length / key1.Length;
            int columnscount = source.Length / key2.Length;
            KeyValuePair<int, char>[] trans2 =
                key1
                    .Select((elem, ind) => new KeyValuePair<int, char>(ind, elem))
                    .OrderBy(pair => pair.Value).ToArray();
            KeyValuePair<int, char>[] trans1 =
                key2
                    .Select((elem, ind) => new KeyValuePair<int, char>(ind, elem))
                    .OrderBy(pair => pair.Value).ToArray();
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < rowscount; i++)
            {
                result.Append(string.Concat(Enumerable.Repeat(' ', key1.Length)));
                for (int j = 0; j < columnscount; j++)
                {
                    result[columnscount * i + j] = source[key1.Length * trans1[i].Key + trans2[j].Key];
                }
            }

            return result.ToString();
        }


        public static string Decode(string source, string key1, string key2)
        {
            int RowCount = source.Length / key1.Length;
            KeyValuePair<int, int>[] rows =
                key1
                    .Select((elem, ind) => new KeyValuePair<int, char>(ind, elem))
                    .OrderBy(pair => pair.Value)
                    .Select((pair, ind) => new KeyValuePair<int, int>(pair.Key, ind))
                    .OrderBy(pair => pair.Key)
                    .ToArray();
            int ColumnsCount = source.Length / key2.Length;
            StringBuilder result = new StringBuilder();
            KeyValuePair<int, int>[] columns =
                key2
                    .Select((elem, ind) => new KeyValuePair<int, char>(ind, elem))
                    .OrderBy(pair => pair.Value)
                    .Select((pair, ind) => new KeyValuePair<int, int>(pair.Key, ind))
                    .OrderBy(pair => pair.Key)
                    .ToArray();
            for (int i = 0; i < ColumnsCount; i++)
                for (int j = 0; j < RowCount; j++)
                    result.Append(source[rows[j].Value * key2.Length + columns[i].Value]);
            return result.ToString().TrimEnd();
        }


    }
}
