namespace Game_Project.Interfaces
{
    /// <summary>
    /// Implemented by any component that needs to react to match-end events.
    /// Decouples the MatchStateManager from the UI layer (Dependency Inversion).
    /// </summary>
    public interface IMatchObserver
    {
        void OnMatchOver(string winnerName, bool isDraw);
    }
}
