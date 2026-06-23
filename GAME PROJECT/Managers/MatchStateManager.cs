using System;
using System.Windows.Forms;
using Game_Project.Interfaces;
using Game_Project.Models;

namespace Game_Project.Managers
{
    /// <summary>
    /// Single Responsibility: manages health, countdown timer, and match-over detection.
    /// Notifies registered IMatchObserver instances when the match ends (Observer pattern).
    /// Does not touch UI directly – it mutates MatchState and raises events.
    /// </summary>
    public class MatchStateManager : IDisposable
    {
        private readonly MatchState _state;
        private readonly Timer      _matchTimer;
        private IMatchObserver      _observer;

        private string _playerName;
        private string _opponentName;

        public MatchState State => _state;

        public MatchStateManager(MatchState state)
        {
            _state      = state;
            _matchTimer = new Timer { Interval = 1000 };
            _matchTimer.Tick += OnTimerTick;
        }

        public void Initialise(string playerName, string opponentName, IMatchObserver observer)
        {
            _playerName   = playerName;
            _opponentName = opponentName;
            _observer     = observer;
        }

        public void StartTimer() => _matchTimer.Start();
        public void StopTimer()  => _matchTimer.Stop();

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (_state.IsOver) return;
            _state.TimeLeft--;
            if (_state.TimeLeft <= 0)
            {
                _matchTimer.Stop();
                CheckTimeOutWinner();
            }
        }

        private void CheckTimeOutWinner()
        {
            if (_state.PlayerHealth > _state.OpponentHealth)
                TriggerGameOver(_playerName, false);
            else if (_state.OpponentHealth > _state.PlayerHealth)
                TriggerGameOver(_opponentName, false);
            else
                TriggerGameOver(null, true);
        }

        /// <summary>Applies damage to the opponent and checks for KO.</summary>
        public void DamageOpponent(int amount)
        {
            if (_state.IsOver) return;
            _state.OpponentHealth = Math.Max(0, _state.OpponentHealth - amount);
            if (_state.OpponentHealth <= 0)
                TriggerGameOver(_playerName, false);
        }

        /// <summary>Applies damage to the player and checks for KO.</summary>
        public void DamagePlayer(int amount)
        {
            if (_state.IsOver) return;
            _state.PlayerHealth = Math.Max(0, _state.PlayerHealth - amount);
            if (_state.PlayerHealth <= 0)
                TriggerGameOver(_opponentName, false);
        }

        private void TriggerGameOver(string winnerName, bool isDraw)
        {
            if (_state.IsOver) return;
            _state.IsOver = true;
            _matchTimer.Stop();
            _observer?.OnMatchOver(winnerName, isDraw);
        }

        public void Dispose()
        {
            _matchTimer.Stop();
            _matchTimer.Dispose();
        }
    }
}
