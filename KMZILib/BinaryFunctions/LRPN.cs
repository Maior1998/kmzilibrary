using System;
using System.Collections.Generic;
using System.Linq;

namespace KMZILib
{
    public partial class BinaryFunction
    {
        private static class LRPN
        {
            public static class CalcLRPN
            {
                private static bool[][] Solve(string LRPNExpression, out char[] NamesOfVariables)
                {
                    List<KeyValuePair<char, bool>> Variables = new List<KeyValuePair<char, bool>>();
                    foreach (char symbol in LRPNExpression)
                    {
                        if (!char.IsLetter(symbol))
                            continue;
                        if (Variables.TrueForAll(pair => pair.Key != symbol))
                            Variables.Add(new KeyValuePair<char, bool>(symbol, false));
                    }

                    Variables = Variables.OrderBy(pair => VariablesAlphabet.IndexOf(pair.Key)).ToList();
                    bool[][] AnswerArray = new bool[(int) Math.Pow(2, Variables.Count)][];
                    for (int i = 0; i < Math.Pow(2, Variables.Count); i++)
                    {
                        SetByNumber(Variables, i);
                        Stack<bool> Operands = new Stack<bool>();
                        for (int j = 0; j < LRPNExpression.Length; j++)
                        {
                            if (LRPNExpression[j] == ' ')
                                continue;
                            if (char.IsLetter(LRPNExpression[j]))
                            {
                                //TODO: неверно задается индекс.
                                Operands.Push(Variables.First(variable => variable.Key == LRPNExpression[j]).Value);
                                continue;
                            }

                            bool FirstOperand;
                            bool SecondOperand;
                            switch (LRPNExpression[j])
                            {
                                case '+':
                                    SecondOperand = Operands.Pop();
                                    FirstOperand = Operands.Pop();
                                    Operands.Push(FirstOperand || SecondOperand);
                                    break;
                                case '*':
                                    SecondOperand = Operands.Pop();
                                    FirstOperand = Operands.Pop();
                                    Operands.Push(FirstOperand && SecondOperand);
                                    break;
                                case '<':
                                    j += 2;
                                    SecondOperand = Operands.Pop();
                                    FirstOperand = Operands.Pop();
                                    Operands.Push(FirstOperand == SecondOperand);
                                    break;
                                case '-':
                                    j++;
                                    SecondOperand = Operands.Pop();
                                    FirstOperand = Operands.Pop();
                                    Operands.Push(FirstOperand || !SecondOperand);
                                    break;
                                case '!':
                                    FirstOperand = Operands.Pop();
                                    Operands.Push(!FirstOperand);
                                    break;
                            }
                        }

                        AnswerArray[i] = new bool[Variables.Count + 1];
                        Variables.Select(pair => pair.Value).ToArray().CopyTo(AnswerArray[i], 0);
                        AnswerArray[i][AnswerArray[i].Length - 1] = Operands.Pop();
                    }

                    NamesOfVariables = Variables.Select(pair => pair.Key).ToArray();
                    return AnswerArray;
                }

                public static bool[] GetValues(string Expression)
                {
                    bool[][] a = Solve(LRPNParser.GetRPN(Expression), out char[] _);
                    return a.Select(row => row.Last()).ToArray();
                }

                private static void SetByNumber(List<KeyValuePair<char, bool>> VariablesList, int Number)
                {
                    //необходимо заполнить значения списка KeyValuePair двоичными кодами числа Number
                    bool[] a = GetBinaryArray(Number, VariablesList.Count);
                    IList<char> Keys = VariablesList.Select(pair => pair.Key).ToList();
                    for (int i = 0; i < Keys.Count; i++)
                        VariablesList[i] = new KeyValuePair<char, bool>(Keys[i], a[i]);
                }
            }

            //Logical Reverse Polish Notation Parser
            private static class LRPNParser
            {
                public static string GetRPN(string Input)
                {
                    while (Input.IndexOf(' ') != -1)
                        Input = Input.Remove(Input.IndexOf(' '), 1);
                    //удаляем все пробелы в выражении
                    return GetLevel1(Input);
                }

                private static string GetLevel1(string Input)
                {
                    for (int i = Input.Length - 1; i >= 0; i--)
                    {
                        //Бежим по строке справа налево, пропуская скобки, в поисках эквивалентности
                        //Если находим эквивалентность - разделяем. Если нет - просто отдаем следующему уровню.
                        if (char.IsLetter(Input, i))
                            continue;
                        if (Input[i] == ')')
                        {
                            Stack<byte> Brackets = new Stack<byte>(new byte[] {0});
                            i--;
                            while (i >= 0 && Brackets.Count != 0)
                            {
                                if (Input[i] == ')')
                                    Brackets.Push(0);
                                else if (Input[i] == '(')
                                    Brackets.Pop();
                                i--;
                            }

                            i++;
                            continue;
                        }

                        if (Input[i] == '>' && i > 1 && Input.Substring(i - 2, 3) == "<->")
                            return
                                $"{GetLevel2(Input.Substring(i + 1, Input.Length - i - 1))} {GetLevel1(Input.Substring(0, i - 2))} <->";
                        //(распологаем их в обратном порядке, потому что обратная польская запись)
                        //(если вам здесь чего-то не понятно, то не волнуйтесь, я тоже уже нихера не понимаю)
                    }

                    //Если ничего не нашли - отдаем все выражение следующему уровню
                    return GetLevel2(Input);
                }

                private static string GetLevel2(string Input)
                {
                    for (int i = Input.Length - 1; i >= 0; i--)
                    {
                        //Бежим по строке справа налево, пропуская скобки, в поисках импликации
                        //Если находим импликацию - разделяем. Если нет - просто отдаем следующему уровню.
                        if (char.IsLetter(Input, i))
                            continue;
                        if (Input[i] == ')')
                        {
                            Stack<byte> Brackets = new Stack<byte>(new byte[] {0});
                            i--;
                            while (i >= 0 && Brackets.Count != 0)
                            {
                                if (Input[i] == ')')
                                    Brackets.Push(0);
                                else if (Input[i] == '(')
                                    Brackets.Pop();
                                i--;
                            }

                            i++;
                            continue;
                        }

                        if (Input[i] == '>' && i > 0 && Input.Substring(i - 1, 2) == "->")
                            return
                                $"{GetLevel3(Input.Substring(i + 1, Input.Length - i - 1))} {GetLevel2(Input.Substring(0, i - 1))} ->";
                    }

                    //Если ничего не нашли - отдаем все выражение следующему уровню
                    return GetLevel3(Input);
                }

                private static string GetLevel3(string Input)
                {
                    for (int i = Input.Length - 1; i >= 0; i--)
                    {
                        //Бежим по строке справа налево, пропуская скобки, в поисках сложения
                        //Если находим сложение - разделяем. Если нет - просто отдаем следующему уровню.
                        if (char.IsLetter(Input, i))
                            continue;
                        if (Input[i] == ')')
                        {
                            Stack<byte> Brackets = new Stack<byte>(new byte[] {0});
                            i--;
                            while (i >= 0 && Brackets.Count != 0)
                            {
                                if (Input[i] == ')')
                                    Brackets.Push(0);
                                else if (Input[i] == '(')
                                    Brackets.Pop();
                                i--;
                            }

                            i++;
                            continue;
                        }

                        if (Input[i] == '+')
                        {
                            return
                                $"{GetLevel4(Input.Substring(i + 1, Input.Length - i - 1))} {GetLevel3(Input.Substring(0, i))} +";
                        }
                    }

                    //Если ничего не нашли - отдаем все выражение следующему уровню
                    return GetLevel4(Input);
                }

                private static string GetLevel4(string Input)
                {
                    for (int i = Input.Length - 1; i >= 0; i--)
                    {
                        //Бежим по строке справа налево, пропуская скобки, в поисках умножения
                        //Если находим умножение - разделяем. Если нет - просто отдаем следующему уровню.
                        if (char.IsLetter(Input, i))
                            continue;
                        if (Input[i] == ')')
                        {
                            Stack<byte> Brackets = new Stack<byte>(new byte[] {0});
                            i--;
                            while (i >= 0 && Brackets.Count != 0)
                            {
                                if (Input[i] == ')')
                                    Brackets.Push(0);
                                else if (Input[i] == '(')
                                    Brackets.Pop();
                                i--;
                            }

                            i++;
                            continue;
                        }

                        if (Input[i] == '*')
                            return
                                $"{GetLevel5(Input.Substring(i + 1, Input.Length - i - 1))} {GetLevel4(Input.Substring(0, i))} *";
                    }

                    //Если ничего не нашли - отдаем все выражение следующему уровню
                    return GetLevel5(Input);
                }

                private static string GetLevel5(string Input)
                {
                    //Тут два варианта, либо перед оставшимся выражением стоит !, либо не стоит
                    if (Input[0] == '!')
                        return $"{GetLevel6(Input.Substring(1, Input.Length - 1))} !";
                    return GetLevel6(Input);
                }

                private static string GetLevel6(string Input)
                {
                    //Два варианта: Либо это конечная остановочка, тут просто буква и мы закончили
                    //Либо это в скобках еще одно выражение
                    if (Input[0] == '(' && Input[Input.Length - 1] == ')')
                        return GetLevel1(Input.Substring(1, Input.Length - 2));
                    return Input;
                }
            }
        }
    }
}