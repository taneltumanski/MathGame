using System;
using System.Collections.Generic;
using System.Text;

namespace MathGame.Core.Models
{
    public class PlayerInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class RoundAnswer
    {
        public bool IsEquationCorrect { get; set; }
    }
}
