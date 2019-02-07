using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using KMZILib;
using static KMZILib.Ciphers.Languages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static KMZILib.Ciphers.SecretSharing.CRT;

namespace Tests
{
    [TestClass]
    public class MainTests
    {
        [TestMethod]
        public void LanguageOneInstance()
        {
            ALanguage firstrus = RussianLanguage.GetInstanse();
            ALanguage secondrus = RussianLanguage.GetInstanse();

            ALanguage firsteng = EnglishLanguage.GetInstanse();
            ALanguage secondeng = EnglishLanguage.GetInstanse();

            Assert.AreEqual(firstrus, secondrus);
            Assert.AreEqual(firsteng, secondeng);
        }

        [TestMethod]
        public void LanguageSearchBySymbol()
        {
            foreach (ALanguage language in LanguagesList)
                foreach (char symbol in language.Alphabet)
                    Assert.AreEqual(LangByChar(RD.Rand.Next(0, 2) == 0 ? symbol : char.ToLower(symbol)), language);
        }

        [TestMethod]
        public void SecretSharingCRT()
        {
            BigInteger Key = RD.Rand.Next(0, 1000);
            int Count = RD.Rand.Next(3, 16);
            int Limit = RD.Rand.Next(2, Count + 1);
            Assert.AreEqual(Key, AsmuthBloomScheme.Restore(AsmuthBloomScheme.Share(Key, Count, Limit).ToList(), Limit));

        }
    }
}
