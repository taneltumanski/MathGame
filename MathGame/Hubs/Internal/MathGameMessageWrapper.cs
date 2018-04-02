using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathGame.Core.Models;

namespace MathGame.Hubs.Internal
{
    public class MathGameMessageWrapper
    {
        public string MessageType { get; set; }
        public MathGameMessage Message { get; set; }
    }
}
