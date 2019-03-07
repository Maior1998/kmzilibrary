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

namespace Ручной_тест
{
    class Program
    {

        
        public static string Cipher1 = "БЦКНКВСФНТХБЦЫЫЕМЛ";
        public static string Cipher2 = "ФЩРИОЯЫЩЮЧЗНЧЯКШЦИ";
        public static string Es = "ЭСКАДРА";
        static void Main(string[] args)
        {


            Vector init = new Vector(new[] { 1.0, 0, 1, 0, 1 });
            CRS.MLFSR test = new CRS.MLFSR("an+5=2an+4+2an+1",3,new[]{1,0,1,0,1});
            Console.WriteLine(test.InitializeVector);
            for (int i = 0; i < 255; i++)
            {
                Console.WriteLine(test.CurrentState);
                test.GetNext(false);
                Vector current = test.StateVector;
                if (test.StatesHistory.Contains(current))
                {
                    Console.WriteLine($"Период найден на {i+1} шаге");
                    Console.WriteLine(test.CurrentState);
                    Console.WriteLine($"({string.Join(", ", test.Values)})");
                    Console.WriteLine($"Общая последовательность:\n{string.Join(" ",test.ValuesHistory)}");
                    break;
                }
                test.StatesHistory.Enqueue(current);
            }
            return;
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


