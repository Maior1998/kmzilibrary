using System.Numerics;

namespace OSULib.Maths
{
    /// <summary>
    ///     Представляет статический класс для работы с Диофантовыми уравнениями 1 степени.
    /// </summary>
    public static class DiophantineEquation
    {
        /// <summary>
        ///     Метод, осуществляющий попытку решить Диофантово уравнение первой степени.
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Resultx"></param>
        /// <param name="Resulty"></param>
        /// <param name="GCD"></param>
        /// <returns></returns>
        public static bool TrySolve(BigInteger[] Source, out BigInteger[] Resultx, out BigInteger[] Resulty,
            BigInteger[] GCD = null)
        {
            BigInteger First = Source[0];
            BigInteger Second = Source[1];
            BigInteger Res = Source[2];


            Resultx = new BigInteger[2];
            Resulty = new BigInteger[2];
            if (GCD == null) GCD = AdvancedEuclidsalgorithm.GCD(First, Second);
            if (GCD == null) return false;
            if (Res % GCD[0] != 0) return false;

            Resultx = new[] {GCD[1] * (Res / GCD[0]), Second / GCD[0]};
            Resulty = new[] {GCD[2] * (Res / GCD[0]), -(First / GCD[0])};
            //Надо сделать решение наименьшим по модулю, но
            //Это возможно только когда оба свободных члена уменьшаются
            //по модулю при свободном t

            while (BigInteger.Abs(Resultx[0] + Resultx[1]) < BigInteger.Abs(Resultx[0]) &&
                   BigInteger.Abs(Resulty[0] + Resulty[1]) < BigInteger.Abs(Resulty[0]))
            {
                Resultx[0] += Resultx[1];
                Resulty[0] += Resulty[1];
            }

            while (BigInteger.Abs(Resultx[0] - Resultx[1]) < BigInteger.Abs(Resultx[0]) &&
                   BigInteger.Abs(Resulty[0] - Resulty[1]) < BigInteger.Abs(Resulty[0]))
            {
                Resultx[0] -= Resultx[1];
                Resulty[0] -= Resulty[1];
            }

            return true;
        }
    }
}