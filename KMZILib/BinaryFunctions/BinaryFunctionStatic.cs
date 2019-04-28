using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace KMZILib
{
    public partial class BinaryFunction
    {
        /// <summary>
        ///     Возвращает расстояние между двумя функциями или, другими словами, число различающихся значений их
        ///     векторов-столбцов.
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static int Distance(BinaryFunction First, BinaryFunction Second)
        {
            if (First.ValuesArray.Length != Second.ValuesArray.Length)
                throw new InvalidOperationException("Длины столбцов-значений функций ддолжны совпадать.");

            return First.ValuesArray.Select((val, ind) => val ^ Second.ValuesArray[ind]).Count(val => val);
        }

        /// <summary>
        ///     Возвращает массив-формулу преобразования Фурье для заданной булевой функции
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        private static bool[][] GetFourierTransformFormula(BinaryFunction Source)
        {
            List<bool[]> FourierTransformFormula = new List<bool[]>();
            for (int i = 0; i < Source.ValuesArray.Length; i++)
            {
                if (!Source.ValuesArray[i])
                    continue;
                FourierTransformFormula.Add(GetBinaryArray(i, Source.CountOfVariables));
            }

            return FourierTransformFormula.ToArray();
        }

        /// <summary>
        ///     Возвращает строковое представления формулы преобразования Фурье для заданной функции.
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static string GetFourierTransformString(BinaryFunction Source)
        {
            bool[][] FourierTransformFormula = GetFourierTransformFormula(Source);
            StringBuilder Result = new StringBuilder(FourierTransformFormula[0].All(boo => !boo)
                ? "1"
                : $"(-1)^{string.Join("⊕", FourierTransformFormula[0].Select((val, ind) => $"u{ind + 1}").Where((val, ind) => FourierTransformFormula[0][ind]))}");
            for (int i = 1; i < FourierTransformFormula.Length; i++)
                Result.Append(
                    $" + (-1)^{string.Join("⊕", FourierTransformFormula[i].Select((val, ind) => $"u{ind + 1}").Where((val, ind) => FourierTransformFormula[i][ind]))}");

            return Result.ToString();
        }

        /// <summary>
        ///     Возвращает спектр Фурье для заданной булевой функции.
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static int[] GetFourierSpectrum(BinaryFunction Source)
        {
            bool[][] FourierTransformFormula = GetFourierTransformFormula(Source);

            int[] Result = new int[Source.ValuesArray.Length];
            for (int i = 0; i < Source.ValuesArray.Length; i++)
            {
                bool[] CurrentSet = GetBinaryArray(i, Source.CountOfVariables);
                Result[i] = FourierTransformFormula[0].All(boo => !boo) ? 1 : 0;
                for (int j = FourierTransformFormula[0].All(boo => !boo) ? 1 : 0;
                    j < FourierTransformFormula.Length;
                    j++)
                    Result[i] += (int) Math.Pow(-1,
                        CurrentSet.Where((val, ind) => FourierTransformFormula[j][ind])
                            .Aggregate(false, (Current, next) => Current ^ next)
                            ? 1
                            : 0);
            }

            return Result;
        }

        /// <summary>
        ///     Определяет, является ли заданная функция m-устойчивой.
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static bool IsStable(BinaryFunction Source, int m)
        {
            if (m >= Source.CountOfVariables)
                throw new InvalidOperationException("Число m должно быть меньше числа аргументов функции.");
            if (!Source.IsEquilibrium)
                return false;
            for (int i = 0; i < Source.ValuesArray.Length; i++)
            {
                bool[] CurrentSet = GetBinaryArray(i, Source.CountOfVariables);
                int Weight = CurrentSet.Count(val => val);
                if (Weight > m || Weight == 0)
                    continue;
                if (Source.ValuesArray[i])
                    return false;
            }


            return true;
        }

        /// <summary>
        ///     Возвращает максимальный порядок устойчивости заданной функции.
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static int GetMaxStableValue(BinaryFunction Source)
        {
            int FixedVariablesCount = 1;
            for (; FixedVariablesCount <= Source.CountOfVariables; FixedVariablesCount++)
            {
                //Выбираем, какие переменные фиксировать. Задаем наборы, в которых на местах фиксированных переменных будут 1, на месте свободных 0.
                List<bool[]> States = new List<bool[]>();
                for (int i = 0; i < (int) Math.Pow(2, Source.CountOfVariables); i++)
                {
                    bool[] CurrentSet = GetBinaryArray(i, Source.CountOfVariables);
                    if (CurrentSet.Count(val => val) != FixedVariablesCount)
                        continue;
                    if (States.Any(set => set.Select((boo, ind) => boo == CurrentSet[ind]).All(boo => boo)))
                        continue;
                    States.Add(CurrentSet);
                    //Выбрали, какие переменные зафиксировать.

                    //Создаем словарь, где ключи - конкретный зафиксированный набор, а значения - значения столбца функции в таких наборах
                    Dictionary<bool[], List<bool>> NewFunctions = new Dictionary<bool[], List<bool>>();
                    for (int j = 0; j < Source.ValuesArray.Length; j++)
                    {
                        //Текущая строка таблицы истинности
                        bool[] CurrentFuncSet = GetBinaryArray(j, Source.CountOfVariables);
                        bool FuncValue = Source.ValuesArray[j];
                        if (NewFunctions.Keys.Any(row =>
                            row.Select((variable, index) => !CurrentSet[index] || variable == CurrentFuncSet[index])
                                .All(res => res)))
                        {
                            NewFunctions[NewFunctions.Keys.First(row =>
                                row.Select((variable, index) => !CurrentSet[index] || variable == CurrentFuncSet[index])
                                    .All(res => res))].Add(FuncValue);
                        }
                        else
                            NewFunctions.Add(CurrentFuncSet, new List<bool>(new[] {FuncValue}));
                    }

                    if (NewFunctions.Values.Any(val => !new BinaryFunction(val.ToArray()).IsEquilibrium))
                        return FixedVariablesCount - 1;
                }
            }


            return FixedVariablesCount - 1;
        }

        /// <summary>
        ///     Возвращает максимальный порядок имунности для заданной функции.
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static int GetMaxCorrelationImmunityValue(BinaryFunction Source)
        {
            int m = 1;
            for (; m < Source.CountOfVariables; m++)
            for (int i = 0; i < Source.ValuesArray.Length; i++)
            {
                bool[] CurrentSet = GetBinaryArray(i, Source.CountOfVariables);
                int Weight = CurrentSet.Count(val => val);
                if (Weight > m || Weight == 0)
                    continue;
                if (Source.ValuesArray[i])
                    break;
            }


            return m - 1;
        }

        /// <summary>
        ///     Определеяет, имеет ли функция корреляционную иммунность порядка m.
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static bool IsCorrelationImmune(BinaryFunction Source, int m)
        {
            if (m >= Source.CountOfVariables)
                throw new InvalidOperationException("Число m должно быть меньше числа аргументов функции.");
            for (int i = 0; i < Source.ValuesArray.Length; i++)
            {
                bool[] CurrentSet = GetBinaryArray(i, Source.CountOfVariables);
                int Weight = CurrentSet.Count(val => val);
                if (Weight > m || Weight == 0)
                    continue;
                if (Source.ValuesArray[i])
                    return false;
            }


            return true;
        }

        /// <summary>
        ///     Возвращает массив-формулу преобразования Уолша-Адамара для заданной булевой функции
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        private static (bool[], bool)[] GetWalshHadamardTransformFormula(BinaryFunction Source)
        {
            List<(bool[], bool)> FourierTransformFormula = new List<(bool[], bool)>();
            for (int i = 0; i < Source.ValuesArray.Length; i++)
                FourierTransformFormula.Add((GetBinaryArray(i, Source.CountOfVariables), Source.ValuesArray[i]));

            return FourierTransformFormula.ToArray();
        }

        /// <summary>
        ///     Возвращает строковое представления формулы преобразования Уолша-Адамара для заданной функции.
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static string GetWalshHadamardTransformString(BinaryFunction Source)
        {
            (bool[], bool)[] WalshHadamardTransformFormula = GetWalshHadamardTransformFormula(Source);
            StringBuilder Result = new StringBuilder(WalshHadamardTransformFormula[0].Item1.All(boo => !boo)
                ? WalshHadamardTransformFormula[0].Item2 ? "-1" : "1"
                : $"(-1)^{string.Join("⊕", WalshHadamardTransformFormula[0].Item1.Select((val, ind) => $"u{ind + 1}").Where((val, ind) => WalshHadamardTransformFormula[0].Item1[ind]))}");
            for (int i = 1; i < WalshHadamardTransformFormula.Length; i++)
                Result.Append(
                    $" + (-1)^{string.Join("⊕", WalshHadamardTransformFormula[i].Item1.Select((val, ind) => $"u{ind + 1}").Where((val, ind) => WalshHadamardTransformFormula[i].Item1[ind]))}{(Source.ValuesArray[i] ? "⊕1" : "")}");

            return Result.ToString();
        }

        /// <summary>
        ///     Возвращает спектр Уолша-Адамара для заданной булевой функции.
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static int[] GetWalshHadamardSpectrum(BinaryFunction Source)
        {
            (bool[], bool)[] WalshHadamardTransformFormula = GetWalshHadamardTransformFormula(Source);

            int[] Result = new int[Source.ValuesArray.Length];
            for (int i = 0; i < Source.ValuesArray.Length; i++)
            {
                bool[] CurrentSet = GetBinaryArray(i, Source.CountOfVariables);
                Result[i] = WalshHadamardTransformFormula[0].Item1.All(boo => !boo)
                    ? WalshHadamardTransformFormula[0].Item2 ? -1 : 1
                    : 0;
                for (int j = WalshHadamardTransformFormula[0].Item1.All(boo => !boo) ? 1 : 0;
                    j < WalshHadamardTransformFormula.Length;
                    j++)
                    Result[i] += (int) Math.Pow(-1,
                        CurrentSet.Where((val, ind) => WalshHadamardTransformFormula[j].Item1[ind])
                            .Aggregate(false, (Current, next) => Current ^ next) ^
                        WalshHadamardTransformFormula[j].Item2
                            ? 1
                            : 0);
            }

            return Result;
        }

        /// <summary>
        ///     Возвращает расстояние между заданной функцией и множеством функций или, другими словами, минимальное расстояние
        ///     между всевозможными переборами.
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static int Distance(BinaryFunction First, BinaryFunction[] Second)
        {
            return Distance(new[] {First}, Second);
        }

        /// <summary>
        ///     Возвращает расстояние между двумя множествами функций или, другими словами, минимальное расстояние между
        ///     всевозможными переборами.
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static int Distance(BinaryFunction[] First, BinaryFunction[] Second)
        {
            return First.Select(func1 => Second.Select(func2 => func2.Distance(func1)).Min()).Min();
        }

        private static void RemoveColumns(IReadOnlyDictionary<DNF, Dictionary<DNF, bool>> QuineTable, DNF TargetRow)
        {
            //необходимо удлаить столбцы, в которых задланная строка имеет true
            DNF[] TargetingColumns =
                QuineTable[TargetRow].Keys.Where(key => QuineTable[TargetRow][key]).ToArray();
            foreach (KeyValuePair<DNF, Dictionary<DNF, bool>> row in QuineTable)
            foreach (DNF targetingColumn in TargetingColumns)
                row.Value.Remove(targetingColumn);
        }

        /// <summary>
        ///     Получение столбца таблицы Квайна по числу точек в нем
        /// </summary>
        /// <param name="QuineTable"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        private static DNF GetColumnByCount(Dictionary<DNF, Dictionary<DNF, bool>> QuineTable, int Count)
        {
            if (QuineTable.First().Value.Keys.All(sdnf => PrecedesDNFsCount(QuineTable, sdnf) != Count))
                return null;
            return QuineTable.First().Value.Keys.First(sdnf => PrecedesDNFsCount(QuineTable, sdnf) == Count);
        }

        /// <summary>
        ///     Получение числа точек в столбце таблицы Квайна
        /// </summary>
        /// <param name="QuineTable"></param>
        /// <param name="BaseDNF"></param>
        /// <returns></returns>
        private static int PrecedesDNFsCount(Dictionary<DNF, Dictionary<DNF, bool>> QuineTable, DNF BaseDNF)
        {
            return QuineTable.Count(row => row.Value[BaseDNF]);
        }

        private static List<DNF> Minimize(List<DNF> BufferDNFs)
        {
            while (true)
            {
                //в один момент у нас все перестанет склеиваться, поэтому придется завести такую переменную.
                bool IsHereAnyDifferences = false;
                List<DNF> buffer = new List<DNF>();
                foreach (DNF bufferDnF in BufferDNFs)
                    bufferDnF.IsParticiated = false;
                for (int i = 0; i < BufferDNFs.Count - 1; i++)
                {
                    if (buffer.Contains(BufferDNFs[i]))
                        continue;
                    //фиксируем набор и пробуем его со всеми склеить
                    for (int j = i + 1; j < BufferDNFs.Count; j++)
                    {
                        //Если получается склеить - делаем склейку и вставляем ее во временный список
                        if (!BufferDNFs[i].CanGluing(BufferDNFs[j]))
                            continue;
                        DNF GluinedDNF = new DNF(DNF.Gluing(BufferDNFs[i], BufferDNFs[j]));
                        if (!buffer.Contains(GluinedDNF))
                            buffer.Add(GluinedDNF);
                        //Переменные поучавствовали в склеивании
                        BufferDNFs[i].IsParticiated = BufferDNFs[j].IsParticiated = true;
                        //Изменения в составе произошли
                        IsHereAnyDifferences = true;
                    }

                    //Если же переменная ни с кем не склеивалась, то копируем ее во временный список без изменений
                    if (!BufferDNFs[i].IsParticiated)
                        buffer.Add(BufferDNFs[i]);
                }

                if (!BufferDNFs.Last().IsParticiated)
                    buffer.Add(BufferDNFs.Last());
                //Обвновляем текущий список измененным
                BufferDNFs = buffer;
                //Если никаких изменений в результате прохода не произошло - можно останавливаться - достигли тупика.
                if (!IsHereAnyDifferences)
                    break;
            }

            return BufferDNFs;
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
            return string.Concat(GetBinaryArray(Number).Select(val => val ? '1' : '0'));
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
            return string.Concat(GetBinaryArray(Number, Count).Select(val => val ? '1' : '0'));
        }

        /// <summary>
        ///     Метод, возвращающий таблицу Поста для заданного множества функций. Строки - функции. Столбцы - класс функций T0,
        ///     T1, Ts, Tm, Tl соответственно
        /// </summary>
        /// <param name="Functions">Множество функций</param>
        /// <returns>Массив bool - таблица поста</returns>
        public static bool[,] GetPostTable(IList<BinaryFunction> Functions)
        {
            bool[,] Result = new bool[Functions.Count, 5];
            int i = 0;
            foreach (BinaryFunction binaryFunction in Functions)
            {
                Result[i, 0] = binaryFunction.IsT0Class;
                Result[i, 1] = binaryFunction.IsT1Class;
                Result[i, 2] = binaryFunction.IsTsClass;
                Result[i, 3] = binaryFunction.IsTmClass;
                Result[i, 4] = binaryFunction.IsTlClass;
                i++;
            }

            return Result;
        }

        /// <summary>
        ///     Определяет, является ли указанное множество функций функционально полным (да, тавтология)
        /// </summary>
        /// <param name="Functions"></param>
        /// <returns></returns>
        public static bool IsFunctionallyComplete(IList<BinaryFunction> Functions)
        {
            bool[,] PostResult = GetPostTable(Functions);
            for (int j = 0; j < 5; j++)
            {
                bool IsColumnOK = true;
                for (int i = 0; i < Functions.Count; i++)
                {
                    if (PostResult[i, j])
                        continue;

                    IsColumnOK = false;
                    break;
                }

                if (IsColumnOK)
                    return false;
            }

            return true;
        }

        /// <summary>
        ///     Для заданной таблице Поста вычисляет, является ли множество функций, по которой составлена таблица, функционально
        ///     полным
        /// </summary>
        /// <param name="PostTable">Таблица Поста</param>
        /// <returns>true - функционально полное множество; false - неполное</returns>
        public static bool IsFunctionallyComplete(bool[,] PostTable)
        {
            for (int j = 0; j < 5; j++)
            {
                bool IsColumnOK = true;
                for (int i = 0; i < PostTable.GetLength(0); i++)
                {
                    if (PostTable[i, j])
                        continue;

                    IsColumnOK = false;
                    break;
                }

                if (IsColumnOK)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Возвращает значение параметра нелинейности для заданной функции.
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static int GetNonLinearityValue(BinaryFunction Source)
        {
            return (int)(Math.Pow(2, Source.CountOfVariables - 1) - 0.5 * Source.WalshHadamardSpectrum.Select(Math.Abs).Max());
        }
    }
}
