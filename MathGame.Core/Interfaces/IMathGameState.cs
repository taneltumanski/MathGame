using MathGame.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathGame.Core.Interfaces
{
    public interface IMathGameState
    {
        GameState CurrentState { get; }
        GameRound CurrentRound { get; }

        IEnumerable<GamePlayer> Players { get; }
        IEnumerable<GameRoundHistory> RoundHistory { get; }

        Task<PlayerUpdateResult> AddOrUpdatePlayer(GamePlayer player);
        Task<PlayerRemoveResult> RemovePlayer(string id);
        Task<PlayerRemoveResult> RemovePlayer(GamePlayer player);

        Task<StartRoundResult> StartNewRound(MathEquation equation);
        Task<CheckAnswerResult> CheckAnswer(RoundAnswer answer, string id);
        Task<CheckAnswerResult> CheckAnswer(RoundAnswer answer, GamePlayer player);
    }

    public class GameResult
    {
        public GameState GameState { get; }
        public GameRound Round { get; }
        public GamePlayer WinningPlayer { get; }

        public bool IsCorrectAnswer => WinningPlayer != null;

        public GameResult(GameState gameState, GameRound round)
        {
            this.GameState = gameState;
            this.Round = round;
        }

        public GameResult(GameState gameState, GameRound round, GamePlayer winningPlayer) : this(gameState, round)
        {
            this.WinningPlayer = winningPlayer ?? throw new ArgumentNullException(nameof(winningPlayer));
        }
    }

    public class BaseResult
    {
        public RoundState State { get; }

        public BaseResult(RoundState state)
        {
            this.State = state;
        } 
    }

    public class StartRoundResult : BaseResult
    {
        public GameRound Round { get; }

        public StartRoundResult(RoundState state, GameRound round) : base(state)
        {
            this.Round = round ?? throw new ArgumentNullException(nameof(round));
        } 
    }

    public class PlayerUpdateResult : BaseResult
    {
        public GamePlayer Player { get; }

        public PlayerUpdateResult(RoundState state, GamePlayer Player) : base(state)
        {
            this.Player = Player ?? throw new ArgumentNullException(nameof(Player));
        }
    }

    public class PlayerRemoveResult : BaseResult
    {
        public GamePlayer Player { get; }

        public PlayerRemoveResult(RoundState state, GamePlayer Player) : base(state)
        {
            this.Player = Player ?? throw new ArgumentNullException(nameof(Player));
        }
    }

    public class CheckAnswerResult : BaseResult
    {
        public GamePlayer WinningPlayer { get; }

        public bool IsCorrectAnswer => WinningPlayer != null;

        public CheckAnswerResult(RoundState state) : this(state, null) { }
        public CheckAnswerResult(RoundState state, GamePlayer winningPlayer) : base(state)
        {
            this.WinningPlayer = winningPlayer;
        }
    }

    public enum RoundState
    {
        Invalid = 0,
        Waiting = 1,
        CanStart = 2,
        InPlay = 3,
        Canceled = 4
    }

    public enum GameState
    {
        Invalid = 0,
        Waiting = 1,
        CanStart = 2,
        RoundInPlay = 3,
    }

    public class GameRound
    {
        public Guid Id { get; }
        public DateTimeOffset StartTime { get; }
        public MathEquation Equation { get; }

        public IEnumerable<GamePlayer> Players { get; }
        public IEnumerable<GamePlayer> AnsweredPlayers => _answeredPlayers;

        private readonly object _lock = new object();

        private ImmutableArray<GamePlayer> _answeredPlayers = ImmutableArray<GamePlayer>.Empty;

        public GameRound(Guid id, DateTimeOffset startTime, MathEquation equation, IEnumerable<GamePlayer> players)
        {
            this.Id = id;
            this.StartTime = startTime;
            this.Equation = equation ?? throw new ArgumentNullException(nameof(equation));
            this.Players = players?.ToImmutableArray() ?? throw new ArgumentNullException(nameof(players));
        }

        public bool CheckAnswer(RoundAnswer answer, GamePlayer player)
        {
            lock (_lock)
            {
                if (!Players.Any(x => x.Id == player.Id))
                {
                    return false;
                }

                if (_answeredPlayers.Any(x => x.Id == player.Id))
                {
                    return false;
                }

                _answeredPlayers = _answeredPlayers.Add(player);

                bool isAnswerCorrect;

                if (answer.IsEquationCorrect)
                {
                    isAnswerCorrect = Equation.CheckResult(Equation.ShownResult);
                }
                else
                {
                    isAnswerCorrect = !Equation.CheckResult(Equation.ShownResult);
                }

                return isAnswerCorrect;
            }
        }
    }

    public class GameRoundHistory
    {
        public Guid Id { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public GamePlayer WinningPlayer { get; set; }

        public IEnumerable<GamePlayer> Players { get; set; }
    }

    public class GamePlayer
    {
        public string Id { get; }
        public string Name { get; set; }
        public int Score { get; set; }

        public GamePlayer(string id, string name)
        {
            this.Id = id ?? throw new ArgumentNullException(nameof(id));
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }

    public class GameScore
    {
        public IEnumerable<GamePlayer> TopPlayers { get; set; }
    }
}
