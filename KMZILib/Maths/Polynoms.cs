﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using static KMZILib.Comparison;

namespace KMZILib
{
    /// <summary>
    ///     Статический класс для работы с многочленами.
    /// </summary>
    public static class Polynoms
    {
        /// <summary>
        ///     Многочлен n-ой степени (Например, 2x^6 - x^5 - 9x^4 + 10x^3 - 11x + 9)
        /// </summary>
        public class Polynom
        {
            /// <summary>
            ///     Массив коэффициентов многочлена. От старших степеней к младшим. Длинными числами могут быть только коэффициенты.
            /// </summary>
            private readonly double[] Coefficients;

            /// <summary>
            ///     Инициализация нового многочлена с помощью готового массива его коэффициентов
            /// </summary>
            public Polynom(IReadOnlyCollection<double> coefficients)
            {
                Coefficients = new double[coefficients.Count];
                coefficients.ToArray().CopyTo(Coefficients, 0);
            }

            /// <summary>
            ///     Инициализация нового многочлена с помощью готового массива его коэффициентов
            /// </summary>
            public Polynom(double[] coefficients)
            {
                Coefficients = new double[coefficients.Length];
                coefficients.CopyTo(Coefficients, 0);
            }

            public Polynom(int[] coefficient) :this(coefficient.Select(val=>(double)val).ToString()){ }

            /// <summary>
            ///     Инициализация нового многочлена с помощью его строкового представления.
            /// </summary>
            /// <param name="polynom"></param>
            public Polynom(string polynom)
            {
                /*Возможные варианты:
                 * C
                 * X
                 * AX
                 * X^B
                 * AX^B
                 * Всего 5 штук
                */
                //Regex PolynomRegex = new Regex(@"((?<value>[1-9][0-9]*)?(?=x\s*\^\s*(?<degree>[1-9][0-9]*)))|((?<value>[1-9][0-9]*)(?=x\s*\^\s*(?<degree>[1-9][0-9]*))?)|((?<value>[1-9][0-9]*)x?)");


                Regex PolynomRegex =
                    new Regex(
                        @"(\s*(?<sign>[+-])?\s*(?<value>\s*[1-9][0-9]*)?\s*x\s*(\^\s*(?<degree>[1-9][0-9]*))?)|((?<sign>-?)\s*(?<value>[1-9][0-9]*))");
                List<Nom> noms = new List<Nom>();
                foreach (Match polynommatch in PolynomRegex.Matches(polynom))
                {
                    int sign = polynommatch.Groups["sign"].Value.Contains("-") ? -1 : 1;
                    double value = polynommatch.Groups["value"].Value != ""
                        ? double.Parse(polynommatch.Groups["value"].Value)
                        : 1;
                    int degree = polynommatch.Groups["degree"].Value != ""
                        ? Convert.ToInt32(polynommatch.Groups["degree"].Value)
                        : polynommatch.Value.Contains('x')
                            ? 1
                            : 0;
                    //опеределили знак. степень и значние 
                    //заносим в таблицу
                    noms.Add(new Nom(sign, value, degree));
                }



                Coefficients = new double[noms.Max(nom => nom.Degree) + 1];

                foreach (Nom nom in noms)
                    Coefficients[Coefficients.Length - 1 - nom.Degree] = nom.Value * nom.Sign;
            }

            /// <summary>
            /// Инициализирует нулевой многочлен заданной степени
            /// </summary>
            /// <param name="value"></param>
            private Polynom(int value)
            {
                Coefficients = new double[value+1];
            }

            /// <summary>
            ///     Степень данного многочлена.
            /// </summary>
            public int Degree => Coefficients.Length - 1;

            /// <summary>
            ///     Вычисление значения многочлена при заданном x. Используется схема Горнера.
            /// </summary>
            /// <param name="x">Точка, в которой нужно вычислить многочлен</param>
            /// <returns>Значение многочлена в точке x</returns>
            public double GetValue(double x)
            {
                return GetValueArray(x).Last();
            }

            /// <summary>
            ///     Вычисление значения многочлена при заданном x. Возвращается вся получившаяся строка значений. Используется схема
            ///     Горнера.
            /// </summary>
            /// <param name="x">Точка, в которой нужно вычислить многочлен</param>
            /// <returns>Значение многочлена в точке x</returns>
            public double[] GetValueArray(double x)
            {
                double[] ResultArray = new double[Coefficients.Length];
                ResultArray[0] = Coefficients[0];
                for (int i = 1; i < Coefficients.Length; i++)
                    ResultArray[i] = ResultArray[i - 1] * x + Coefficients[i];
                return ResultArray;
            }

            /// <summary>
            ///     Вычисление значения многочлена при заданном x в поле Zn. Используется схема Горнера.
            /// </summary>
            /// <param name="x">Точка, в которой нужно вычислить многочлен</param>
            /// <param name="module">Модуль, по которому происходит вычисление</param>
            /// <returns></returns>
            public double GetValue(int x, int module)
            {
                return GetValueArray(x, module).Last();
            }

            /// <summary>
            ///     Вычисление значения многочлена при заданном x в поле Zn. Возвращается вся получившаяся строка значений.
            ///     Используется схема Горнера.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="module"></param>
            /// <returns></returns>
            public int[] GetValueArray(int x, int module)
            {
                LinearComparison[] ResultArray = new LinearComparison[Coefficients.Length];
                ResultArray[0] = new LinearComparison((int)Coefficients[0], module);
                for (int i = 1; i < Coefficients.Length; i++)
                    ResultArray[i] = new LinearComparison((int)(ResultArray[i - 1].A * x + (int)Coefficients[i]), module);
                return ResultArray.Select(comparison => (int)comparison.A).ToArray();
            }

            /// <summary>
            ///     Вычисление корней данного многочлена по заданному модулю.
            /// </summary>
            /// <param name="module">Модуль, по которому происходит нахожлдение корней.</param>
            /// <returns>Пары "корень-кратность"</returns>
            public Dictionary<int,int> SolveResults(int module)
            {
                Dictionary<int,int> Roots = new Dictionary<int,int>();
                Stack<int[]> rows = new Stack<int[]>();
                rows.Push(Coefficients.Select(coef=>(int)coef).ToArray());
                //поместили изначальный массив коэффициентов 
                for (int i = 0; i < module; i++)
                {
                    Polynom buffer = new Polynom(rows.Peek());
                    int[] CurrentRow = buffer.GetValueArray(i, module);
                    if (CurrentRow.Last() != 0)
                        continue;

                    int[] bufferarray = new int[CurrentRow.Length - 1];
                    Array.Copy(CurrentRow, bufferarray, bufferarray.Length);
                    rows.Push(bufferarray);
                    if (Roots.ContainsKey(i))
                        Roots[i]++;
                    else
                        Roots.Add(i, 1);
                    i--;
                }

                return Roots;
            }

            /// <summary>
            ///     Метод, осуществляющий перевод многочлена в строковый вид
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                StringBuilder answer = new StringBuilder();
                for (int i = 0; i < Coefficients.Length; i++)
                {
                    if (Coefficients[i] == 0)
                        continue;
                    if (i <= Coefficients.Length - 2)
                    {
                        answer.Append(
                            $"{(Coefficients[i] > 0 ? Coefficients[i] == 1 ? i == 0 ? "" : " + " : $"{(i == 0 ? "" : " + ") + Coefficients[i]}" : $"{(i == 0 ? "-" : " - ")}{(Math.Abs(Coefficients[i]) == 1 ? "" : Math.Abs(Coefficients[i]).ToString())}")}x");
                        if (i < Coefficients.Length - 2)
                            answer.Append($"^{Coefficients.Length - i - 1}");
                    }
                    else
                    {
                        answer.Append(Coefficients[i] > 0
                            ? $"{(i == 0 ? "" : " + ") + Coefficients[i]}"
                            : $" - {Math.Abs(Coefficients[i])}");
                    }
                }

                return answer.ToString();
            }

            /// <summary>
            /// Возвращает результат сложения двух многочленов.
            /// </summary>
            /// <param name="First"></param>
            /// <param name="Second"></param>
            /// <returns></returns>
            public static Polynom operator +(Polynom First, Polynom Second)
            {
                double[] MinArray = First.Coefficients.Length <= Second.Coefficients.Length
                    ? First.Coefficients
                    : Second.Coefficients;
                double[] MaxArray = First.Coefficients.Length > Second.Coefficients.Length
                    ? First.Coefficients
                    : Second.Coefficients;
                int offset = MaxArray.Length - MinArray.Length;
                double[] Result = new double[MaxArray.Length];
                for (int i = 0; i < offset; i++)
                    Result[i] = MaxArray[i];
                for (int i = offset; i < Result.Length; i++)
                    Result[i] = MaxArray[i] + MinArray[i - offset];
                return Result.Any(val => val != 0) ? new Polynom(Result.SkipWhile(num => num == 0).ToArray()) : new Polynom(new[] { (double)0 });
            }

            /// <summary>
            /// Возвращает результат вычитания одного многочлена из другого.
            /// </summary>
            /// <param name="First"></param>
            /// <param name="Second"></param>
            /// <returns></returns>
            public static Polynom operator -(Polynom First, Polynom Second)
            {
                return First + -Second;
            }

            /// <summary>
            /// Возвращает степень коэффициента с заданным индексом.
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            private int GetCoefDegree(int index)
            {
                return Coefficients.Length - 1 - index;
            }

            /// <summary>
            /// Возвращает индекс коэффициента с заданной степенью.
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            private int GetCoefIndex(int degree)
            {
                return Coefficients.Length - 1 - degree;
            }

            /// <summary>
            /// Возвращает результат умножения всех коэффициентов многочлена на заданное число.
            /// </summary>
            /// <param name="First"></param>
            /// <param name="Second"></param>
            /// <returns></returns>
            public static Polynom operator *(Polynom First, double Second)
            {
                return new Polynom(First.Coefficients.Select(val => val * Second).ToArray());
            }

            /// <summary>
            /// Возвращает результат умножения всех коэффициентов многочлена на заданное число.
            /// </summary>
            /// <param name="First"></param>
            /// <param name="Second"></param>
            /// <returns></returns>
            public static Polynom operator *(Polynom First, int Second)
            {
                return First * (double)Second;
            }

            public static Polynom operator *(Polynom First, Polynom Second)
            {
                List<Polynom> summaryarray = new List<Polynom>();
                for (int i = 0; i <= First.Degree; i++)
                {
                    int FirstDegree = First.GetCoefDegree(i);
                    if(First[i]==0) continue;
                    summaryarray.Add(new Polynom(FirstDegree + Second.Degree));
                    for (int j = 0; j <= Second.Degree; j++)
                    {
                        int SecondDegree = Second.GetCoefDegree(j);
                        summaryarray[summaryarray.Count-1][summaryarray[summaryarray.Count - 1].GetCoefIndex(FirstDegree + SecondDegree)] = First[i] * Second[j];
                    }
                }
                Polynom Result = new Polynom(summaryarray.Select(pol=>pol.Degree).Max());
                Result = summaryarray.Aggregate(Result, (Current, part) => Current + part);
                return Result;
            }

            public double this[int Index]
            {
                get => Coefficients[Index];
                set => Coefficients[Index] = value;
            }

            /// <summary>
            /// Приводит число в вид многочлена нулевой степени.
            /// </summary>
            /// <param name="Value"></param>
            public static implicit operator Polynom(int Value)
            {
                return new Polynom(new double[] { Value });
            }

            /// <summary>
            /// Приводит число в вид многочлена нулевой степени.
            /// </summary>
            /// <param name="Value"></param>
            public static implicit operator Polynom(double Value)
            {
                return new Polynom(new double[] { Value });
            }

            /// <summary>
            /// Инвертирует значения коэффициентов данного многочлена.
            /// </summary>
            /// <param name="Source"></param>
            /// <returns></returns>
            public static Polynom operator -(Polynom Source)
            {
                return new Polynom(Source.Coefficients.Select(coef => -coef).ToArray());
            }

            /// <summary>
            ///     Класс, представляющий собой одночлен. Нужен для парсинга строкового представления многочлена.
            /// </summary>
            private class Nom
            {
                /// <summary>
                ///     Степень одночлена (x^y - y-степень)
                /// </summary>
                public readonly int Degree;

                /// <summary>
                ///     Знак одночлена
                /// </summary>
                public readonly int Sign;

                /// <summary>
                ///     Значение одночлена по модулю
                /// </summary>
                public readonly double Value;

                /// <summary>
                ///     Конструктор одночлена, использующий все имеющиеся поля
                /// </summary>
                /// <param name="sign">Знак одночлена (-1/0/1)</param>
                /// <param name="value">Значение одночлена. Целое число</param>
                /// <param name="degree">Степень одночлена. Целое неотрицательное число</param>
                public Nom(int sign, double value, int degree)
                {
                    Sign = sign;
                    Value = value;
                    Degree = degree;
                }
            }
        }
    }
}