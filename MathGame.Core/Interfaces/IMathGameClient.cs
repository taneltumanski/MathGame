using MathGame.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MathGame.Core.Interfaces
{
    public interface IMathGameClient
    {
        void RoundStart(RoundStartMessage startParameters);
        void RoundEnd(RoundEndMessage endParameters);
        void DisableRound(DisableRoundMessage disableRoundMessage);
        void PlayerInfoUpdate(PlayerInfoUpdateMessage playerInfoUpdate);
        void FullUpdate(FullUpdateMessage updateMessage, string key);
    }
}
