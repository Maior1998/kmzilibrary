using System;
using System.Linq;

namespace OSULib.Maths.GamesTheory
{
    public class GameWithNature
    {
        /// <summary>
        /// Представляет собой матрицу весов выйгрышей (A).
        /// </summary>
        private readonly Matrix strategiesWeights;
        public GameWithNature(Matrix strategiesWeights)
        {
            this.strategiesWeights = strategiesWeights.Copy();
            //double min = this.strategiesWeights.GetMin();
            //if (min < 0)
            //    this.strategiesWeights.IncrementBy(Math.Abs(min));
        }

        private Matrix risksMatrix;
        /// <summary>
        /// Матрица рисков.
        /// </summary>
        public Matrix RisksMatrix
        {
            get
            {
                if (risksMatrix != null)
                    return risksMatrix;

                Matrix bufferRiskMatrix = new Matrix(strategiesWeights.Values.GetLength(0), strategiesWeights.Values[0].Length);
                for (int j = 0; j < strategiesWeights.Values[0].Length; j++)
                {
                    double maxInColumn = strategiesWeights.GetMaxAbsInColumn(j);

                    for (int i = 0; i < strategiesWeights.Values.GetLength(0); i++)
                    {
                        bufferRiskMatrix[i, j] = maxInColumn - strategiesWeights[i, j];
                    }
                }
                return risksMatrix = bufferRiskMatrix;
            }

        }

        private Matrix lossMatrix;
        /// <summary>
        /// Матрица потерь.
        /// </summary>
        public Matrix LossMatrix
        {
            get
            {
                if (lossMatrix != null)
                    return lossMatrix;
                lossMatrix = strategiesWeights.GetMultipliedCopy(-1);
                return lossMatrix;
            }
        }

        /// <summary>
        /// Критерий макси-макса по матрице стратегий.
        /// </summary>
        public double StrategiesMaxMaxCriteria => StrategiesGetGurwitsCriteria(0);

        /// <summary>
        /// Критерий макси-макса по матрице рисков.
        /// </summary>
        public double RisksMaxMaxCriteria => RisksGetGurwitsCriteria(0);

        /// <summary>
        /// Критерий Вальда по матрице стратегий.
        /// </summary>
        public double StrategiesWaldsMaximinModel => StrategiesGetGurwitsCriteria(1);

        /// <summary>
        /// Критерий Вальда по матрице рисков.
        /// </summary>
        public double RisksWaldsMaximinModel => RisksGetGurwitsCriteria(1);

        /// <summary>
        /// Вычисляет критерий Гурвица для заданного уровня пессимизма по матрице стратегий.
        /// </summary>
        /// <param name="pessimismLevel">Уровень пессимизма, для которого необходимо рассчитать критерий.</param>
        /// <returns>Вещественное число - критерий Гурвица.</returns>
        public double StrategiesGetGurwitsCriteria(double pessimismLevel)
        {
            if (pessimismLevel < 0 || pessimismLevel > 1)
                throw new InvalidOperationException("Уровень пессимизма должен лежать в отрезке от 0 до 1.");
            return strategiesWeights.Values.Select(x => pessimismLevel * x.Min() + (1 - pessimismLevel) * x.Max()).Max();
        }

        /// <summary>
        /// Вычисляет критерий Гурвица для заданного уровня пессимизма по матрице рисков.
        /// </summary>
        /// <param name="pessimismLevel">Уровень пессимизма, для которого необходимо рассчитать критерий.</param>
        /// <returns>Вещественное число - критерий Гурвица.</returns>
        public double RisksGetGurwitsCriteria(double pessimismLevel)
        {
            if (pessimismLevel < 0 || pessimismLevel > 1)
                throw new InvalidOperationException("Уровень пессимизма должен лежать в отрезке от 0 до 1.");
            return RisksMatrix.Values.Select(x => pessimismLevel * x.Max() + (1 - pessimismLevel) * x.Min()).Min();
        }

        /// <summary>
        /// Критерий Сэвиджа.
        /// </summary>
        public double SavageCriteria => RisksMatrix.Values.Select(x => x.Max()).Min();

        /// <summary>
        /// Получает критерий Байеса для заданного массива вероятностей по матрице стратегий.
        /// </summary>
        /// <param name="probabilities">Массив вероятностей стратегий погоды.</param>
        /// <returns>Критерий Байеса.</returns>
        public double StrategiesGetBayesCriteria(double[] probabilities)
        {
            if (probabilities.Length != strategiesWeights.Values.First().Length)
                throw new InvalidOperationException("Длина массива вероятностей не равна числу стратегий природы!");
            return strategiesWeights.Values
                .Select(x => x.Select((value, index) => value * probabilities[index]).Sum()).Max();
        }

        /// <summary>
        /// Получает критерий Байеса для заданного массива вероятностей по матрице рисков.
        /// </summary>
        /// <param name="probabilities">Массив вероятностей стратегий погоды.</param>
        /// <returns>Критерий Байеса.</returns>
        public double RisksGetBayesCriteria(double[] probabilities)
        {
            if (probabilities.Length != strategiesWeights.Values.First().Length)
                throw new InvalidOperationException("Длина массива вероятностей не равна числу стратегий природы!");
            return RisksMatrix.Values.Select(x => x.Select((value, index) => value * probabilities[index]).Sum()).Min();
        }

        /// <summary>
        /// Простой критерий Байеса по матрице стратегий, когда все стратегии природы равновероятны.
        /// </summary>
        public double StrategiesSimpleBayesCriteria => StrategiesGetBayesCriteria(Enumerable.Range(0, strategiesWeights.LengthX).Select(x => 1d / strategiesWeights.LengthX).ToArray());

        /// <summary>
        /// Простой критерий Байеса по матрице рисков, когда все стратегии природы равновероятны.
        /// </summary>
        public double RisksSimpleBayesCriteria => RisksGetBayesCriteria(Enumerable.Range(0, strategiesWeights.LengthX).Select(x => 1d / strategiesWeights.LengthX).ToArray());


    }
}
