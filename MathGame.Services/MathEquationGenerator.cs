using MathGame.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MathGame.Services
{
    public class MathEquationGenerator : IMathEquationGenerator
    {
        private const double ANSWER_TOLERANCE = 0.1d;

        private readonly IRandomGenerator _randomGenerator;

        public MathEquationGenerator(IRandomGenerator randomGenerator)
        {
            this._randomGenerator = randomGenerator;
        }

        public MathEquation GenerateEquation()
        {
            var oneSide = _randomGenerator.Int(0, 10);
            var otherSide = _randomGenerator.Int(0, 10);
            var @operator = GetOperator(_randomGenerator.Double());

            var actualResult = Math.Round(GetResult(oneSide, otherSide, @operator), 1);
            var useActualResult = _randomGenerator.Bool();

            double shownResult;

            if (useActualResult)
            {
                shownResult = actualResult;
            }
            else
            {
                shownResult = Math.Round(GenerateProbableResult(@operator, actualResult), 0);
            }

            var equationString = GetEquationString(oneSide, otherSide, shownResult, @operator);

            return new MathEquation(equationString, new[] { actualResult }, shownResult, ANSWER_TOLERANCE);
        }

        private double GenerateProbableResult(Operator @operator, double actualResult)
        {
            // Set the confusion value 0%-25% away from the actual result
            var confusionValue = actualResult * 0.25d * _randomGenerator.Double();

            if (@operator == Operator.Division)
            {
                confusionValue = Math.Round(confusionValue, 0, MidpointRounding.AwayFromZero);
            }

            if (_randomGenerator.Bool())
            {
                confusionValue = -confusionValue;
            }

            return actualResult + confusionValue;
        }

        private double GetResult(double oneSide, double otherSide, Operator @operator)
        {
            switch (@operator)
            {
                case Operator.Plus: return oneSide + otherSide;
                case Operator.Minus: return oneSide - otherSide;
                case Operator.Multiply: return oneSide * otherSide;
                case Operator.Division: return oneSide / otherSide;
            }

            throw new ArgumentException($"Invalid value {@operator}", nameof(@operator));
        }

        private string GetEquationString(int oneSide, int otherSide, double result, Operator @operator)
        {
            var strOperator = GetOperatorString(@operator);

            return $"{oneSide} {strOperator} {otherSide} = {result.ToString(CultureInfo.InvariantCulture)}";
        }

        private string GetOperatorString(Operator @operator)
        {
            switch (@operator)
            {
                case Operator.Plus: return "+";
                case Operator.Minus: return "-";
                case Operator.Multiply: return "*";
                case Operator.Division: return "/";
            }

            throw new ArgumentException($"Invalid value {@operator}", nameof(@operator));
        }

        private Operator GetOperator(double value)
        {
            if (value < 0.25d)
            {
                return Operator.Plus;
            }
            else if (value < 0.5d)
            {
                return Operator.Minus;
            }
            else if (value < 0.75d)
            {
                return Operator.Multiply;
            }

            return Operator.Division;
        }

        private enum Operator
        {
            Invalid = 0,
            Plus = 1,
            Minus = 2,
            Division = 3,
            Multiply = 4
        } 
    }
}
