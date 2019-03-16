﻿using KMZILib;

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



        static void Main(string[] args)
        {
            //Polynoms.ModularPolynom test1=new Polynoms.ModularPolynom("x^4 + x^3 + x^2 + 3",7);
            //Polynoms.ModularPolynom test2 = new Polynoms.ModularPolynom("x^2 + x + 3", 7);

            Console.WriteLine(new ModularPolynom("6x^2 + 2x + 6",11)/new ModularPolynom("9x",11));
            Console.WriteLine(new ModularPolynom("6x^2 + 2x + 6", 11) % new ModularPolynom("9x", 11));
            

            Polynoms.ModularPolynom test1 = new Polynoms.ModularPolynom("7x^5 + 4x^3 + 2x + 1", 11);
            Polynoms.ModularPolynom test2 = new Polynoms.ModularPolynom("5x^3 + 2", 11);
            Polynoms.ModularPolynom test3 = new Polynoms.ModularPolynom("-3x", 7);
            Polynoms.ModularPolynom a =AdvancedEuclidsalgorithm.GCDResult(test1,test2);



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


