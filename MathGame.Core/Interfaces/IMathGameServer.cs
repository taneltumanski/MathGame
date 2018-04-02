using MathGame.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MathGame.Core.Interfaces
{
    public interface IMathGameServer
    {
        Task UpdateInfo(PlayerInfo info);
        Task Answer(RoundAnswer answer);
    }
}
