using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace KMZILib
{
    /// <summary>
    /// Класс для работы с длинными целыми числами
    /// </summary>
    public class SuperLong : ICloneable, IComparable<SuperLong>
    {
        #region Поля

        private readonly List<byte> NumberList = new List<byte>();
        /// <summary>
        /// Определяет, является ли число неотрицательным
        /// </summary>
        public bool IsNonNegative { get; private set; } = true;

        /// <summary>
        /// Число разрядов данного длинного числа
        /// </summary>
        public int Length
        {
            get => NumberList.Count;
            set
            {
                if (value > NumberList.Count)
                    NumberList.InsertRange(0, new byte[value - NumberList.Count]);
                else
                    NumberList.RemoveRange(0, NumberList.Count - value);
            }
        } //Добавляет нули в конец списка, или удаляет первые элементы

        public sbyte Sign
        {
            get
            {
                if(this==0) return 0;
                return (sbyte)(IsNonNegative ? 1 : -1);
            }
        }

        #endregion

        #region Конструкторы

        private SuperLong()
        {
        }

        /// <summary>
        /// Инициализирует новое длиннное число с помощью целого числа типа <see cref="int"/>
        /// </summary>
        /// <param name="Val"></param>
        public SuperLong(int Val) : this(Val.ToString())
        {
        }

        /// <summary>
        /// Инициализирует новое длинное число с помощью строки <see cref="string"/>, в которой оно записано
        /// </summary>
        /// <param name="NumberString"></param>
        public SuperLong(string NumberString)
        {
            if (NumberString.IndexOf('-') != -1)
            {
                IsNonNegative = false;
                NumberString = NumberString.Remove(0, 1);
            }

            foreach (char DigitSymbol in NumberString)
                NumberList.Add((byte) char.GetNumericValue(DigitSymbol));
            NormalizeForm(this);
        }

        /// <summary>
        /// Инициализирует новое длинное число с помощью уже существующего.
        /// </summary>
        /// <param name="Other"></param>
        public SuperLong(SuperLong Other)
        {
            NumberList=new List<byte>(Other.NumberList);
            IsNonNegative = Other.IsNonNegative;
        }

        #endregion

        #region Переписанные методы/операторы

        public int CompareTo(SuperLong other)
        {
            if (this > other) return 1;
            return this == other ? 0 : -1;
        }

        /// <summary>
        /// Возвращает строковое представление длинного числа
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder Answer = new StringBuilder();
            if (!IsNonNegative) //Если число отрицательное, в начало строки необходимо поставить знак "-"
                Answer.Append('-');
            foreach (byte b in NumberList)
                Answer.Append(b); //Добавление всех цифр в строку и последующее ее возвращение
            return Answer.ToString();
        }

        /// <summary>
        /// Получает доступ к массиву разрядов данного длинного числа
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public byte this[int index]
        {
            //Перегрузка индексирования для более удобного доступа к цифрам числа
            get => NumberList[index];
            set => NumberList[index] = value;
        }

        /// <summary>
        /// Осуществляет возведение длинного числа в степень
        /// </summary>
        /// <param name="Base">Основание</param>
        /// <param name="Degree">Степень</param>
        /// <returns></returns>
        public static SuperLong Pow(SuperLong Base, int Degree)
        {
            SuperLong Result = Copy(Base);

            BitArray array = new BitArray(new[] {Degree});
            for (int i = 0; i < array.Length; i++)
                if (array[i])
                {
                    array[i] = false;
                    break;
                }

            for (int i = 0; i < array.Length; i++)
                if (array[i])
                {
                    Result *= Result;
                    Result *= Base;
                }
                else
                    Result *= Base;

            return Result;
        }

        /// <summary>
        /// Вычисляет модуль длинного числа
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static SuperLong Abs(SuperLong Source)
        {
            return new SuperLong(Source) { IsNonNegative = true };
        }

        /// <summary>
        /// Инкремент длинного числа
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static SuperLong operator ++(SuperLong Source)
        {
            //Оператор инкремента
            return Source + new SuperLong(1);
        }

        /// <summary>
        /// Декремент длинного числа
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static SuperLong operator --(SuperLong Source)
        {
            //Оператор декремента
            return Source - new SuperLong(1);
        }

        /// <summary>
        /// Выполняет сложение двух длинных чисел
        /// </summary>
        /// <param name="First">Первый операнд</param>
        /// <param name="Second">Второй операнд</param>
        /// <returns>Результат операции</returns>
        public static SuperLong operator +(SuperLong First, SuperLong Second)
        {
            //Оператор сложения с учетом знаков
            SuperLong Result;
            if (First.IsNonNegative && Second.IsNonNegative) //если оба числа положительные,
                return Plus(First, Second); //то ответ - сумма их модулей
            if (First.IsNonNegative != Second.IsNonNegative)
            {
                //Если знаки чисел не равны, то из большего операнда
                //Вычитается меньший и результату присваивается знак большего
                //операнда
                if (First > Second)
                {
                    Result = Minus(First, Second);
                    Result.IsNonNegative = First.IsNonNegative;
                    return Result;
                }

                if (First < Second)
                {
                    Result = Minus(Second, First);
                    Result.IsNonNegative = Second.IsNonNegative;
                    return Result;
                }

                //Если же они равны, что результатом такого сложения будет 0
                return new SuperLong("0");
            }

            //Если оба операнда отрицательные, необходимо сложить
            //их модули и поставить знак "-"
            Result = Plus(First, Second);
            Result.IsNonNegative = false;
            return Result;
        }

        /// <summary>
        /// Выполняет сложение длиннного числа с целым <see cref="int"/>
        /// </summary>
        /// <param name="First">Первый операнд</param>
        /// <param name="SecondInt">Второй операнд</param>
        /// <returns>Результат операции</returns>
        public static SuperLong operator +(SuperLong First, int SecondInt)
        {
            //Переводим второй операнд к типу SuperLong и выполняем сложение
            //уже двух операндов типа SuperLong
            SuperLong Second = new SuperLong(SecondInt);
            return First + Second;
        }

        /// <summary>
        /// Выполняет сложение целого числа <see cref="int"/> с длинным числом
        /// </summary>
        /// <param name="FirstInt">Первый операнд</param>
        /// <param name="Second">Второй операнд</param>
        /// <returns>Результат операции</returns>
        public static SuperLong operator +(int FirstInt, SuperLong Second)
        {
            //Переводим первый операнд к типу SuperLong и выполняем сложение
            //уже двух операндов типа SuperLong
            SuperLong First = new SuperLong(FirstInt);
            return First + Second;
        }

        /// <summary>
        /// Выполняет инвертирование длинного числа (Замену знака)
        /// </summary>
        /// <param name="Source">Первый операнд</param>
        /// <returns>Результат операции</returns>
        public static SuperLong operator -(SuperLong Source)
        {
            //Создаем копию исходного числа
            SuperLong Result = Copy(Source);
            //и меняем у нее знак на противоположный
            Result.IsNonNegative = !Result.IsNonNegative;
            return Result;
        }


        /// <summary>
        /// Выполняет вычитание целого числа <see cref="int"/> с длинным числом
        /// </summary>
        /// <param name="FirstInt">Первый операнд</param>
        /// <param name="Second">Второй операнд</param>
        /// <returns>Результат операции</returns>
        public static SuperLong operator -(int FirstInt, SuperLong Second)
        {
            //Переводим первый операнд к типу SuperLong и выполняем вычитание
            //уже двух операндов типа SuperLong
            SuperLong First = new SuperLong(FirstInt);
            return First - Second;
        }

        /// <summary>
        /// Выполняет сложение длинных чисел
        /// </summary>
        /// <param name="First">Первый операнд</param>
        /// <param name="Second">Второй операнд</param>
        /// <returns>Результат операции</returns>
        public static SuperLong operator -(SuperLong First, SuperLong Second)
        {
            SuperLong Result;
            //Если оба операнда отрицательные
            //то необходимо сравнить их по модулям
            if (!First.IsNonNegative && !Second.IsNonNegative)
            {
                //выбрав наибольший, находим разность с меньшим
                //и ставим знак большего
                if (First > Second)
                {
                    Result = Minus(First, Second);
                    Result.IsNonNegative = First.IsNonNegative;
                    return Result;
                }

                if (First < Second)
                {
                    Result = Minus(Second, First);
                    Result.IsNonNegative = Second.IsNonNegative;
                    return Result;
                }

                //Если же они равны - результатом будет 0
                return new SuperLong("0");
            }

            if (First.IsNonNegative && Second.IsNonNegative)
            {
                //Если оба операнда положительные - опять их сравниваем
                if (First > Second)
                {
                    //самый простой случай - когда первый операнд
                    //больше второго - просто находим разность
                    Result = Minus(First, Second);
                    return Result;
                }

                if (First < Second)
                {
                    //Если второй больше первого - находим разность
                    //модулей и ставим знак "-"
                    Result = Minus(Second, First);
                    Result.IsNonNegative = false;
                    return Result;
                }

                //Если они равны - отдаем 0
                return new SuperLong("0");
            }

            //Остается единственный случай, когда знаки операндов не равны
            //тогда складываем их и ставим знак первого операнда
            Result = Plus(First, Second);
            Result.IsNonNegative = First.IsNonNegative;
            return Result;
        }

        /// <summary>
        /// Выполняет вычитание длинного числа и целого числа <see cref="int"/>
        /// </summary>
        /// <param name="First">Первый операнд</param>
        /// <param name="SecondInt">Второй операнд</param>
        /// <returns>Результат операции</returns>
        public static SuperLong operator -(SuperLong First, int SecondInt)
        {
            //Переводим второй операнд к типу SuperLong и находим разность
            //уже двух операндов типа SuperLong
            SuperLong Second = new SuperLong(SecondInt);
            return First - Second;
        }

        /// <summary>
        /// Выполняет умножение длинного числа на целое число <see cref="int"/>
        /// </summary>
        /// <param name="First">Первый операнд</param>
        /// <param name="SecondInt">Второй операнд</param>
        /// <returns>Результат операции</returns>
        public static SuperLong operator *(SuperLong First, int SecondInt)
        {
            //Переводим второй операнд к типу SuperLong и выполняем умножение
            //уже двух операндов типа SuperLong
            SuperLong Second = new SuperLong(SecondInt);
            return First * Second;
        }

        /// <summary>
        /// Выполняет умножение целого числа <see cref="int"/> на длиннное число
        /// </summary>
        /// <param name="FirstInt">Первый операнд</param>
        /// <param name="Second">Второй операнд</param>
        /// <returns>Результат операции</returns>
        public static SuperLong operator *(int FirstInt, SuperLong Second)
        {
            //Переводим первый операнд к типу SuperLong и выполняем умножение
            //уже двух операндов типа SuperLong
            SuperLong First = new SuperLong(FirstInt);
            return First * Second;
        }

        /// <summary>
        /// Выполняет умножение двух длинных чисел
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static SuperLong operator *(SuperLong First, SuperLong Second)
        {
            //Если хотя бы один из операндов - 0
            //то результат умножения - 0
            if (First == 0 || Second == 0)
                return new SuperLong("0");
            SuperLong Result = Mul(First, Second);
            //Находим результат перемножения модулей и 
            //ставим знак в зависимости от знаков операндов по 
            //правилам умножения со знаком
            Result.IsNonNegative = First.IsNonNegative == Second.IsNonNegative;
            return Result;
        }

        /// <summary>
        /// Выполняет деление двух длинных чисел
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static SuperLong operator /(SuperLong First, SuperLong Second)
        {
            NormalizeForm(First);
            NormalizeForm(Second);
            //удаляем незначащие нули в записях чисел перед их сравнением
            //Сравниваем: Если первое число меньше второго, то результат - 0
            if (CompareAbs(First,Second)<0)
                return new SuperLong(0);
            //Если второй операнд - 0, то вызываем исключения деления на ноль
            if (Second == 0)
                throw new DivideByZeroException("Попытка деления на ноль.");
            SuperLong Result = Division(First, Second, out SuperLong _);
            //Вызываем деление и ставим знак по правилам выполнения деления
            //со знаком
            Result.IsNonNegative = First.IsNonNegative == Second.IsNonNegative;
            return Result;
        }

        /// <summary>
        /// Выполняет деление целого числа <see cref="int"/> на длиннное число
        /// </summary>
        /// <param name="FirstInt"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static SuperLong operator /(int FirstInt, SuperLong Second)
        {
            //Переводим первый операнд к типу SuperLong и выполняем деление
            //уже двух операндов типа SuperLong
            SuperLong First = new SuperLong(FirstInt);
            return First / Second;
        }

        /// <summary>
        /// Выполняет деление длинного числа на целое число <see cref="int"/>
        /// </summary>
        /// <param name="First"></param>
        /// <param name="SecondInt"></param>
        /// <returns></returns>
        public static SuperLong operator /(SuperLong First, int SecondInt)
        {
            //Переводим второй операнд к типу SuperLong и выполняем деление
            //уже двух операндов типа SuperLong
            SuperLong Second = new SuperLong(SecondInt);
            return First / Second;
        }

        /// <summary>
        /// Возвращает остаток от деления двух длинных чисел
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static SuperLong operator %(SuperLong First, SuperLong Second)
        {
            NormalizeForm(First);
            NormalizeForm(Second);
            if (First < Second)
                return First;
            //удаляем незначащие нули в записях чисел перед их сравнением
            //Если второй операнд - 0, то вызываем исключения деления на ноль
            if (Second == 0)
                throw new DivideByZeroException("Попытка деления на ноль.");
            SuperLong Result;
            if (CompareAbs(First, Second) > 0)
                Division(Abs(First), Abs(Second), out Result);
            else
                Division(Abs(Second), Abs(First), out Result);
            return Result;
        }

        /// <summary>
        /// Возвращает остаток от деления длинного числа на целое типа <see cref="int"/>
        /// </summary>
        /// <param name="First"></param>
        /// <param name="SecondInt"></param>
        /// <returns></returns>
        public static SuperLong operator %(SuperLong First, int SecondInt)
        {
            //Переводим второй операнд к типу SuperLong и выполняем деление
            //уже двух операндов типа SuperLong
            SuperLong Second = new SuperLong(SecondInt);
            return First % Second;
        }

        /// <summary>
        /// Выполняет приведение длинного числа к целому <see cref="int"/>. Если длинное число больше чем <see cref="int.MaxValue"/>, обрезает младшие разряды
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static explicit operator int(SuperLong Source)
        {
            SuperLong buffer = Copy(Source);
            //Создаем копию числа
            NormalizeForm(buffer);
            int Result = 0;
            for (int i = 0; i < Source.Length; i++)
                Result += (int) Math.Pow(10, i) * Source[Source.Length - 1 - i];
            //Пробегаем по всем цифрам числа и записываем их в int
            //Возможны потери, если исходное число будет больше Int32.MaxValue
            if (!buffer.IsNonNegative)
                Result *= -1;
            //Если длинное число было отрицательным, умножаем результат на -1
            return Result;
        }

        /// <summary>
        /// Выполняет приведение целого числа <see cref="int"/> к длинному
        /// </summary>
        /// <param name="Source"></param>
        public static implicit operator SuperLong(int Source)
        {
            //Передаем число в конструток, принимающий на вход целочисленный тип int
            return new SuperLong(Source);
        }

        /// <summary>
        /// Сравнивает два длинных числа
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static bool operator ==(SuperLong First, SuperLong Second)
        {

            //Если операнды равны, сравнение модулей вернет 0
            return Compare(First, Second) == 0;
        }

        /// <summary>
        /// Сравнивает два длинных числа 
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static bool operator !=(SuperLong First, SuperLong Second)
        {
            //Если операнды неравны, сравнение модулей не вернет 0
            return Compare(First, Second) != 0;
        }

        /// <summary>
        /// Сравнивает два длинных числа 
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static bool operator >(SuperLong First, SuperLong Second)
        {
            return Compare(First, Second) == 1;
        }

        /// <summary>
        /// Сравнивает два длинных числа 
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static bool operator <(SuperLong First, SuperLong Second)
        {
            return Compare(First, Second) == -1;
        }

        /// <summary>
        /// Сравнивает два длинных числа 
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static bool operator >=(SuperLong First, SuperLong Second)
        {
            return Compare(First, Second) >= 0;
        }

        /// <summary>
        /// Сравнивает два длинных числа 
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static bool operator <=(SuperLong First, SuperLong Second)
        {
            return Compare(First, Second) <= 0;
        }

        #endregion

        #region Вспомогательные методы (скрытые)

        private static void NormalizeForm(SuperLong Number)
        {
            //Пока встречается 0 в начале списка цифр и этот ноль - не последняя его 
            //цифра - удаляем этот незначащий 0
            while (Number[0] == 0 && Number.Length > 1)
                Number.NumberList.RemoveAt(0);
        }

        /// <summary>
        /// Сравнение двух длинных чисел по модулю
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static int CompareAbs(SuperLong First, SuperLong Second)
        {
            NormalizeForm(First);
            NormalizeForm(Second);
            //Нормализуем формы чисел, чтобы убрать незначащие нули
            if (First.Length > Second.Length)
                return 1;
            if (First.Length < Second.Length)
                return -1;
            //в таком представлении число, имеющее больше цифр, очевидно, по модулю будет больше
            for (int i = 0; i < First.Length; i++)
            {
                if (First[i] > Second[i])
                    return 1;
                if (First[i] < Second[i])
                    return -1;
                //в противном случае бежим по соответсвующим цифрам числа 
                //как только находим неравные - возвращаем 1 или -1
            }

            //Если пробежали по всем цифрам и ни разу не встретились разные - числа равны
            return 0;
        }

        /// <summary>
        /// Сравнение двух длинных чисел с учетом знака и null
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns>1: First больше Second. 0: First равно Second. -1: First меньше Second. </returns>
        public static int Compare(SuperLong First, SuperLong Second)
        {
            bool IsFirstNull = ReferenceEquals(First, null);
            bool IsSecondNull = ReferenceEquals(Second, null);
            if (IsFirstNull || IsSecondNull)
            {
                if (IsFirstNull && IsSecondNull) return 0;
                if (IsFirstNull) return Second.IsNonNegative ? -1 : 1;
                return IsFirstNull ? 1 : -1;
            }

            if (First.IsNonNegative != Second.IsNonNegative)
                return First.IsNonNegative ? 1 : -1;
            return First.IsNonNegative ? CompareAbs(First, Second):CompareAbs(Second,First);
        }

        private static SuperLong Plus(SuperLong First, SuperLong Second)
        {
            byte memory = 0;
            //buffer хранит в себе промежуточный результат сложения цифр
            //memory служит для хранения переноса
            SuperLong Result = new SuperLong(1)
            {
                Length = First.Length = Second.Length = Math.Max(First.Length, Second.Length) + 1
            };
            for (int i = First.Length - 1; i >= 0; i--)
            {
                //на каждом этапе складываем соответствующие цифры чисел + перенос
                byte buffer = (byte) (First[i] + Second[i] + memory);
                memory = (byte) (buffer / 10);
                //в перенос отправляем результат целочисленного деления на 10
                Result[i] = (byte) (buffer % 10);
                //в разряд ответа отправляем остаток от деления на 10
            }

            NormalizeForm(First);
            NormalizeForm(Second);
            NormalizeForm(Result);
            //восстанавливаем формы чисел и возвращаем результат
            return Result;
        }

        private static SuperLong Minus(SuperLong First, SuperLong Second)
        {
            SuperLong Result = Copy(First);
            int TempLegth = Math.Max(First.Length, Second.Length);
            Result.IsNonNegative = true;
            Second.Length = Result.Length = TempLegth;
            byte Carry = 0;
            //Carry - займ, когда результатом разности двух разрядов будет отрицательное число
            //buffer - для хранения временного результата
            for (int i = Result.Length - 1; i >= 0; i--)
            {
                sbyte buffer = (sbyte) (Result[i] - Second[i] - Carry);
                //на каждой итерации из разряда вычитаем разряд вычитаемого + займ
                if (buffer < 0)
                {
                    //Если результат отрицательный - то в ответный разряд добавляем 10
                    //и устанавливаем перенос на 1
                    Result[i] = (byte) (buffer + 10);
                    Carry = 1;
                }
                else
                {
                    Result[i] = (byte) buffer;
                    Carry = 0;
                }
            }

            //нормализуем формы чисел и возвращаем результат
            NormalizeForm(Second);
            NormalizeForm(Result);
            return Result;
        }

        private static SuperLong Mul(SuperLong First, SuperLong Second)
        {
            NormalizeForm(First);
            NormalizeForm(Second);
            SuperLong Result = new SuperLong();
            ulong[] ResBuffer = new ulong[First.Length + Second.Length + 1];
            //ResBuffer - массив для хранения результатов поразрядного умножения
            Result.Length = First.Length + Second.Length + 1;
            for (int i = Second.Length - 1, c = 0; i >= 0; i--, c++)
            for (int j = First.Length - 1, k = Result.Length - 1; j >= 0; j--, k--)
                ResBuffer[k - c] += (ulong) (First[j] * Second[i]);
            //на каждой итерации перемножаем все разряды друг с другом
            ulong remainder = 0;
            //remainder - для хранения переносов
            for (int i = Result.Length - 1; i >= 0; i--)
            {
                ResBuffer[i] += remainder;
                remainder = ResBuffer[i] / 10;
                ResBuffer[i] %= 10;
                //выполняем перенос
            }

            for (int i = Result.NumberList.Count - 1; i >= 0; i--)
                Result[i] = (byte) ResBuffer[i];
            //помещаем полученный массив в массив разрядов числа,
            //нормализуем его форму и возвращаем в качестве ответа
            NormalizeForm(Result);
            return Result;
        }

        private static SuperLong Copy(SuperLong Source)
        {
            SuperLong Result = new SuperLong {Length = Source.Length};
            //переносим все разряды из одного массива во второй
            //копируем знак и возвращаем как результат копирования
            for (int i = 0; i < Result.Length; i++)
                Result[i] = Source[i];
            Result.IsNonNegative = Source.IsNonNegative;
            return Result;
        }

        private SuperLong SubNumber(int start, int leng)
        {
            StringBuilder buffer = new StringBuilder();
            //в списке разрядов выделяем подчисло с start позиции длины leng
            for (int i = start; i < leng; i++)
                buffer.Append(NumberList[i]);
            return new SuperLong(buffer.ToString());
        }
        /// <summary>
        /// Деление чисел по модулю
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <param name="residue"></param>
        /// <returns></returns>
        private static SuperLong Division(SuperLong First, SuperLong Second, out SuperLong residue)
        {
            SuperLong BufferFirst = Copy(First);
            SuperLong Result = new SuperLong("0");
            int i = 0, leng = Second.Length;
            while (BufferFirst.SubNumber(0, leng) < Second)
                leng++;
            //подбираем начальное число так, чтобы оно было больше делителя
            //если, взяв и делимого подчисло длины делителя мы 
            //получили слишком маленькое число то берем еще одну цифру
            SuperLong Buffer = BufferFirst.SubNumber(0, leng);
            while (Second * i <= Buffer)
                i++;
            //подбираем множитель i так, чтобы при умножении на него получалось число
            //как можно более близкое к делимому подчислу
            Buffer -= Second * --i;
            Result += i;
            //доабвляем это i к ответу
            for (i = leng; i < BufferFirst.Length; i++)
            {
                Buffer.NumberList.Add(0);
                Result.NumberList.Add(0);
                //в цикле последовательно отделяем цифры от делимого
                //пока они еще остаются
                Buffer += BufferFirst[i];
                int j = 0;
                while (Second * j <= Buffer)
                    j++;
                Buffer -= Second * --j;
                Result += j;
            }

            residue = Buffer;
            //после выхода из цикла Buffer - остаток от деления
            return Result;
        }

        /// <summary>
        /// Определяет, является ли указанный объект длинным числом, совпадающим с данным
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true - является. false - не является</returns>
        public override bool Equals(object obj)
        {
            SuperLong @long = obj as SuperLong;
            return @long != null &&
                   EqualityComparer<List<byte>>.Default.Equals(NumberList, @long.NumberList) &&
                   IsNonNegative == @long.IsNonNegative;
        }

        /// <summary>
        /// Возвращает хэш-код данного длинного числа
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int hashCode = 315807058;
            hashCode = hashCode * -1521134295 + EqualityComparer<List<byte>>.Default.GetHashCode(NumberList);
            hashCode = hashCode * -1521134295 + IsNonNegative.GetHashCode();
            return hashCode;
        }

        public object Clone()
        {
            return new SuperLong(this);
        }

        #endregion
    }
}