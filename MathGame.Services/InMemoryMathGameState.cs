using MathGame.Core.Interfaces;
using MathGame.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathGame.Services
{
    public class InMemoryMathGameState : IMathGameState
    {
        private ImmutableArray<GameRoundHistory> _roundHistoryArray = ImmutableArray<GameRoundHistory>.Empty;

        private readonly ConcurrentDictionary<string, GamePlayer> _players = new ConcurrentDictionary<string, GamePlayer>();
        private readonly object _lock = new object();

        private readonly IDateTimeProvider _dateTimeProvider;

        public GameState CurrentState { get; private set; }
        public GameRound CurrentRound { get; private set; }

        public IEnumerable<GamePlayer> Players => _players.Values.ToImmutableArray();
        public IEnumerable<GameRoundHistory> RoundHistory => _roundHistoryArray;

        public InMemoryMathGameState(IDateTimeProvider dateTimeProvider)
        {
            this._dateTimeProvider = dateTimeProvider;

            this.CurrentState = GameState.Waiting;
        }

        public Task<StartRoundResult> StartNewRound(MathEquation equation)
        {
            lock(_lock)
            {
                if (CurrentState == GameState.RoundInPlay)
                {
                    throw new InvalidOperationException("Cannot start new round when previous round is in play");
                }

                var gamePlayers = Players.ToArray();

                if (!gamePlayers.Any())
                {
                    var gameState = UpdateRoundStatus();

                    return Task.FromResult(new StartRoundResult(gameState.State, CurrentRound));
                }

                CurrentRound = new GameRound(Guid.NewGuid(), _dateTimeProvider.UtcNow, equation, gamePlayers);
                CurrentState = GameState.RoundInPlay;

                var state = UpdateRoundStatus();

                return Task.FromResult(new StartRoundResult(state.State, CurrentRound));
            }
        }

        public async Task<CheckAnswerResult> CheckAnswer(RoundAnswer answer, string id)
        {
            if (_players.TryGetValue(id, out var player))
            {
                return await CheckAnswer(answer, player);
            }

            var status = UpdateRoundStatus();

            return new CheckAnswerResult(status.State);
        }

        public Task<CheckAnswerResult> CheckAnswer(RoundAnswer answer, GamePlayer player)
        {
            lock (_lock)
            {
                if (CurrentState != GameState.RoundInPlay)
                {
                    return Task.FromResult(new CheckAnswerResult(GetRoundState(CurrentState)));
                }

                var isAnswerCorrect = CurrentRound.CheckAnswer(answer, player);

                if (isAnswerCorrect)
                {
                    player.Score++;

                    var roundHistoryItem = new GameRoundHistory()
                    {
                        Id = CurrentRound.Id,
                        StartTime = CurrentRound.StartTime,
                        EndTime = _dateTimeProvider.UtcNow,
                        Players = CurrentRound.Players,
                        WinningPlayer = player
                    };

                    _roundHistoryArray = _roundHistoryArray.Add(roundHistoryItem);

                    var winningRoundState = ResetRound();

                    return Task.FromResult(new CheckAnswerResult(winningRoundState.State, player));
                }

                var roundState = UpdateRoundStatus();

                return Task.FromResult(new CheckAnswerResult(roundState.State));
            }
        }

        public Task<PlayerUpdateResult> AddOrUpdatePlayer(GamePlayer player)
        {
            lock (_lock)
            {
                var updatePlayer = player;

                if (!_players.TryAdd(player.Id, player))
                {
                    updatePlayer = _players[player.Id];

                    updatePlayer.Name = player.Name;
                }

                var state = UpdateRoundStatus();

                return Task.FromResult(new PlayerUpdateResult(state.State, updatePlayer));
            }
        }

        public Task<PlayerRemoveResult> RemovePlayer(string id)
        {
            lock (_lock)
            {
                GamePlayer player = null;

                _players.TryRemove(id, out player);

                var state = UpdateRoundStatus();

                return Task.FromResult(new PlayerRemoveResult(state.State, player));
            }
        }

        public Task<PlayerRemoveResult> RemovePlayer(GamePlayer player)
        {
            lock (_lock)
            {
                return RemovePlayer(player.Id);
            }
        }

        private BaseResult UpdateRoundStatus()
        {
            lock (_lock)
            {
                if (CurrentState == GameState.Invalid)
                {
                    throw new InvalidOperationException("Game state cannot be invalid");
                }

                if (CurrentState == GameState.Waiting)
                {
                    if (Players.Any())
                    {
                        CurrentState = GameState.CanStart;
                    }
                }
                else if (CurrentState == GameState.CanStart)
                {
                    if (!Players.Any())
                    {
                        CurrentState = GameState.Waiting;
                    }
                }
                else if (CurrentState == GameState.RoundInPlay)
                {
                    var roundPlayers = CurrentRound.Players.ToDictionary(x => x.Id);
                    var connectedPlayers = Players.ToDictionary(x => x.Id);
                    var connectedRoundPlayers = roundPlayers
                        .Where(x => connectedPlayers.ContainsKey(x.Key))
                        .ToArray();

                    var roundHasConnectedPlayers = connectedRoundPlayers.Any();

                    if (!roundHasConnectedPlayers)
                    {
                        return ResetRound();
                    }

                    var answeredPlayers = CurrentRound.AnsweredPlayers.ToDictionary(x => x.Id);
                    var allPlayersAnswered = connectedRoundPlayers.All(x => answeredPlayers.ContainsKey(x.Key));

                    if (allPlayersAnswered)
                    {
                        return ResetRound();
                    }
                }

                var roundState = GetRoundState(CurrentState);

                return new BaseResult(roundState);
            }
        }

        private RoundState GetRoundState(GameState currentState)
        {
            switch (currentState)
            {
                case GameState.Waiting: return RoundState.Waiting;
                case GameState.CanStart: return RoundState.CanStart;
                case GameState.RoundInPlay: return RoundState.InPlay;
            }

            throw new InvalidOperationException($"State not handled: {CurrentState}");
        }

        private BaseResult ResetRound()
        {
            CurrentRound = null;
            CurrentState = GameState.Waiting;

            return UpdateRoundStatus();
        }
    }
}
