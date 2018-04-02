using System;
using System.Collections.Generic;
using System.Text;

namespace MathGame.Core.Models
{
    public class MathGameMessage
    {
        
    }

    public class RoundEndMessage : MathGameMessage
    {
        public string WinningPlayerId { get; set; }
    }

    public class DisableRoundMessage : MathGameMessage
    {
        public string DisableForPlayerId { get; set; }
    }

    public class RoundStartMessage : MathGameMessage
    {
        public string Equation { get; set; }
    }

    public class PlayerInfoUpdateMessage : MathGameMessage
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
    }

    public class PlayerRemoveMessage : MathGameMessage
    {
        public string Id { get; set; }
    }

    public class FullUpdateMessage : MathGameMessage
    {
        public IEnumerable<PlayerInfoUpdateMessage> PlayerInfos { get; set; }
    }
}
