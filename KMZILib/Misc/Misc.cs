using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace KMZILib
{
    /// <summary>
    /// Разная всячина. Полезные функции и методы.
    /// </summary>
    public static partial class Misc
    {
        /// <summary>
        /// Возвращает дробную часть вещественного числа в двоичном представлении.
        /// </summary>
        /// <param name="Source">Вещественное число.</param>
        /// <param name="Length">Необходимое число знаков после запятой.</param>
        /// <returns></returns>
        public static string DoubleFractToString(double Source, int Length)
        {
            Source -= (int)Source;
            StringBuilder Result = new StringBuilder();
            while (Result.Length < Length)
            {
                Source *= 2;
                if (Source>1-Math.Pow(10,-4))
                {
                    Source -= 1;
                    Result.Append('1');
                }
                else
                {
                    Result.Append('0');
                }
            }
            //Если набрали меньше символов, чем в заданной длине, то дополняем до неё.
            if (Result.Length < Length)
                Result.Append(new char[Length - Result.Length].Select(ch => '0').ToArray());

            //отдаем ответ.
            return Result.ToString();
        }

        /// <summary>
        /// Переводит дробное двочисное число из интервала (0;1) в десятичное представление.
        /// </summary>
        /// <param name="Source">Строка, содержащая цифры после запятой исходного числа.</param>
        /// <returns></returns>
        public static double BinaryStringToFract(string Source)
        {
            double Result = 0;
            int FractIndex = -1;
            for (int i = 0; i < Source.Length; i++, FractIndex--)
                Result += (Source[i] == '1' ? 1 : 0) * Math.Pow(2, FractIndex);
            return Result;
        }

        public static string GetSignedValue(double Source, int CountOfDigits=-1)
        {
            return $" {(Source >= 0 ? "+" : "-")} {(CountOfDigits==-1? Math.Abs(Source): Math.Abs(Math.Round(Source,CountOfDigits)))}";
        }

        public static string GetSignedValue(int Source)
        {
            return $" {(Source >= 0 ? "+" : "-")} {Math.Abs(Source)}";
        }

        public static int GetValue(bool[] Set)
        {
            int n = 0;
            for (int i = 0; i < Set.Length; i++)
                n |= (Set[i] ? 1 : 0) ;
            return n;
        }
        /// <summary>
        ///     Возвращает значение целого числа в виде массива двоичных значений в указанном количество от его начала
        ///     (младших бит)
        /// </summary>
        /// <param name="Number">Число, которое необходимо перевести в двоичную систему счисления</param>
        /// <param name="Count">Число разрядов. Отсчет ведется от младших к старщим</param>
        /// <returns>Массив двоичных значение - двоичное представление числа</returns>
        public static bool[] GetBinaryArray(BigInteger Number, int Count)
        {
            if (Number < 0)
                throw new InvalidOperationException("Число должно быть положительным.");
            BitArray number = new BitArray(Number.ToByteArray());
            bool[] buffer = number.Cast<bool>().Reverse().SkipWhile(val => !val).ToArray();
            bool[] Result = new bool[Count];
            Array.Copy(buffer, 0, Result, Count < buffer.Length ? 0 : Count - buffer.Length,
                Math.Min(Count, buffer.Length));
            return Result.ToArray();
        }

        /// <summary>
        ///     Возвращает значение целого числа в виде массива двоичных значений
        /// </summary>
        /// <param name="Number">Число, которое необходимо перевести в двоичную систему счисления</param>
        /// <returns>Массив двоичных значение - двоичное представление числа</returns>
        public static bool[] GetBinaryArray(BigInteger Number)
        {
            if (Number < 0)
                throw new InvalidOperationException("Число должно быть положительным.");
            if (Number == 0) return new[] {false};
            BitArray number = new BitArray(Number.ToByteArray());

            int i = number.Count - 1;
            while (i >= 0 && !number[i])
                i--;
            bool[] Result = new bool[i + 1];
            for (int k = 0; i >= 0; i--, k++)
                Result[k] = number[i];
            return Result;
        }

        /// <summary>
        ///     Возвращает бинарное представление целого числа.
        ///     (младших бит)
        /// </summary>
        /// <param name="Number">Число, которое необходимо перевести в двоичную систему счисления</param>
        /// <returns>Строка - двоичное представление числа</returns>
        public static string GetBinaryString(BigInteger Number)
        {
            return String.Concat(GetBinaryArray(Number).Select(val => val ? '1' : '0'));
        }

        /// <summary>
        ///     Возвращает бинарное представление целого числа с заданным числом разрядов.
        ///     (младших бит)
        /// </summary>
        /// <param name="Number">Число, которое необходимо перевести в двоичную систему счисления</param>
        /// <param name="Count">Число разрядов. Отсчет ведется от младших к старщим</param>
        /// <returns>Строка - двоичное представление числа</returns>
        public static string GetBinaryString(BigInteger Number, int Count)
        {
            return String.Concat(GetBinaryArray(Number, Count).Select(val => val ? '1' : '0'));
        }
    }
}
