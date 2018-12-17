using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static KMZILib.Comparison;

namespace KMZILib
{
    /// <summary>
    ///     Класс для работы с числами многомодульной арифметики. (MultiModular Arithmetic)
    /// </summary>
    public class MMA
    {
        /// <summary>
        /// Определяет, совпадает ли данное число сдругим числом <see cref="MMA"/>
        /// </summary>
        /// <param name="other">Сравниваемое число класса <see cref="MMA"/></param>
        /// <returns>true - Совпадают. false - Что-то отличается</returns>
        protected bool Equals(MMA other)
        {
            return Equals(Values, other.Values);
        }

        /// <summary>
        /// Определяет, является указанный объект числом <see cref="MMA"/>, совпадающим с данным
        /// </summary>
        /// <param name="obj">Сравниваемый объект</param>
        /// <returns>true - Является. false - не является</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((MMA) obj);
        }

        /// <summary>
        /// Возвращает хэш-код данного числа
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (Values != null ? Values.GetHashCode() : 0);
        }

        /// <summary>
        ///     Список сравнений многомодульного числа, содержащий как остатки деления числа на соответствующий модуль, так и сами
        ///     модули
        /// </summary>
        private List<Comparison.LinearComparison> Values = new List<Comparison.LinearComparison>();

        /// <summary>
        /// Инициализирует новое многомодульное число по заданному набору сравнений
        /// </summary>
        /// <param name="comparisons"></param>
        public MMA(IEnumerable<Comparison.LinearComparison> comparisons)
        {
            foreach (Comparison.LinearComparison comparison in comparisons)
                Values.Add(new Comparison.LinearComparison(comparison.A, comparison.M));
        }

        /// <summary>
        ///     Список остатков многомодульного числа
        /// </summary>
        public List<BigInteger> Remainders => Values.Select(value => value.A).ToList();

        /// <summary>
        ///     Список модулей многомодульного числа
        /// </summary>
        public List<BigInteger> Modules => Values.Select(value => value.M).ToList();

        /// <summary>
        ///     Знак многомодульного числа. 0 - положительное. 1 - отрицательное.
        /// </summary>
        public int Sign
        {
            get
            {
                //В противном случае, мы можем определить знак только если у нас есть двойка в модулях
                if (!Modules.Contains(2))
                    throw new InvalidOperationException(
                        "Нахождение знака возможно только при наличии модуля 2 в составе числа!");

                //создаем копию текущего набора сравнений, так как в процессе вычисления знака надо будет этот набор изменять
                List<Comparison.LinearComparison> buffer = new List<Comparison.LinearComparison>();
                foreach (Comparison.LinearComparison comparison in Values)
                    buffer.Add(new Comparison.LinearComparison(comparison.A, comparison.M));

                //Сортируем набор в порядке убывания модулей, чтобы двойка оказалась в самом его конце, как нужно по алгоритму
                buffer=buffer.OrderByDescending(element => element.M).ToList();


                for (int i = 0; i < buffer.Count; i++)
                {
                    //набор оставшихся ненулевыхз модулей
                    IEnumerable<Comparison.LinearComparison> a = buffer.Skip(i);
                    //Если все имеющиеся на данный момент остатки равны 0 - нет смысла считать дальше - возвращаем 0.
                    if (a.All(comparison => comparison.A == 0)) return 0;
                    //сначала нужно отнять первый ненулевой остаток у всех остальных сравнений
                    BigInteger firstremainder = buffer[i].A;
                    //из из него самого тоже, так что просто его обнуляем
                    buffer[i].A = 0;
                    //начинаем идти от следующего остатка 
                    for (int j = i + 1; j < buffer.Count; j++)
                    {
                        //отнимаем сохраненный остаток
                        buffer[j].A -=
                            firstremainder; //Вычли везде первый ненулевой остаток, готовы умножать на обратный
                        //и умножаем на обратный элемент
                        buffer[j].A *= MultiplicativeInverse.Solve(buffer[i].M, buffer[j].M);
                    }
                }

                //Если за все итерации все остатки так и не были равны 0 - значит, число отрицательное.
                return 1;
            }
        }

        /// <summary>
        ///     Максимальное по модулю значение, которое может принять число при текущем наборе модулей.
        /// </summary>
        public int MaxValue => (Module - 1) / 2;

        /// <summary>
        ///     Общий модуль числа.
        /// </summary>
        public int Module
        {
            get
            {
                if (Modules.Count == 0) return 0;
                int res = 1;
                foreach (int module in Modules)
                    res *= module;
                return res;
            }
        }

        /// <summary>
        ///     Сравнение по текущего многомодульного числа по полному модулю. Вычисляется за счет ГКТ (<see cref="CRT" />)
        /// </summary>
        public Comparison.LinearComparison ComparisonValue => CRT.Solve(Values);

        /// <summary>
        ///     Наименьшее по модулю значение числа. Вычисляется за счет ГКТ (<see cref="CRT" />)
        /// </summary>
        public BigInteger Value => CRT.Solve(Values).LeastModulo;

        /// <summary>
        ///     Наименьшее неотрицательно значение числа. Вычисляется за счет ГКТ (<see cref="CRT" />)
        /// </summary>
        public BigInteger ABSValue => CRT.Solve(Values).SmallestNonnegative;

        /// <summary>
        /// Метод, добавляющий модуль в многомодульное число.
        /// </summary>
        /// <param name="comparison"></param>
        /// <exception cref="InvalidOperationException">Модуль уже присутствует в числе</exception>
        public void AddModule(Comparison.LinearComparison comparison)
        {
            if (Modules.Contains(comparison.M))
                throw new InvalidOperationException("Такой модуль уже присутствует в числе!");
            Values.Add(comparison);
            Values = Values.OrderByDescending(element => element.M).ToList();
        }

        /// <summary>
        /// Сложение двух многомодульных чисел
        /// </summary>
        /// <param name="First">Первый операнд</param>
        /// <param name="Second">Второй операнд</param>
        /// <returns>Результат операции</returns>
        public static MMA operator +(MMA First, MMA Second)
        {
            if (First.Modules.Count != Second.Modules.Count)
                throw new ArgumentException("Наборы модулей чисел не совпадают!");
            First.Sort();
            Second.Sort();
            MMA Result = new MMA(new List<Comparison.LinearComparison>());
            for (int i = 0; i < First.Values.Count; i++)
            {
                if (First.Values[i].M != Second.Values[i].M)
                    throw new ArgumentException("Наборы модулей чисел не совпадают!");
                Result.AddModule(new Comparison.LinearComparison(First.Values[i].A + Second.Values[i].A,
                    First.Values[i].M));
            }

            return Result;
        }

        /// <summary>
        /// Разность двух многомодульных чисел
        /// </summary>
        /// <param name="First">Первый операнд</param>
        /// <param name="Second">Второй операнд</param>
        /// <returns>Результат операции</returns>
        public static MMA operator -(MMA First, MMA Second)
        {
            if (First.Modules.Count != Second.Modules.Count)
                throw new ArgumentException("Наборы модулей чисел не совпадают!");
            First.Sort();
            Second.Sort();
            MMA Result = new MMA(new List<Comparison.LinearComparison>());
            for (int i = 0; i < First.Values.Count; i++)
            {
                if (First.Values[i].M != Second.Values[i].M)
                    throw new ArgumentException("Наборы модулей чисел не совпадают!");
                Result.AddModule(new Comparison.LinearComparison(First.Values[i].A - Second.Values[i].A,
                    First.Values[i].M));
            }

            return Result;
        }

        /// <summary>
        /// Произведение двух многомодульных чисел
        /// </summary>
        /// <param name="First">Первый операнд</param>
        /// <param name="Second">Второй операнд</param>
        /// <returns>Результат операции</returns>
        public static MMA operator *(MMA First, MMA Second)
        {
            if (First.Modules.Count != Second.Modules.Count)
                throw new ArgumentException("Наборы модулей чисел не совпадают!");
            First.Sort();
            Second.Sort();
            MMA Result = new MMA(new List<Comparison.LinearComparison>());
            for (int i = 0; i < First.Values.Count; i++)
            {
                if (First.Values[i].M != Second.Values[i].M)
                    throw new ArgumentException("Наборы модулей чисел не совпадают!");
                Result.AddModule(new Comparison.LinearComparison(First.Values[i].A * Second.Values[i].A,
                    First.Values[i].M));
            }

            return Result;
        }

        /// <summary>
        /// Деление двух многомодульных чисел
        /// </summary>
        /// <param name="First">Первый операнд</param>
        /// <param name="Second">Второй операнд</param>
        /// <returns>Результат операции</returns>
        public static MMA operator /(MMA First, MMA Second)
        {
            if (First.Modules.Count != Second.Modules.Count)
                throw new ArgumentException("Наборы модулей чисел не совпадают!");
            First.Sort();
            Second.Sort();
            MMA Result = new MMA(new List<Comparison.LinearComparison>());
            for (int i = 0; i < First.Values.Count; i++)
            {
                if (First.Values[i].M != Second.Values[i].M)
                    throw new ArgumentException("Наборы модулей чисел не совпадают!");
                Result.AddModule(new Comparison.LinearComparison(First.Values[i].A / Second.Values[i].A,
                    First.Values[i].M));
            }

            return Result;
        }

        /// <summary>
        /// Сравнивает набор сравнений двух многомодульных чисел
        /// </summary>
        /// <param name="First">Сравниваемое число</param>
        /// <param name="Second">Сравниваемое число</param>
        /// <returns>true - наборы совпадают. false - отличаются</returns>
        public static bool operator ==(MMA First, MMA Second)
        {
            if (First.Values.Count != Second.Values.Count) return false;
            First.Sort();
            Second.Sort();
            return !First.Values.Where((t, i) => t != Second.Values[i]).Any();
        }

        private void Sort()
        {
            Values.OrderBy(element => element.M);
        }

        /// <summary>
        /// Сравнивает набор сравнений двух многомодульных чисел
        /// </summary>
        /// <param name="First">Сравниваемое число</param>
        /// <param name="Second">Сравниваемое число</param>
        /// <returns>true - наборы отличаются. false - совпадают</returns>
        public static bool operator !=(MMA First, MMA Second)
        {
            if (First.Values.Count != Second.Values.Count) return true;
            First.Sort();
            Second.Sort();
            return First.Values.Where((t, i) => t != Second.Values[i]).Any();
        }

        /// <summary>
        /// Возвращает строковое представление данного многомодульюного числа по общему модулю
        /// </summary>
        /// <returns>Строка, представляющая данное многомодульное число</returns>
        public override string ToString()
        {
            return $"x ~ ({string.Join(", ", Remainders)}) mod({string.Join(", ", Modules)})";
        }
    }
}