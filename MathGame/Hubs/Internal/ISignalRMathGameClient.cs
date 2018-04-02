using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathGame.Hubs.Internal
{
    public interface ISignalRMathGameClient
    {
        void SendMessage(MathGameMessageWrapper message);
        void SetId(string id);
    }
}
