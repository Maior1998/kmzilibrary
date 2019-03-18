using KMZILib;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Ciphers = KMZILib.Ciphers;
using Languages = KMZILib.Ciphers.Languages;
using Polynoms = KMZILib.Polynoms;
using ModularPolynom=KMZILib.Polynoms.ModularPolynom;

namespace Ручной_тест
{
    class Program
    {

        public static double[] Mp = {2.0, 12, 34, 40, 10, 2};
        public static double[] Mt = {1.83, 12.99, 35.08, 35.08, 12.99, 1.861};

        static void Main(string[] args)
        {
            Console.WriteLine($"Распределение {(MathStatistics.IsNormal(Mp,Mt)?"":"не")} подчиняется нормальному закану распределения");



        }

        


        public static string Cipher1 = "БЦКНКВСФНТХБЦЫЫЕМЛ";
        public static string Cipher2 = "ФЩРИОЯЫЩЮЧЗНЧЯКШЦИ";
        public static string Es = "ЭСКАДРА";
        public static void kmzitask()
        {
            Languages.CurrentLanguage = Languages.RussianLanguage.GetInstanse();
            Languages.CurrentLanguage.Alphabet = "АБВГДЕЖЗИКЛМНОПРСТУФХЦЧШЩЬЫЭЮЯ";
            Console.WriteLine(Ciphers.VigenereCipher.Decrypt(Cipher1, "ЧИЖИКПЫЖИКГДЕТЫБЫЛ"));
            Console.WriteLine(Ciphers.VigenereCipher.Decrypt(Cipher2, "ЧИЖИКПЫЖИКГДЕТЫБЫЛ"));
            for (int KeyLength = 10; KeyLength <= Cipher1.Length; KeyLength++)
            {

                Console.WriteLine($"Длины ключа: {KeyLength}");
                for (int FirstPostion = 0; FirstPostion <= Cipher1.Length - Es.Length; FirstPostion++)
                    for (int SecondPostion = 0; SecondPostion <= Cipher2.Length - Es.Length; SecondPostion++)
                    {
                        Dictionary<int, char> PossibleKey = new Dictionary<int, char>();
                        for (int i = 0; i < KeyLength; i++)
                            PossibleKey.Add(i, '0');
                        for (int CurrentPos = 0; CurrentPos < Es.Length; CurrentPos++)
                        {
                            //Идем одновременно по двум словам
                            int FirstAbsoluteIndex = FirstPostion + CurrentPos;
                            int SecondAbsoluteIndex = SecondPostion + CurrentPos;
                            //это индексы текущих оперируемых символов. Например, "Э". ПОтом "С", "К"...
                            int FirstKeyIndex = FirstAbsoluteIndex % KeyLength;
                            int SecondKeyIndex = SecondAbsoluteIndex % KeyLength;
                            //Это индексы символов ключа в этих позициях

                            char FirstKeyPart = Ciphers.VigenereCipher.GetDecryptedChar(Cipher1[FirstAbsoluteIndex],
                                Es[CurrentPos]);
                            char SecondKeyPart = Ciphers.VigenereCipher.GetDecryptedChar(Cipher2[SecondAbsoluteIndex],
                                Es[CurrentPos]);
                            if (PossibleKey[FirstAbsoluteIndex % KeyLength] == '0')
                                PossibleKey[FirstAbsoluteIndex % KeyLength] = FirstKeyPart;
                            if (PossibleKey[FirstAbsoluteIndex % KeyLength] != FirstKeyPart)
                            {
                                PossibleKey[FirstAbsoluteIndex % KeyLength] = '1';
                                break;
                            }

                            if (PossibleKey[SecondAbsoluteIndex % KeyLength] == '0')
                                PossibleKey[SecondAbsoluteIndex % KeyLength] = SecondKeyPart;
                            if (PossibleKey[SecondAbsoluteIndex % KeyLength] != SecondKeyPart)
                            {
                                PossibleKey[SecondAbsoluteIndex % KeyLength] = '1';
                                break;
                            }
                        }
                        if (PossibleKey.ContainsValue('1'))
                            continue;

                        for (int i = 0; i < KeyLength; i++)
                            Console.Write(PossibleKey[i] == '0' ? ' ' : PossibleKey[i]);
                        Console.WriteLine();
                    }
            }
        }
    }
}


