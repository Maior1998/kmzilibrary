using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using KMZILib;

namespace Ручной_тест
{
    class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<Ciphers.SecretSharing.CRT.AsmuthBloomScheme.CRTPart> a = Ciphers.SecretSharing.CRT.AsmuthBloomScheme.Share(17, 5, 4);
            Console.WriteLine(Ciphers.SecretSharing.CRT.AsmuthBloomScheme.Restore(a.ToList(),4));
        }
    }
}
