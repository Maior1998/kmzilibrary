using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using static KMZILib.Comparison;

namespace KMZILib
{
    /// <summary>
    ///     Статический класс для работы с многочленами
    /// </summary>
    public static class Polynoms
    {
        /// <summary>
        ///     Многочлен n-ой степени (Например, 2x^6 - x^5 - 9x^4 + 10x^3 - 11x + 9)
        /// </summary>
        public class Polynom
        {
            /// <summary>
            ///     Массив коэффициентов многочлена. От старших степеней к младшим.
            /// </summary>
            private readonly BigInteger[] Coefficients;

            /// <summary>
            ///     Инициализация нового многочлена с помощью готового массива его коэффициентов
            /// </summary>
            public Polynom(IReadOnlyCollection<int> coefficients)
            {
                Coefficients = new BigInteger[coefficients.Count];
                coefficients.Select(cod => new BigInteger(cod)).ToArray().CopyTo(Coefficients, 0);
            }

            /// <summary>
            ///     Инициализация нового многочлена с помощью готового массива его коэффициентов
            /// </summary>
            public Polynom(BigInteger[] coefficients)
            {
                Coefficients = new BigInteger[coefficients.Length];
                coefficients.CopyTo(Coefficients, 0);
            }

            /// <summary>
            ///     Инициализация нового многочлена с помощью его строкового представления
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
                    int value = polynommatch.Groups["value"].Value != ""
                        ? Convert.ToInt32(polynommatch.Groups["value"].Value)
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

                Coefficients = new BigInteger[noms.Max(nom => nom.Degree) + 1];

                foreach (Nom nom in noms)
                    Coefficients[Coefficients.Length - 1 - nom.Degree] = nom.Value * nom.Sign;
            }

            /// <summary>
            ///     Степень данного многочлена
            /// </summary>
            public int Degree => Coefficients.Length - 1;

            /// <summary>
            ///     Вычисление значения многочлена при заданном x. Используется схема Горнера.
            /// </summary>
            /// <param name="x">Точка, в которой нужно вычислить многочлен</param>
            /// <returns>Значение многочлена в точке x</returns>
            public BigInteger GetValue(int x)
            {
                return GetValueArray(x).Last();
            }

            /// <summary>
            ///     Вычисление значения многочлена при заданном x. Возвращается вся получившаяся строка значений. Используется схема
            ///     Горнера.
            /// </summary>
            /// <param name="x">Точка, в которой нужно вычислить многочлен</param>
            /// <returns>Значение многочлена в точке x</returns>
            public BigInteger[] GetValueArray(int x)
            {
                BigInteger[] ResultArray = new BigInteger[Coefficients.Length];
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
            public BigInteger GetValue(BigInteger x, BigInteger module)
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
            public BigInteger[] GetValueArray(BigInteger x, BigInteger module)
            {
                LinearComparison[] ResultArray = new LinearComparison[Coefficients.Length];
                ResultArray[0] = new LinearComparison(Coefficients[0], module);
                for (int i = 1; i < Coefficients.Length; i++)
                    ResultArray[i] = new LinearComparison(ResultArray[i - 1].A * x + Coefficients[i], module);
                return ResultArray.Select(comparison => comparison.A).ToArray();
            }

            /// <summary>
            ///     Вычисление корней данного многочлена по заданному модулю.
            /// </summary>
            /// <param name="module">Модуль, по которому происходит нахожлдение корней.</param>
            /// <returns>Пары "корень-кратность"</returns>
            public Dictionary<int, int> SolveResults(int module)
            {
                Dictionary<int, int> Roots = new Dictionary<int, int>();
                Stack<BigInteger[]> rows = new Stack<BigInteger[]>();
                rows.Push(Coefficients.ToArray());
                //поместили изначальный массив коэффициентов 
                for (int i = 0; i < module; i++)
                {
                    Polynom buffer = new Polynom(rows.Peek());
                    BigInteger[] CurrentRow = buffer.GetValueArray(i, module);
                    if (CurrentRow.Last() != 0) continue;

                    BigInteger[] bufferarray = new BigInteger[CurrentRow.Length - 1];
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
                    if (Coefficients[i] == 0) continue;
                    if (i <= Coefficients.Length - 2)
                    {
                        answer.Append(
                            $"{(Coefficients[i] > 0 ? Coefficients[i] == 1 ? i == 0 ? "" : " + " : $"{(i == 0 ? "" : " + ") + Coefficients[i]}" : $"{(i == 0 ? "-" : " - ")}{(BigInteger.Abs(Coefficients[i]) == 1 ? "" : BigInteger.Abs(Coefficients[i]).ToString())}")}x");
                        if (i < Coefficients.Length - 2)
                            answer.Append($"^{Coefficients.Length - i - 1}");
                    }
                    else
                    {
                        answer.Append(Coefficients[i] > 0
                            ? $"{(i == 0 ? "" : " + ") + Coefficients[i]}"
                            : $" - {BigInteger.Abs(Coefficients[i])}");
                    }
                }

                return answer.ToString();
            }

            /// <summary>
            ///     Класс, представляющий собой одночлен
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
                public readonly int Value;

                /// <summary>
                ///     Конструктор одночлена, использующий все имеющиеся поля
                /// </summary>
                /// <param name="sign">Знак одночлена (-1/0/1)</param>
                /// <param name="value">Значение одночлена. Целое число</param>
                /// <param name="degree">Степень одночлена. Целое неотрицательное число</param>
                public Nom(int sign, int value, int degree)
                {
                    Sign = sign;
                    Value = value;
                    Degree = degree;
                }
            }
        }
    }
}