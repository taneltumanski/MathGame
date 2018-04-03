using MathGame.Core.Interfaces;
using MathGame.Core.Models;
using MathGame.Hubs.Internal;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathGame.Hubs
{
    public class MathGameHub : Hub<ISignalRMathGameClient>, IMathGameServer
    {
        private readonly SignalRMathGameClient _client;

        public MathGameHub(SignalRMathGameClient mathGameEngine)
        {
            _client = mathGameEngine;
        }

        public Task Answer(RoundAnswer answer)
        {
            return _client.Answer(answer, Context.ConnectionId);
        }

        public Task UpdateInfo(PlayerInfo info)
        {
            return _client.UpdateInfo(info, Context.ConnectionId);
        }

        public override Task OnConnectedAsync()
        {
            Clients.Caller.SetId(Context.ConnectionId);

            return _client.PlayerConnected(Context.ConnectionId);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return _client.PlayerDisconnected(Context.ConnectionId);
        }  
    }
}
