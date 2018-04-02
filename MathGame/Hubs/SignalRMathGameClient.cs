using MathGame.Core.Interfaces;
using MathGame.Core.Models;
using MathGame.Hubs.Internal;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Reactive;
using System.Threading.Tasks;

namespace MathGame.Hubs
{
    public class SignalRMathGameClient : IDisposable
    {
        private readonly ConcurrentDictionary<string, IDisposable> _subscribers = new ConcurrentDictionary<string, IDisposable>();

        private readonly IHubContext<MathGameHub, ISignalRMathGameClient> _hubContext;
        private readonly IMathGameProcessor _mathGameProcessor;

        public SignalRMathGameClient(IHubContext<MathGameHub, ISignalRMathGameClient> hubContext, IMathGameProcessor mathGameProcessor)
        {
            _hubContext = hubContext;
            _mathGameProcessor = mathGameProcessor;
        }

        public Task UpdateInfo(PlayerInfo info, string key)
        {
            info.Id = info.Id ?? key;

            return _mathGameProcessor.PlayerConnected(info);
        }

        public Task Answer(RoundAnswer answer, string key)
        {
            return _mathGameProcessor.Answer(key, answer);
        }

        public Task PlayerConnected(string connectionId)
        {
            var observer = Observer.Create<MathGameMessage>(x =>
            {
                var messageWrapper = new MathGameMessageWrapper()
                {
                    MessageType = x.GetType().Name,
                    Message = x,
                };

                _hubContext.Clients.Client(connectionId).SendMessage(messageWrapper);
            });

            var subscription = _mathGameProcessor
                .GetMessageObservable()
                .Subscribe(observer);

            _subscribers.TryAdd(connectionId, subscription);

            return Task.CompletedTask;
        }

        public Task PlayerDisconnected(string key)
        {
            if (_subscribers.TryRemove(key, out var disposable))
            {
                disposable.Dispose();
            }

            return _mathGameProcessor.PlayerDisconnected(key);
        }

        public void Dispose()
        {
            foreach (var item in _subscribers)
            {
                item.Value.Dispose();
            }
        }
    }
}