using MathGame.Services;
using System;
using System.Linq;
using Xunit;

namespace MathGame.Test
{
    public class MathEquationGeneratorTest
    {
        [Fact]
        public void Generates_Correct_String_OperatorPlus_ActualResult()
        {
            var randomGen = new MockRandomGenerator();
            var generator = new MathEquationGenerator(randomGen);

            randomGen.NextInts = new[] { 5, 6 };
            randomGen.NextDoubles = new[] { 0d };
            randomGen.NextBools = new[] { true };

            var equation = generator.GenerateEquation();

            Assert.Equal("5 + 6 = 11", equation.EquationString);
            Assert.Equal(11d, equation.ShownResult, 1);
            Assert.Single(equation.ActualResults);
            Assert.Equal(11d, equation.ActualResults.First(), 1);
        }

        [Fact]
        public void Generates_Correct_String_OperatorMinus_ActualResult()
        {
            var randomGen = new MockRandomGenerator();
            var generator = new MathEquationGenerator(randomGen);

            randomGen.NextInts = new[] { 5, 6 };
            randomGen.NextDoubles = new[] { 0.3d };
            randomGen.NextBools = new[] { true };

            var equation = generator.GenerateEquation();

            Assert.Equal("5 - 6 = -1", equation.EquationString);
            Assert.Equal(-1d, equation.ShownResult, 1);
            Assert.Single(equation.ActualResults);
            Assert.Equal(-1d, equation.ActualResults.First(), 1);
        }

        [Fact]
        public void Generates_Correct_String_OperatorMultiply_ActualResult()
        {
            var randomGen = new MockRandomGenerator();
            var generator = new MathEquationGenerator(randomGen);

            randomGen.NextInts = new[] { 5, 6 };
            randomGen.NextDoubles = new[] { 0.6d };
            randomGen.NextBools = new[] { true };

            var equation = generator.GenerateEquation();

            Assert.Equal("5 * 6 = 30", equation.EquationString);
            Assert.Equal(30d, equation.ShownResult, 1);
            Assert.Single(equation.ActualResults);
            Assert.Equal(30d, equation.ActualResults.First(), 1);
        }

        [Fact]
        public void Generates_Correct_String_OperatorDivide_ActualResult()
        {
            var randomGen = new MockRandomGenerator();
            var generator = new MathEquationGenerator(randomGen);

            randomGen.NextInts = new[] { 12, 2 };
            randomGen.NextDoubles = new[] { 0.9d };
            randomGen.NextBools = new[] { true };

            var equation = generator.GenerateEquation();

            Assert.Equal("12 / 2 = 6", equation.EquationString);
            Assert.Equal(6d, equation.ShownResult, 1);
            Assert.Single(equation.ActualResults);
            Assert.Equal(6d, equation.ActualResults.First(), 1);

            randomGen.NextInts = new[] { 10, 4 };
            randomGen.NextDoubles = new[] { 0.9d };
            randomGen.NextBools = new[] { true };

            equation = generator.GenerateEquation();

            Assert.EndsWith("10 / 4 = 2.5", equation.EquationString);
            Assert.Equal(2.5d, equation.ShownResult, 1);
            Assert.Single(equation.ActualResults);
            Assert.Equal(2.5d, equation.ActualResults.First(), 1);
        }
    }
}
