using MathGame.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MathGame.Services
{
    public class PseudoRandomGenerator : IRandomGenerator
    {
        private readonly Random _randomNumberGenerator = new Random();
        private readonly object _lock = new object();

        public bool Bool()
        {
            lock (_lock)
            {
                return _randomNumberGenerator.NextDouble() < 0.5d;
            }
        }

        public double Double()
        {
            lock (_lock)
            {
                return _randomNumberGenerator.NextDouble();
            }
        }

        public int Int()
        {
            lock (_lock)
            {
                return _randomNumberGenerator.Next();
            }
        }

        public int Int(int from, int to)
        {
            lock (_lock)
            {
                return _randomNumberGenerator.Next(from, to);
            }
        }
    }
}
