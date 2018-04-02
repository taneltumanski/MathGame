using MathGame.Core.Interfaces;

namespace MathGame.Test
{
    internal class MockRandomGenerator : IRandomGenerator
    {
        public bool[] NextBools { get; set; }
        public double[] NextDoubles { get; set; }
        public int[] NextInts { get; set; }

        private int _boolIndex;
        private int _doubleIndex;
        private int _intIndex;

        public bool Bool()
        {
            var i = _boolIndex++;
            _boolIndex = _boolIndex % NextBools.Length;

            return NextBools[i];
        }

        public double Double()
        {
            var i = _doubleIndex++;
            _doubleIndex = _doubleIndex % NextDoubles.Length;

            return NextDoubles[i];
        }

        public int Int()
        {
            var i = _intIndex++;
            _intIndex = _intIndex % NextInts.Length;

            return NextInts[i];
        }

        public int Int(int from, int to)
        {
            var i = _intIndex++;
            _intIndex = _intIndex % NextInts.Length;

            return NextInts[i];
        }
    }
}