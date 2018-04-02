using MathGame.Core.Models;
using System;
using System.Threading.Tasks;

namespace MathGame.Core.Interfaces
{
    public interface IMathGameProcessor
    {
        Task Answer(string playerKey, RoundAnswer answer);
        Task PlayerConnected(PlayerInfo player);
        Task PlayerDisconnected(string playerKey);

        IObservable<MathGameMessage> GetMessageObservable();
    }
}