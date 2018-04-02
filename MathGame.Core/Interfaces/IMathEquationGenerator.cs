using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace MathGame.Core.Interfaces
{
    public interface IMathEquationGenerator
    {
        MathEquation GenerateEquation();
    }

    public class MathEquation
    {
        public string EquationString { get; }
        public double ShownResult { get; }
        public IEnumerable<double> ActualResults { get; }
        public double Tolerance { get; }

        public MathEquation(string equationString, IEnumerable<double> actualResults, double shownResult, double tolerance)
        {
            this.EquationString = equationString ?? throw new ArgumentNullException(nameof(equationString));
            this.ShownResult = shownResult;
            this.Tolerance = tolerance;
            this.ActualResults = actualResults?.ToImmutableArray() ?? throw new ArgumentNullException(nameof(actualResults));
        }

        public bool CheckResult(double result)
        {
            return ActualResults
                .Any(x => Math.Abs(x - result) <= Tolerance);
        }
    }
}
