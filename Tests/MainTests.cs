using System;
using System.Linq;
using KMZILib;
using static KMZILib.Ciphers.Languages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}
