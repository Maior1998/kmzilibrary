using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMZILib.Misc
{
    /// <summary>
    /// Разная всячина. Полезные функции и методы.
    /// </summary>
    public static partial class Misc
    {
        /// <summary>
        /// Гиперболический синус.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double sh(double x) => (Math.Exp(x)-Math.Exp(-x))/2;
    }
}
