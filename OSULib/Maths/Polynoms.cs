using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using static OSULib.Maths.Comparison;

namespace OSULib.Maths
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

            /// <summary>
            /// Инициализирует новый многочлен, который является копией заданного многочлена.
            /// </summary>
            /// <param name="Source"></param>
            public Polynom(Polynom Source)
            {
                Coefficients = new double[Source.Coefficients.Length];
                Source.Coefficients.CopyTo(Coefficients, 0);
            }

            /// <summary>
            /// Инициализирует новый многочлен с заднными целочисленными коэффициентами.
            /// </summary>
            /// <param name="coefficient"></param>
            public Polynom(int[] coefficient) : this(coefficient.Select(val => (double)val).ToString()) { }

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
                        @"(\s*(?<sign>[+-])?\s*(?<value>\s*[0-9]+(?:\.[0-9]+)?)?\s*x\s*(\^\s*(?<degree>[1-9][0-9]*))?)|((?<sign>-?)\s*(?<value>[0-9]+(?:\.[0-9]+)?))");
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
                    //опеределили знак, степень и значние 
                    //заносим в список
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
                if (value < 0) value = 0;
                Coefficients = new double[value + 1];
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
            ///     Метод, осуществляющий перевод многочлена в строковый вид
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                if (Coefficients.Length == 1) return Coefficients.First().ToString();
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
            /// <param name="degree"></param>
            /// <returns></returns>
            private int GetCoefIndex(int degree)
            {
                return Coefficients.Length - 1 - degree;
            }

            /// <summary>
            /// Возвращает производную от данного многочлена.
            /// </summary>
            /// <returns></returns>
            public Polynom Derivative()
            {
                double[] Result = new double[Coefficients.Length-1];
                for (int i = Result.Length - 1; i >= 0; i--)
                    Result[i] = Coefficients[i] * (Degree - i);
                return new Polynom(Result);
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

            /// <summary>
            /// Возвращает результат умножения многочленов.
            /// </summary>
            /// <param name="First"></param>
            /// <param name="Second"></param>
            /// <returns></returns>
            public static Polynom operator *(Polynom First, Polynom Second)
            {
                List<Polynom> summaryarray = new List<Polynom>();
                for (int i = 0; i <= First.Degree; i++)
                {
                    int FirstDegree = First.GetCoefDegree(i);
                    if (First[i] == 0)
                        continue;
                    summaryarray.Add(new Polynom(FirstDegree + Second.Degree));
                    for (int j = 0; j <= Second.Degree; j++)
                    {
                        int SecondDegree = Second.GetCoefDegree(j);
                        summaryarray[summaryarray.Count - 1][summaryarray[summaryarray.Count - 1].GetCoefIndex(FirstDegree + SecondDegree)] = First[i] * Second[j];
                    }
                }
                Polynom Result = new Polynom(summaryarray.Select(pol => pol.Degree).Max());
                Result = summaryarray.Aggregate(Result, (Current, part) => Current + part);
                return Result;
            }

            /// <summary>
            /// Получает доступ к коэффициенту текущего многочлена с заданным индексом.
            /// </summary>
            /// <param name="Index"></param>
            /// <returns></returns>
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
            /// Возвращает неполное частное деления многочленов.
            /// </summary>
            /// <param name="First"></param>
            /// <param name="Second"></param>
            /// <returns></returns>
            public static Polynom operator /(Polynom First, Polynom Second)
            {
                //Создали результат - многочлен нужной степени.
                Polynom Residue = new Polynom(First);
                int i = 0;
                Polynom Result = new Polynom(First.Degree - Second.Degree);
                while (Residue.Degree >= Second.Degree)
                {
                    int index = Result.Coefficients.Length - 1 - (Residue.Degree - Second.Degree);
                    Result[index] = Residue[0] / Second[0];

                    Polynom buffer = new Polynom(Result.Degree);
                    buffer[index] = Result[index];

                    Residue -= buffer*Second;
                    //Произвели вычитание текущей степени. Теперь она обнулена.
                    i++;
                }

                return Result;
            }

            /// <summary>
            /// Возвращает остаток деления многочленов.
            /// </summary>
            /// <param name="First"></param>
            /// <param name="Second"></param>
            /// <returns></returns>
            public static Polynom operator %(Polynom First, Polynom Second)
            {
                //Создали результат - многочлен нужной степени.
                Polynom Residue = new Polynom(First);
                int i = 0;
                Polynom Result = new Polynom(First.Degree - Second.Degree);
                while (Residue.Degree >= Second.Degree)
                {
                    int index = Result.Coefficients.Length - 1 - (Residue.Degree - Second.Degree);
                    Result[index] = Residue[0] / Second[0];

                    Polynom buffer = new Polynom(Result.Degree);
                    buffer[index] = Result[index];

                    Residue -= buffer * Second;
                    //Произвели вычитание текущей степени. Теперь она обнулена.
                    i++;
                }

                return Residue;
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

        /// <summary>
        ///     Многочлен n-ой степени (Например, 2x^6 - x^5 - 9x^4 + 10x^3 - 11x + 9)
        /// </summary>
        public class ModularPolynom
        {
            //TODO: унаследовать от простого многочлена?
            /// <summary>
            ///     Массив коэффициентов многочлена. От старших степеней к младшим. Длинными числами могут быть только коэффициенты.
            /// </summary>
            private readonly Comparison.LinearComparison[] Coefficients;

            /// <summary>
            /// Модуль данного многочлена.
            /// </summary>
            public readonly int Module;

            /// <summary>
            ///     Инициализация нового многочлена с помощью готового массива его коэффициентов
            /// </summary>
            public ModularPolynom(IReadOnlyCollection<int> coefficients,int module)
            {
                Module = module;
                Coefficients = coefficients.Select(elem=>new Comparison.LinearComparison(elem,module)).ToArray();
            }
            
            /// <summary>
            /// Инициализирует новый многочлен, который является копией заданного многочлена.
            /// </summary>
            /// <param name="Source"></param>
            public ModularPolynom(ModularPolynom Source)
            {
                Module = Source.Module;
                Coefficients = Source.Coefficients.Select(comp=>(int)comp.A).Select(elem => new Comparison.LinearComparison(elem, Module)).ToArray();
            }

            /// <summary>
            ///     Инициализация нового многочлена с помощью его строкового представления.
            /// </summary>
            /// <param name="ModularPolynom"></param>
            /// <param name="module"></param>
            public ModularPolynom(string ModularPolynom,int module)
            {
                Module = module;
                /*Возможные варианты:
                 * C
                 * X
                 * AX
                 * X^B
                 * AX^B
                 * Всего 5 штук
                */
                //Regex ModularPolynomRegex = new Regex(@"((?<value>[1-9][0-9]*)?(?=x\s*\^\s*(?<degree>[1-9][0-9]*)))|((?<value>[1-9][0-9]*)(?=x\s*\^\s*(?<degree>[1-9][0-9]*))?)|((?<value>[1-9][0-9]*)x?)");


                Regex ModularPolynomRegex =
                    new Regex(
                        @"(\s*(?<sign>[+-])?\s*(?<value>\s*[1-9][0-9]*)?\s*x\s*(\^\s*(?<degree>[1-9][0-9]*))?)|((?<sign>-?)\s*(?<value>[1-9][0-9]*))");
                List<Nom> noms = new List<Nom>();
                foreach (Match ModularPolynommatch in ModularPolynomRegex.Matches(ModularPolynom))
                {
                    int sign = ModularPolynommatch.Groups["sign"].Value.Contains("-") ? -1 : 1;
                    double value = ModularPolynommatch.Groups["value"].Value != ""
                        ? double.Parse(ModularPolynommatch.Groups["value"].Value)
                        : 1;
                    int degree = ModularPolynommatch.Groups["degree"].Value != ""
                        ? Convert.ToInt32(ModularPolynommatch.Groups["degree"].Value)
                        : ModularPolynommatch.Value.Contains('x')
                            ? 1
                            : 0;
                    //опеределили знак, степень и значние 
                    //заносим в список
                    noms.Add(new Nom(sign, value, degree));
                }

                Coefficients = new Comparison.LinearComparison[noms.Max(nom => nom.Degree) + 1].Select(comp=>new Comparison.LinearComparison(0,Module)).ToArray();

                foreach (Nom nom in noms)
                    Coefficients[Coefficients.Length - 1 - nom.Degree] = new Comparison.LinearComparison((BigInteger)(nom.Value * nom.Sign),Module);
            }

            /// <summary>
            /// Инициализирует нулевой многочлен заданной степени
            /// </summary>
            /// <param name="value"></param>
            /// <param name="module"></param>
            public ModularPolynom(int value,int module)
            {
                Module = module;
                if (value < 0)
                    value = 0;
                Coefficients = new Comparison.LinearComparison[value + 1].Select(comp=>new Comparison.LinearComparison(0,Module)).ToArray();
            }

            /// <summary>
            /// Порядок данного многочлена. Подразумевается, что многочлен неприводим.
            /// </summary>
            public int Exponent
            {
                get
                {
                    //список в который будут помещаться числа, на которые должен делиться порядок.
                    List<int> NOKArray=new List<int>();
                    int MAX = (int)Math.Pow(Module, Degree) - 1;
                    Dictionary<int, int> Dividers = Comparison.Factor(MAX);
                    foreach (int PrimeDivider in Dividers.Keys)
                    {
                        for (int primedegree =1; primedegree <= Dividers[PrimeDivider]; primedegree++)
                        {
                            int degree = MAX / (int) Math.Pow(PrimeDivider, primedegree);
                            ModularPolynom buffer = new ModularPolynom(degree, Module) {[0] = 1};
                            buffer %= this;
                            if (buffer.Degree!=0||( buffer[0] != 1 && buffer[0] != 1 - Module))
                            {
                                NOKArray.Add((int)Math.Pow(PrimeDivider, Dividers[PrimeDivider]+1-primedegree));
                                break;
                            }
                        }
                    }
                    

                    return (int)AdvancedEuclidsalgorithm.LCM(NOKArray.Select(val=>(BigInteger)val).ToArray());

                }
            }

            /// <summary>
            ///     Степень данного многочлена.
            /// </summary>
            public int Degree => Coefficients.Length - 1;

            /// <summary>
            ///     Вычисление значения многочлена при заданном x в поле Zn. Используется схема Горнера.
            /// </summary>
            /// <param name="x">Точка, в которой нужно вычислить многочлен</param>
            /// <returns></returns>
            public double GetValue(int x)
            {
                return GetValueArray(x).Last();
            }

            /// <summary>
            ///     Вычисление значения многочлена при заданном x в поле Zn. Возвращается вся получившаяся строка значений.
            ///     Используется схема Горнера.
            /// </summary>
            /// <param name="x"></param>
            /// <returns></returns>
            public int[] GetValueArray(int x)
            {
                Comparison.LinearComparison[] ResultArray = new Comparison.LinearComparison[Coefficients.Length];
                ResultArray[0] = new Comparison.LinearComparison((int)Coefficients[0].A, Module);
                for (int i = 1; i < Coefficients.Length; i++)
                    ResultArray[i] = new Comparison.LinearComparison((int)(ResultArray[i - 1].A * x + (int)Coefficients[i].A), Module);
                return ResultArray.Select(comparison => (int)comparison.A).ToArray();
            }

            /// <summary>
            ///     Вычисление корней данного многочлена по заданному модулю.
            /// </summary>
            /// <returns>Пары "корень-кратность"</returns>
            public Dictionary<int, int> SolveResults()
            {
                Dictionary<int, int> Roots = new Dictionary<int, int>();
                Stack<int[]> rows = new Stack<int[]>();
                rows.Push(Coefficients.Select(coef => (int)coef.A).ToArray());
                //поместили изначальный массив коэффициентов 
                for (int i = 0; i < Module; i++)
                {
                    ModularPolynom buffer = new ModularPolynom(rows.Peek(),Module);
                    int[] CurrentRow = buffer.GetValueArray(i);
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
                if (Coefficients.Length == 1||Coefficients.All(coef=>coef.A==0))
                    return $"{Coefficients.First().A} (mod {Module})";
                StringBuilder answer = new StringBuilder();
                for (int i = 0; i < Coefficients.Length; i++)
                {
                    if (Coefficients[i].A == 0)
                        continue;
                    if (i <= Coefficients.Length - 2)
                    {
                        /*
                        answer.Append(
                            $"{(Coefficients[i].LeastModulo > 0 ? Coefficients[i].LeastModulo == 1 ? i == 0 ? "" : " + " : $"{(i == 0 ? "" : " + ") + Coefficients[i].LeastModulo}" : $"{(i == 0 ? "-" : " - ")}{(BigInteger.Abs(Coefficients[i].LeastModulo) == 1 ? "" : BigInteger.Abs(Coefficients[i].LeastModulo).ToString())}")}x");
                            */
                        answer.Append(
                            $"{(Coefficients[i].A == 1 ? i == 0 ? "" : " + " : $"{(i == 0 ? "" : " + ") + Coefficients[i].A}")}x");


                        if (i < Coefficients.Length - 2)
                            answer.Append($"^{Coefficients.Length - i - 1}");
                    }
                    else
                    {
                        answer.Append($"{(i == 0 ? "" : " + ") + Coefficients[i].A}");
                    }
                }

                answer.Append($" (mod {Module})");
                return answer.ToString();
            }

            /// <summary>
            /// Возвращает результат сложения двух многочленов.
            /// </summary>
            /// <param name="First"></param>
            /// <param name="Second"></param>
            /// <returns></returns>
            public static ModularPolynom operator +(ModularPolynom First, ModularPolynom Second)
            {
                if (First.Module!=Second.Module)
                    throw new InvalidOperationException("Модули многочленов должны совпадать.");
                int[] MinArray = First.Coefficients.Length <= Second.Coefficients.Length
                    ? First.Coefficients.Select(val=>(int)val.A).ToArray()
                    : Second.Coefficients.Select(val => (int)val.A).ToArray();
                int[] MaxArray = First.Coefficients.Length > Second.Coefficients.Length
                    ? First.Coefficients.Select(val => (int)val.A).ToArray()
                    : Second.Coefficients.Select(val => (int)val.A).ToArray();
                int offset = MaxArray.Length - MinArray.Length;
                int[] Result = new int[MaxArray.Length];
                for (int i = 0; i < offset; i++)
                    Result[i] = MaxArray[i];
                for (int i = offset; i < Result.Length; i++)
                    Result[i] = MaxArray[i] + MinArray[i - offset];
                return Result.Any(val => val %First.Module!= 0) ? new ModularPolynom(Result.SkipWhile(num => num %First.Module== 0).ToArray(),First.Module) : new ModularPolynom(new[] { 0 },First.Module);
            }

            /// <summary>
            /// Возвращает результат вычитания одного многочлена из другого.
            /// </summary>
            /// <param name="First"></param>
            /// <param name="Second"></param>
            /// <returns></returns>
            public static ModularPolynom operator -(ModularPolynom First, ModularPolynom Second)
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
            /// <param name="degree"></param>
            /// <returns></returns>
            private int GetCoefIndex(int degree)
            {
                return Coefficients.Length - 1 - degree;
            }

            public static ModularPolynom GetNormalized(ModularPolynom Source)
            {
                return Source.Coefficients.Any(val => val.A != 0)
                    ? new ModularPolynom(
                        Source.Coefficients.SkipWhile(num => num.A == 0).Select(val => (int) val.A).ToArray(),
                        Source.Module)
                    : new ModularPolynom(new[] {0}, Source.Module);
            }

            /// <summary>
            /// Возвращает результат умножения всех коэффициентов многочлена на заданное число.
            /// </summary>
            /// <param name="First"></param>
            /// <param name="Second"></param>
            /// <returns></returns>
            public static ModularPolynom operator *(ModularPolynom First, int Second)
            {
                return new ModularPolynom(First.Coefficients.Select(val => (int)val.A * Second).ToArray(),First.Module);
            }
            /// <summary>
            /// Возвращает результат умножения многочленов по модулю.
            /// </summary>
            /// <param name="First"></param>
            /// <param name="Second"></param>
            /// <returns></returns>
            public static ModularPolynom operator *(ModularPolynom First, ModularPolynom Second)
            {
                First = GetNormalized(First);
                Second = GetNormalized(Second);
                if (First.Module != Second.Module)
                    throw new InvalidOperationException("Модули многочленов должны совпадать.");
                if (First.Degree == 0) return Second * First[0];
                if (Second.Degree == 0)
                    return First * Second[0];
                List<ModularPolynom> summaryarray = new List<ModularPolynom>();
                for (int i = 0; i <= First.Degree; i++)
                {
                    int FirstDegree = First.GetCoefDegree(i);
                    if (First[i] == 0)
                        continue;
                    summaryarray.Add(new ModularPolynom(FirstDegree + Second.Degree,First.Module));
                    for (int j = 0; j <= Second.Degree; j++)
                    {
                        int SecondDegree = Second.GetCoefDegree(j);
                        summaryarray[summaryarray.Count - 1][summaryarray[summaryarray.Count - 1].GetCoefIndex(FirstDegree + SecondDegree)] = First[i] * Second[j];
                    }
                }
                ModularPolynom Result = new ModularPolynom(summaryarray.Select(pol => pol.Degree).Max(),First.Module);
                Result = summaryarray.Aggregate(Result, (Current, part) => Current + part);
                return Result;
            }

            /// <summary>
            /// Получает доступ к коэффициентам текущего модулярного многочлена с заданным индексом.
            /// </summary>
            /// <param name="Index"></param>
            /// <returns></returns>
            public int this[int Index]
            {
                get => (int)Coefficients[Index].A;
                set => Coefficients[Index].A = value;
            }

            /// <summary>
            /// Инвертирует значения коэффициентов данного многочлена.
            /// </summary>
            /// <param name="Source"></param>
            /// <returns></returns>
            public static ModularPolynom operator -(ModularPolynom Source)
            {
                return new ModularPolynom(Source.Coefficients.Select(coef => -(int)coef.A).ToArray(),Source.Module);
            }

            public ModularPolynom GetNormalized()
            {
                return this * (int) Comparison.MultiplicativeInverse.Solve(this[0], Module);
            }

            /// <summary>
            /// Возвращает неполное частное от деления многочленов по модулю.
            /// </summary>
            /// <param name="First"></param>
            /// <param name="Second"></param>
            /// <returns></returns>
            public static ModularPolynom operator /(ModularPolynom First, ModularPolynom Second)
            {
                if (First.Module != Second.Module)
                    throw new InvalidOperationException("Модули многочленов должны совпадать.");
                //Создали результат - многочлен нужной степени.
                ModularPolynom Residue = new ModularPolynom(First);

                int i = 0;
                int multiplier =(int)Comparison.MultiplicativeInverse.Solve(Second[0], Second.Module);
                Second *= multiplier;
                ModularPolynom Result = new ModularPolynom(First.Degree - Second.Degree,First.Module);
                while (Residue.Degree >= Second.Degree&&Residue.Degree>=1)
                {
                    int index = Result.Coefficients.Length - 1 - (Residue.Degree - Second.Degree);
                    Result[index] = Residue[0] ;

                    ModularPolynom buffer = new ModularPolynom(Result.Degree,First.Module);
                    buffer[index] = Result[index];

                    Residue -= buffer * Second;
                    //Произвели вычитание текущей степени. Теперь она обнулена.
                    i++;
                }

                Second *= (int) Comparison.MultiplicativeInverse.Solve(multiplier, Second.Module);
                Result *= multiplier;
                return Result;
            }

            /// <summary>
            /// Возвращает остаток от деления многочленов по модулю.
            /// </summary>
            /// <param name="First"></param>
            /// <param name="Second"></param>
            /// <returns></returns>
            public static ModularPolynom operator %(ModularPolynom First, ModularPolynom Second)
            {
                if (First.Module != Second.Module)
                    throw new InvalidOperationException("Модули многочленов должны совпадать.");
                //Создали результат - многочлен нужной степени.
                ModularPolynom Residue = new ModularPolynom(First);
                int i = 0;
                int multiplier = (int)Comparison.MultiplicativeInverse.Solve(Second[0], Second.Module);
                Second *= multiplier;
                ModularPolynom Result = new ModularPolynom(First.Degree - Second.Degree,First.Module);
                while (Residue.Degree >= Second.Degree && Residue.Degree >= 1)
                {
                    int index = Result.Coefficients.Length - 1 - (Residue.Degree - Second.Degree);
                    Result[index] = Residue[0];

                    ModularPolynom buffer = new ModularPolynom(Result.Degree,First.Module);
                    buffer[index] = Result[index];

                    Residue -= buffer * Second;
                    //Произвели вычитание текущей степени. Теперь она обнулена.
                    i++;
                }

                return Residue;
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