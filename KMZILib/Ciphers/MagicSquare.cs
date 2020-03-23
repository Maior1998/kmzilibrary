using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KMZILib.Ciphers
{
    public static class MagicSquare
    {
        public static string Encode(string source, int[] key)
        {
            int n = 1;
            while (n * n < key.Length) n++;
            if (n * n != key.Length) throw new InvalidOperationException("Ключ должен быть квадратом!");
            int magicConst = (n * (n * n + 1)) / 2;
            int mainDiagSum = 0;
            int secDiagSum = 0;
            for (int i = 0; i < n; i++)
            {
                int currColumnSum = 0;
                int currRowSum = 0;
                mainDiagSum += key[i * n + i];
                secDiagSum += key[i * n + n - i - 1];
                for (int j = 0; j < n; j++)
                {
                    currRowSum += key[i * n + j];
                    currColumnSum += key[i + j * n];
                }
                if (currRowSum != magicConst || currColumnSum != magicConst)
                    throw new InvalidOperationException("Суммы чисел по строкам, столбцам и диагоналям должны совпадать!");
            }
            if (mainDiagSum != magicConst || secDiagSum != magicConst)
                throw new InvalidOperationException("Суммы чисел по строкам, столбцам и диагоналям должны совпадать!");
            return SIC.Encode(source, key);
        }

        public static string Decode(string source, int[] key)
        {
            int n = 1;
            while (n * n < key.Length) n++;
            if (n * n != key.Length) throw new InvalidOperationException("Ключ должен быть квадратом!");
            int magicConst = (n * (n * n + 1)) / 2;
            int mainDiagSum = 0;
            int secDiagSum = 0;
            for (int i = 0; i < n; i++)
            {
                int currColumnSum = 0;
                int currRowSum = 0;
                mainDiagSum += key[i * n + i];
                secDiagSum += key[i * n + n - i - 1];
                for (int j = 0; j < n; j++)
                {
                    currRowSum += key[i * n + j];
                    currColumnSum += key[i + j * n];
                }
                if (currRowSum != magicConst || currColumnSum != magicConst)
                    throw new InvalidOperationException("Суммы чисел по строкам, столбцам и диагоналям должны совпадать!");

            }
            if (mainDiagSum != magicConst || secDiagSum != magicConst)
                throw new InvalidOperationException("Суммы чисел по строкам, столбцам и диагоналям должны совпадать!");
            return SIC.Decode(source, key);
        }
    }
}
