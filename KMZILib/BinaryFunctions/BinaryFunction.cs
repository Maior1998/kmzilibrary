using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMZILib
{
    /// <summary>
    ///     Класс для работы с бинарными функциями
    /// </summary>
    public partial class BinaryFunction
    {
        private const string VariablesAlphabet = "ABCDXYZKMUWTQS";

        private readonly bool[] ValuesArray;

        /// <summary>
        ///     Инициализирует случайную бинарную функцию заданной длины
        /// </summary>
        /// <param name="Length">Длина столбца-значений функции</param>
        public BinaryFunction(byte Length)
        {
            ValuesArray = new bool[(int) Math.Pow(2, Length)];

            Random rand = new Random();
            for (int i = 0; i < ValuesArray.Length; i++)
                ValuesArray[i] = rand.Next(2) == 1;
        }

        /// <summary>
        ///     Инициализирует новую бинарную функцию по имеющемуся столбцу ее значений
        /// </summary>
        /// <param name="Function">Массив, содержащий в себе столбец-значения</param>
        public BinaryFunction(bool[] Function)
        {
            List<bool> Vector = new List<bool>(Function);
            if (Vector.Count == 1) Vector.Add(Vector[0]);
            while (Math.Abs(Math.Truncate(Math.Log(Vector.Count, 2)) - Math.Log(Vector.Count, 2)) > 0)
                Vector.Insert(0, false);
            ValuesArray = new bool[Vector.Count];
            Vector.CopyTo(ValuesArray, 0);
        }

        /// <summary>
        ///     Инициализирует новую бинарную функцию при помощи знаков операций + | * | ()
        /// </summary>
        /// <param name="Function"></param>
        public BinaryFunction(string Function) : this(LRPN.CalcLRPN.GetValues(Function))
        {
        }

        /// <summary>
        ///     Результирующий столбец функции в виде нулей и единиц
        /// </summary>
        public string Value => string.Join("", ValuesArray.Select(i => i ? 1 : 0));

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
                    bool[] Row = GetBinaryArray(i, CountOfVariables);
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
                for (int i = 0; i < ValuesArray.Length - 1; i++)
                for (int j = i + 1; j < ValuesArray.Length; j++)
                {
                    DNF idnf = new DNF(GetBinaryArray(i, CountOfVariables).Select(val=>new bool?(val)).ToArray());
                    DNF jdnf = new DNF(GetBinaryArray(j, CountOfVariables).Select(val => new bool?(val)).ToArray());
                    if (!idnf.IsPrecedeDNF(jdnf)) continue;
                    if (ValuesArray[i] && !ValuesArray[j]) return false;
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
                    if (!PascalTriangle[i][0]) continue;
                    if (GetBinaryArray(i).Count(val => val) > 1) return false;
                }

                return true;
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
                    if (!ValuesArray[i]) continue;

                    bool[] variables = GetBinaryArray(i, CountOfVariables);
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
                    if (ValuesArray[i]) continue;
                    bool[] variables = GetBinaryArray(i, CountOfVariables);
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
                    if (!ValuesArray[i]) continue;
                    PDNF.Add(GetBinaryArray(i, CountOfVariables).Select(elem => (bool?) elem).ToList());
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
                    if (!PascalTriangle[i][0]) continue;
                    bool[] variables = GetBinaryArray(i, CountOfVariables);

                    List<string> buffer = new List<string>();
                    for (int j = 0; j < CountOfVariables; j++)
                        if (variables[j])
                            buffer.Add(VariablesAlphabet[j].ToString());
                    if (buffer.Count == 0) buffer.Add("1");
                    answer.Add(string.Join("", buffer));
                }

                return string.Join(" + ", answer);
            }
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
            if (QuineTable.First().Value.Keys.All(sdnf => PrecedesDNFsCount(QuineTable, sdnf) != Count)) return null;
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
                    if (buffer.Contains(BufferDNFs[i])) continue;
                    //фиксируем набор и пробуем его со всеми склеить
                    for (int j = i + 1; j < BufferDNFs.Count; j++)
                    {
                        //Если получается склеить - делаем склейку и вставляем ее во временный список
                        if (!BufferDNFs[i].CanGluing(BufferDNFs[j])) continue;
                        DNF GluinedDNF = new DNF(DNF.Gluing(BufferDNFs[i], BufferDNFs[j]));
                        if (!buffer.Contains(GluinedDNF))
                            buffer.Add(GluinedDNF);
                        //Переменные поучавствовали в склеивании
                        BufferDNFs[i].IsParticiated = BufferDNFs[j].IsParticiated = true;
                        //Изменения в составе произошли
                        IsHereAnyDifferences = true;
                    }

                    //Если же переменная ни с кем не склеивалась, то копируем ее во временный список без изменений
                    if (!BufferDNFs[i].IsParticiated) buffer.Add(BufferDNFs[i]);
                }

                if (!BufferDNFs.Last().IsParticiated) buffer.Add(BufferDNFs.Last());
                //Обвновляем текущий список измененным
                BufferDNFs = buffer;
                //Если никаких изменений в результате прохода не произошло - можно останавливаться - достигли тупика.
                if (!IsHereAnyDifferences) break;
            }

            return BufferDNFs;
        }

        /// <summary>
        ///     Строковое представление функции. Возвращается свойство <see cref="Value" />
        /// </summary>
        /// <returns>Строка-представление функции</returns>
        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        ///     Метод, возвращающий значение целого числа в виде массива двоичных значений в указанном количество от его начала
        ///     (младших бит)
        /// </summary>
        /// <param name="Number">Число, которое необходимо перевести в двоичную систему счисления</param>
        /// <param name="Count">Число разрядов. Отсчет ведется от младших к старщим</param>
        /// <returns>Массив двоичных значение - двоичное представление числа</returns>
        public static bool[] GetBinaryArray(int Number, int Count)
        {
            BitArray number = new BitArray(new[] {Number});
            if (Count > number.Count) throw new InvalidOperationException("Число бит не может превосходить 32 бита");
            bool[] Result = new bool[Count];
            for (int i = Count - 1; i >= 0; i--)
                Result[Count - 1 - i] = number[i];
            return Result;
        }

        /// <summary>
        ///     Метод, возвращающий значение целого числа в виде массива двоичных значений
        /// </summary>
        /// <param name="Number">Число, которое необходимо перевести в двоичную систему счисления</param>
        /// <returns>Массив двоичных значение - двоичное представление числа</returns>
        public static bool[] GetBinaryArray(int Number)
        {
            if (Number < 0) throw new InvalidOperationException("Число должно быть положительным");
            if (Number == 0) return new[] {false};
            BitArray number = new BitArray(new[] {Number});

            int i = number.Count - 1;
            while (i >= 0 && !number[i])
                i--;
            bool[] Result = new bool[i + 1];
            for (int k = 0; i >= 0; i--, k++)
                Result[k] = number[i];
            return Result;
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
                    if (PostResult[i, j]) continue;

                    IsColumnOK = false;
                    break;
                }

                if (IsColumnOK) return false;
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
                    if (PostTable[i, j]) continue;

                    IsColumnOK = false;
                    break;
                }

                if (IsColumnOK) return false;
            }

            return true;
        }

        private class DNF : IEquatable<DNF>
        {
            public bool IsParticiated;

            public readonly bool?[] Values;

            public DNF(bool?[] Source)
            {
                Values = new bool?[Source.Length];
                Array.Copy(Source, Values, Source.Length);
            }

            public DNF(DNF Other)
            {
                Values = new bool?[Other.Length];
                Array.Copy(Other.Values, Values, Other.Length);
            }

            public int TrueCount => Values.Count(bit => bit.HasValue && bit.Value);

            public string VariablesValue
            {
                get
                {
                    List<string> Variables = new List<string>();
                    for (int i = 0; i < Values.Length; i++)
                    {
                        if (!Values[i].HasValue) continue;
                        Variables.Add(Values[i].Value ? VariablesAlphabet[i].ToString() : $"!{VariablesAlphabet[i]}");
                    }

                    return string.Join("", Variables);
                }
            }

            public int Length => Values.Length;

            public bool? this[int index]
            {
                get => Values[index];
                private set => Values[index] = value;
            }

            public bool Equals(DNF other)
            {
                if (ReferenceEquals(null, other)) return false;
                return this == other;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return Equals((DNF) obj);
            }

            public override int GetHashCode()
            {
                return Values.GetHashCode();
            }

            public bool CanGluing(DNF Other)
            {
                if (Length != Other.Length) return false;
                bool IsDifferentValuesFound = false;
                for (int i = 0; i < Values.Length; i++)
                {
                    if (Values[i].HasValue != Other[i].HasValue) return false;
                    if (!Values[i].HasValue) continue;
                    if (Values[i].Value == Other[i].Value) continue;
                    if (IsDifferentValuesFound)
                        return false;
                    IsDifferentValuesFound = true;
                }

                return true;
            }

            internal bool IsPrecedeDNF(DNF Second)
            {
                //00-0 : 0000 0010
                for (int i = 0; i < Length; i++)
                {
                    if (!Values[i].HasValue) continue;
                    if (Values[i].Value != Second[i].Value) return false;
                }

                return true;
            }

            public static DNF Gluing(DNF First, DNF Second)
            {
                DNF Result = new DNF(First);
                for (int i = 0; i < Result.Length; i++)
                {
                    if (!First[i].HasValue ||
                        First[i].Value == Second[i].Value) continue;
                    Result[i] = null;
                    break;
                }

                return Result;
            }

            public static bool operator ==(DNF First, DNF Second)
            {
                if (ReferenceEquals(First, null) || ReferenceEquals(Second, null))
                    return ReferenceEquals(First, Second);
                if (First.Values.Length != Second.Values.Length) return false;
                for (int i = 0; i < First.Values.Length; i++)
                {
                    if (First.Values[i].HasValue != Second.Values[i].HasValue) return false;
                    if (!First.Values[i].HasValue) continue;
                    if (First.Values[i].Value == Second.Values[i].Value) continue;
                    return false;
                }

                return true;
            }

            public static bool operator !=(DNF First, DNF Second)
            {
                if (ReferenceEquals(Second, null)) return true;
                if (First.Values.Length != Second.Values.Length) return true;
                for (int i = 0; i < First.Values.Length; i++)
                {
                    if (First.Values[i].HasValue != Second.Values[i].HasValue) return true;
                    if (!First.Values[i].HasValue) continue;
                    if (First.Values[i].Value != Second.Values[i].Value) return true;
                }

                return false;
            }

            public override string ToString()
            {
                StringBuilder result = new StringBuilder();
                foreach (bool? value in Values)
                    if (value == null)
                        result.Append('-');
                    else
                        result.Append(value.Value ? '1' : '0');

                return result.ToString();
            }
        }
    }
}