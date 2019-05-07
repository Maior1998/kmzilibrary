using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security;
using System.Text;

namespace KMZILib
{
    /// <summary>
    ///     Класс для работы с бинарными функциями
    /// </summary>
    public partial class BinaryFunction
    {
        private bool[] valuesarray;

        /// <summary>
        /// Столбец значений функции.
        /// </summary>
        public bool[] ValuesArray
        {
            get => valuesarray;
            set
            {
                if (value.Length != (int) Math.Pow(2,
                        new byte[32].Select((val, ind) => ind + 1).First(val => Math.Pow(2, val) >= value.Length)))
                    throw new InvalidOperationException("Длина столбца значений должна быть степенью 2.");
                valuesarray = value;
            }
        }

        /// <summary>
        ///     Инициализирует случайную бинарную функцию заданной длины
        /// </summary>
        /// <param name="Length">Число переменных функции</param>
        public BinaryFunction(byte Length)
        {
            ValuesArray = new bool[(int) Math.Pow(2, Length)];

            for (int i = 0; i < ValuesArray.Length; i++)
                ValuesArray[i] = RD.Rand.Next(2) == 1;
        }

        /// <summary>
        ///     Инициализирует новую бинарную функцию по имеющемуся столбцу ее значений
        /// </summary>
        /// <param name="Function">Массив, содержащий в себе столбец-значения</param>
        public BinaryFunction(bool[] Function)
        {
            List<bool> Vector = new List<bool>(Function);
            if (Vector.Count == 1)
                Vector.Add(Vector[0]);
            while (Math.Abs(Math.Truncate(Math.Log(Vector.Count, 2)) - Math.Log(Vector.Count, 2)) > 0)
                Vector.Insert(0, false);
            ValuesArray = new bool[Vector.Count];
            Vector.CopyTo(ValuesArray, 0);
        }

        /// <summary>
        /// Инициализирует новую бинарную функцию по имеющемуся столбцу её значений.
        /// </summary>
        /// <param name="Source"></param>
        public BinaryFunction(int[] Source)
        {
            ValuesArray = Source.Select(val => val == 1).ToArray();
        }

        /// <summary>
        ///     Инициализирует новую бинарную функцию при помощи строки, представляющей столбец её значений
        /// </summary>
        /// <param name="Source"></param>
        public BinaryFunction(string Source)
        {
            ValuesArray = Source.ToCharArray().Select(val => val == '1').ToArray();
        }

        /// <summary>
        /// Инициализирует новую бинарную функцию при помощи двоичного значения её вектора-столбца. Число переменных определяется автоматически.
        /// </summary>
        /// <param name="Source"></param>
        public BinaryFunction(int Source)
        {

            ValuesArray = global::Misc.GetBinaryArray(Source, (int) Math.Ceiling(Math.Log(Source, 2)));
        }

        /// <summary>
        /// Инициализирует новую бинарную функцию с заданным числом переменных при помощи двоичного значения её вектора-столбца
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="VariablesCount"></param>
        public BinaryFunction(int Source, int VariablesCount)
        {
            ValuesArray = global::Misc.GetBinaryArray(Source, (int) Math.Pow(2, VariablesCount));
        }

        /// <summary>
        ///     Результирующий столбец функции в виде нулей и единиц
        /// </summary>
        public string Value => string.Join("", ValuesArray.Select(i => i ? 1 : 0));

        /// <summary>
        /// Возвращает вес текущей функции или, другими словами, число единиц в её векторе-значений.
        /// </summary>
        public int Weight => ValuesArray.Count(val => val);

        /// <summary>
        ///     Таблица истинности данной булевой функции
        /// </summary>
        public bool[,] TruthTable
        {
            get
            {
                int TableLength = (int) Math.Pow(2, CountOfVariables);
                bool[,] Result = new bool[TableLength, CountOfVariables + 1];
                for (int i = 0; i < TableLength; i++)
                {
                    bool[] Row = global::Misc.GetBinaryArray(i, CountOfVariables);
                    for (int j = 0; j < CountOfVariables; j++)
                        Result[i, j] = Row[j];
                    Result[i, CountOfVariables] = ValuesArray[i];
                }

                return Result;
            }
        }

        /// <summary>
        ///     Число переменных аргументов функции
        /// </summary>
        public byte CountOfVariables => (byte) Math.Log(ValuesArray.Length, 2);

        /// <summary>
        ///     Свойство функции, показывающее, принадлежит ли она классу функций, сохраняющих нуль
        /// </summary>
        public bool IsT0Class => !ValuesArray[0];

        /// <summary>
        ///     Свойство функции, показывающее, принадлежит ли она классу функций, сохраняющих единицу
        /// </summary>
        public bool IsT1Class => ValuesArray.Last();

        /// <summary>
        ///     Свойство функции, показывающее, принадлежит ли она классу самодвойственных функций
        /// </summary>
        public bool IsTsClass
        {
            /*
             * 1
             * 1
             * 0
             * 1
             * 0
             * 1
             * 0
             * 0
             */
            get
            {
                for (int i = ValuesArray.Length / 2; i < ValuesArray.Length; i++)
                    if (ValuesArray[i] == ValuesArray[ValuesArray.Length - i - 1])
                        return false;
                return true;
            }
        }

        /// <summary>
        ///     Свойство функции, показывающее, принадлежит ли она классу монотонных функций
        /// </summary>
        public bool IsTmClass
        {
            get
            {
                //000
                //|||
                //vvv
                //001

                //0100
                // |
                // v
                //1000

                for (int i = 0; i < ValuesArray.Length - 1; i++)
                {
                    DNF idnf = new DNF(global::Misc.GetBinaryArray(i, CountOfVariables).Select(val => new bool?(val)).ToArray());
                    for (int j = i + 1; j < ValuesArray.Length; j++)
                    {

                        DNF jdnf = new DNF(global::Misc.GetBinaryArray(j, CountOfVariables).Select(val => new bool?(val)).ToArray());
                        if (!idnf.IsPrecedeDNF(jdnf))
                            continue;
                        if (ValuesArray[i] && !ValuesArray[j])
                            return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        ///     Свойство функции, показывающее, принадлежит ли она классу линейных функций
        /// </summary>
        public bool IsTlClass
        {
            get
            {
                bool[][] PascalTriangle = new bool[ValuesArray.Length][];
                PascalTriangle[0] = ValuesArray;
                for (int i = 1; i < ValuesArray.Length; i++)
                {
                    //i - строка в треугольнике Паскаля
                    //каждая последующая строка на 1 короче предыдущей
                    PascalTriangle[i] = new bool[PascalTriangle[i - 1].Length - 1];
                    for (int j = 0; j < PascalTriangle[i].Length; j++)
                        PascalTriangle[i][j] = PascalTriangle[i - 1][j] ^ PascalTriangle[i - 1][j + 1];
                }

                //Треугольник Паскаля заполнен.
                for (int i = 0; i < ValuesArray.Length; i++)
                {
                    if (!PascalTriangle[i][0])
                        continue;
                    if (global::Misc.GetBinaryArray(i).Count(val => val) > 1)
                        return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Степень данной функции в её АНФ.
        /// </summary>
        public int Degree
        {
            get
            {
                bool[][] PascalTriangle = new bool[ValuesArray.Length][];
                PascalTriangle[0] = ValuesArray;
                for (int i = 1; i < ValuesArray.Length; i++)
                {
                    //i - строка в треугольнике Паскаля
                    //каждая последующая строка на 1 короче предыдущей
                    PascalTriangle[i] = new bool[PascalTriangle[i - 1].Length - 1];
                    for (int j = 0; j < PascalTriangle[i].Length; j++)
                        PascalTriangle[i][j] = PascalTriangle[i - 1][j] ^ PascalTriangle[i - 1][j + 1];
                }

                //Треугольник Паскаля заполнен.
                return new int[PascalTriangle.Length].Select((val, ind) => ind)
                    .Where(val => PascalTriangle[val][0]).Select(index => global::Misc.GetBinaryArray(index).Count(val => val))
                    .Max();
            }
        }

        /// <summary>
        /// Определяет, является ли функция уравновешенной, т.е. имеет ли она одинаковое число нулей и единиц в своем столбце значений.
        /// </summary>
        public bool IsEquilibrium => 2 * ValuesArray.Count(val => val) == ValuesArray.Length;

        /// <summary>
        /// Всевозможнные наборы переменных данной функции.
        /// </summary>
        public bool[][] VariablesSets
        {
            get
            {
                return Enumerable.Range(0, ValuesArray.Length).Select(set => global::Misc.GetBinaryArray(set, CountOfVariables))
                    .ToArray();
            }
        }

        /// <summary>
        ///     Строковое представление СДНФ функции
        /// </summary>
        public string PDNF
        {
            get
            {
                if (ValuesArray.All(bit => !bit))
                    return "0";
                List<string> answer = new List<string>();
                for (byte i = 0; i < ValuesArray.Length; i++)
                {
                    if (!ValuesArray[i])
                        continue;

                    bool[] variables = global::Misc.GetBinaryArray(i, CountOfVariables);
                    List<string> buffer = new List<string>();
                    for (int j = 0; j < CountOfVariables; j++)
                        buffer.Add(variables[j]
                            ? VariablesAlphabet[j].ToString()
                            : $"!{VariablesAlphabet[j].ToString()}");
                    answer.Add(string.Join("", buffer));
                }

                return string.Join(" v ", answer);
            }
        }

        /// <summary>
        ///     Строковое представление СКНФ функции
        /// </summary>
        public string PCNF
        {
            get
            {
                if (ValuesArray.All(bit => bit))
                    return "1";
                List<string> answer = new List<string>();
                for (byte i = 0; i < ValuesArray.Length; i++)
                {
                    if (ValuesArray[i])
                        continue;
                    bool[] variables = global::Misc.GetBinaryArray(i, CountOfVariables);
                    List<string> buffer = new List<string>();
                    for (int j = 0; j < CountOfVariables; j++)
                        buffer.Add(!variables[j]
                            ? VariablesAlphabet[j].ToString()
                            : $"!{VariablesAlphabet[j].ToString()}");
                    answer.Add($"({string.Join(" v ", buffer)})");
                }

                return string.Join(" * ", answer);
            }
        }

        /// <summary>
        ///     Строковое представление минимальной ДНФ функции
        /// </summary>
        public string MDNF
        {
            get
            {

                if (ValuesArray.Distinct().Count() == 1)
                    return ValuesArray.First() ? "1" : "0";
                List<List<bool?>> PDNF = new List<List<bool?>>();
                for (byte i = 0; i < ValuesArray.Length; i++)
                {
                    if (!ValuesArray[i])
                        continue;
                    PDNF.Add(global::Misc.GetBinaryArray(i, CountOfVariables).Select(elem => (bool?)elem).ToList());
                }
                //Получили наборы СДНФ

                List<DNF> BufferDNFs = Minimize(PDNF.Select(pdnf => new DNF(pdnf.ToArray())).ToList());
                //Получили наборы импликант
                //Теперь необходимо составить таблицу Квайна

                //BaseSDNFs - список исходных ДНФ
                List<DNF> BaseSDNFs = PDNF.Select(pdnf => new DNF(pdnf.ToArray())).ToList();
                //QuineTable - таблица Квайна
                Dictionary<DNF, Dictionary<DNF, bool>> QuineTable = new Dictionary<DNF, Dictionary<DNF, bool>>();
                foreach (DNF implicant in BufferDNFs)
                {
                    QuineTable.Add(implicant, new Dictionary<DNF, bool>());
                    foreach (DNF BaseDNF in BaseSDNFs)
                        QuineTable[implicant].Add(BaseDNF, implicant.IsPrecedeDNF(BaseDNF));
                }

                List<DNF> Result = new List<DNF>();
                //пока остались столбцы
                while (QuineTable.First().Value.Count != 0)
                {
                    DNF CurrentColumn;
                    int i = 1;
                    while ((CurrentColumn = GetColumnByCount(QuineTable, i)) == null)
                        i++;
                    DNF[] CandidatesRows = QuineTable.Keys.Where(mindnf => QuineTable[mindnf][CurrentColumn]).ToArray();
                    DNF BestRow =
                        CandidatesRows.First(row => row.TrueCount == CandidatesRows.Max(comp => comp.TrueCount));
                    Result.Add(BestRow);
                    RemoveColumns(QuineTable, BestRow);
                }


                //1) Очистить столбцы таблицы
                //2) выбрать столбец с наименьшим числом единиц
                //3) Среди строк, содержащих единицы в этом столбце,
                //      выделяется одна с наибольшим числом единиц.
                //      Эта строка включается в покрытие,
                //      текущая таблица сокращается вычеркиванием всех столбцов,
                //      в которых выбранная строка имеет единицу
                //4) Если в таблице есть не вычеркнутые столбцы, то выполняется п. 2, иначе – покрытие построено
                return string.Join(" v ", Result.Select(dnf => dnf.VariablesValue));
            }
        }

        /// <summary>
        ///     Строковое представление функции в виде полинома Жегалкина
        /// </summary>
        public string ZhegalkinPolynomial
        {
            get
            {
                if (ValuesArray.Distinct().Count() == 1)
                    return ValuesArray.First() ? "1" : "0";
                bool[][] PascalTriangle = new bool[ValuesArray.Length][];
                PascalTriangle[0] = ValuesArray;
                for (int i = 1; i < ValuesArray.Length; i++)
                {
                    //i - строка в треугольнике Паскаля
                    //каждая последующая строка на 1 короче предыдущей
                    PascalTriangle[i] = new bool[PascalTriangle[i - 1].Length - 1];
                    for (int j = 0; j < PascalTriangle[i].Length; j++)
                        PascalTriangle[i][j] = PascalTriangle[i - 1][j] ^ PascalTriangle[i - 1][j + 1];
                }

                //Треугольник Паскаля заполнен.
                List<string> answer = new List<string>();
                for (int i = 0; i < ValuesArray.Length; i++)
                {
                    if (!PascalTriangle[i][0])
                        continue;
                    bool[] variables = global::Misc.GetBinaryArray(i, CountOfVariables);

                    List<string> buffer = new List<string>();
                    for (int j = 0; j < CountOfVariables; j++)
                        if (variables[j])
                            buffer.Add(VariablesAlphabet[j].ToString());
                    if (buffer.Count == 0)
                        buffer.Add("1");
                    answer.Add(string.Join("", buffer));
                }

                return string.Join(" + ", answer);
            }
        }

        /// <summary>
        /// Возвращает строковое представление преобразования Фурье для данной функции.
        /// </summary>
        public string FourierTransform => GetFourierTransformString(this);

        /// <summary>
        /// Возвращает спектр Фурье для данной функции.
        /// </summary>
        public int[] FourierSpectrum => GetFourierSpectrum(this);

        /// <summary>
        /// Возвращает спектр Уолша-Адамара для данной функции.
        /// </summary>
        public int[] WalshHadamardSpectrum => GetWalshHadamardSpectrum(this);

        /// <summary>
        /// Возвращает строковое представление преобразования Уолша-Адамара для данной функции.
        /// </summary>
        public string WalshHadamardTransform => GetWalshHadamardTransformString(this);

        /// <summary>
        /// Возвращает расстояние между текущей и заданной функциями или, другими словами, число различающихся значений их векторов-столбцов.
        /// </summary>
        /// <param name="Other"></param>
        /// <returns></returns>
        public int Distance(BinaryFunction Other)
        {
            return Distance(this, Other);
        }

        /// <summary>
        /// Возвращает значение параметра нелинейности для данной функции.
        /// </summary>
        public int NonLinearityValue => GetNonLinearityValue(this);

        /// <summary>
        /// Возвращает расстояние от текущей функции до заданного множества функций или, другими словами, минимальное расстояние между всевозможными переборами.
        /// </summary>
        /// <param name="Second"></param>
        /// <returns></returns>
        public int Distance(BinaryFunction[] Second)
        {
            return Distance(this, Second);
        }

        /// <summary>
        /// Возвращает максимальный порядок имунности данной функции.
        /// </summary>
        public int MaxCorrelationImmunityValue => GetMaxCorrelationImmunityValue(this);

        /// <summary>
        /// Возвращает максимальный порядок устойчивости данной функции.
        /// </summary>
        public int MaxStableValue => GetMaxStableValue(this);

        public bool GetValue(bool[] Set)
        {
            if (Set.Length!=CountOfVariables)
                throw new InvalidOperationException("Длина набора должна совпадать с числом переменных данной функции");
            return ValuesArray[
                Enumerable.Range(0, (int) Math.Pow(CountOfVariables, 2)).First(val => val == Misc.GetValue(Set))];
        }

        /// <summary>
        ///     Строковое представление функции. Возвращается свойство <see cref="Value" />
        /// </summary>
        /// <returns>Строка-представление функции</returns>
        public override string ToString()
        {
            return Value;
        }

        
    }
}