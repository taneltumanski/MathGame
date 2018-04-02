using MathGame.Core.Interfaces;
using MathGame.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace MathGame.Services
{
    public class MathGameProcessor : ObservableBase<MathGameMessage>, IMathGameProcessor
    {
        private readonly ConcurrentDictionary<Guid, IObserver<MathGameMessage>> _subscribers = new ConcurrentDictionary<Guid, IObserver<MathGameMessage>>();

        private readonly IMathGameState _mathGameState;
        private readonly IMathEquationGenerator _equationGenerator;

        public MathGameProcessor(IMathGameState mathGameState, IMathEquationGenerator equationGenerator)
        {
            _mathGameState = mathGameState;
            _equationGenerator = equationGenerator;
        }

        public async Task Answer(string playerKey, RoundAnswer answer)
        {
            var result = await _mathGameState.CheckAnswer(answer, playerKey);

            if (result.IsCorrectAnswer)
            {
                await SendMessage(new RoundEndMessage()
                {
                    WinningPlayerId = playerKey
                });

                await SendMessage(new PlayerInfoUpdateMessage()
                {
                    Id = result.WinningPlayer.Id,
                    Name = result.WinningPlayer.Name,
                    Score = result.WinningPlayer.Score,
                });
            }

            await SendMessage(new DisableRoundMessage()
            {
                DisableForPlayerId = playerKey
            });

            await TryStartRound(result);
        }

        public async Task PlayerConnected(PlayerInfo player)
        {
            var result = await _mathGameState.AddOrUpdatePlayer(new GamePlayer(player.Id, player.Name));

            await SendMessage(new PlayerInfoUpdateMessage()
            {
                Id = result.Player.Id,
                Name = result.Player.Name,
                Score = result.Player.Score,
            });

            await TryStartRound(result);
        }

        public async Task PlayerDisconnected(string playerKey)
        {
            var result = await _mathGameState.RemovePlayer(playerKey);

            await SendMessage(new PlayerRemoveMessage()
            {
                Id = playerKey,
            });
        }

        private async Task StartRound()
        {
            var startRoundResult = await _mathGameState.StartNewRound(_equationGenerator.GenerateEquation());

            if (startRoundResult.State == RoundState.InPlay)
            {
                await SendMessage(new RoundStartMessage()
                {
                    Equation = startRoundResult.Round.Equation.EquationString
                });
            }
        }

        private async Task TryStartRound(BaseResult result)
        {
            if (result.State == RoundState.CanStart)
            {
                await StartRound();
            }
        }

        public IObservable<MathGameMessage> GetMessageObservable()
        {
            return this;
        }

        protected override IDisposable SubscribeCore(IObserver<MathGameMessage> observer)
        {
            var id = Guid.NewGuid();

            _subscribers.TryAdd(id, observer);

            observer.OnNext(new FullUpdateMessage()
            {
                PlayerInfos = _mathGameState
                    .Players
                    .Select(x => new PlayerInfoUpdateMessage()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Score = x.Score
                    })
                    .ToImmutableArray()
            });

            return Disposable.Create(() =>
            {
                _subscribers.TryRemove(id, out var _);
            });
        }

        private Task SendMessage(MathGameMessage msg)
        {
            return SendMessage(new[] { msg });
        }

        private Task SendMessage(IEnumerable<MathGameMessage> msgs)
        {
            return Task.Run(() =>
            {
                foreach (var msg in msgs)
                {
                    foreach (var subscriber in _subscribers.Values)
                    {
                        subscriber.OnNext(msg);
                    }
                }
            });
        }
    }
}
