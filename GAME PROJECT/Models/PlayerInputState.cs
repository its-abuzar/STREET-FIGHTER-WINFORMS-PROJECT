namespace Game_Project.Models
{
    /// <summary>
    /// Holds the raw pressed-state of every key for one player.
    /// Pure data: no logic, no WinForms references.
    /// </summary>
    public class PlayerInputState
    {
        public bool Left   { get; set; }
        public bool Right  { get; set; }
        public bool Up     { get; set; }
        public bool Down   { get; set; }

        public bool LightPunch { get; set; }
        public bool HeavyPunch { get; set; }
        public bool LightKick  { get; set; }
        public bool HeavyKick  { get; set; }
        public bool Special    { get; set; }

        public bool AttackLocked    { get; set; }
        public bool MovementBlocked { get; set; }  // P2 only: locked during attack animation

        public bool IsMoving => Left || Right;

        public void Reset()
        {
            Left = Right = Up = Down = false;
            LightPunch = HeavyPunch = LightKick = HeavyKick = Special = false;
        }
    }
}
