using System;
using System.Collections.Generic;
using System.Text;

namespace MathGame.Core.Interfaces
{
    public interface IRandomGenerator
    {
        double Double();
        int Int();
        int Int(int from, int to);
        bool Bool();
    }
}
