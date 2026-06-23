namespace Game_Project.Models
{
    /// <summary>
    /// Pure data model holding the mutable state of an ongoing match.
    /// Single Responsibility: data container only – no behaviour, no UI, no timers.
    /// </summary>
    public class MatchState
    {
        public int PlayerHealth  { get; set; } = 100;
        public int OpponentHealth { get; set; } = 100;
        public int TimeLeft      { get; set; } = 60;
        public bool IsOver       { get; set; } = false;
        public bool ShowResult   { get; set; } = false;
        public string ResultText    { get; set; } = "";
        public string ResultSubtitle { get; set; } = "";
        public bool DebugMode    { get; set; } = false;
    }
}
