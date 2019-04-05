using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMZILib
{
    /// <summary>
    /// Разная всячина. Полезные функции и методы.
    /// </summary>
    public static partial class Misc
    {
        public static string GetSignedValue(double Source, int CountOfDigits=-1)
        {
            return $"{(Source >= 0 ? "+" : "-")} {(CountOfDigits==-1? Math.Abs(Source): Math.Abs(Math.Round(Source,CountOfDigits)))}";
        }

        public static string GetSignedValue(int Source)
        {
            return $"{(Source >= 0 ? "+" : "-")} {Math.Abs(Source)}";
        }
    }
}
