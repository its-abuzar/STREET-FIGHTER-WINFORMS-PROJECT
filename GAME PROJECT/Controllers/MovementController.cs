using System.Drawing;

namespace Game_Project.Controllers
{
    /// <summary>
    /// Single Responsibility: moves characters each tick, enforces boundary clamping,
    /// and triggers the correct walk animation.
    /// All numeric constants (MOVE_SPEED, MAX_OVERLAP_PERCENT) are identical to original.
    /// </summary>
    public class MovementController
    {
        private const int   MOVE_SPEED         = 8;
        private const float MAX_OVERLAP_PERCENT = 0.25f;

        private readonly BaseCharacter _player;
        private readonly BaseCharacter _opponent;

        public int CanvasWidth  { get; set; }

        public MovementController(BaseCharacter player, BaseCharacter opponent)
        {
            _player   = player;
            _opponent = opponent;
        }

        /// <summary>Move player left/right based on key flags.</summary>
        public void MovePlayer(bool leftPressed, bool rightPressed)
        {
            bool moved = false;
            int newX   = _player.Position.X;

            if (leftPressed)  { newX -= MOVE_SPEED; moved = true; }
            if (rightPressed) { newX += MOVE_SPEED; moved = true; }

            int maxX = GetMaxPlayerX();
            newX = Clamp(newX, 0, maxX);

            if (newX != _player.Position.X)
            {
                _player.Position = new Point(newX, _player.Position.Y);
            }

            if (moved)
            {
                if (leftPressed)  _player.MoveBackward();
                else if (rightPressed) _player.MoveForward();
            }
            else if (!leftPressed && !rightPressed)
            {
                _player.ReturnToIdle();
            }
        }

        /// <summary>Move opponent (VS mode – P2 controlled).</summary>
        public void MoveOpponentVs(bool p2Left, bool p2Right)
        {
            bool moved = false;
            int newX   = _opponent.Position.X;

            if (p2Left)  { newX -= MOVE_SPEED; moved = true; }
            if (p2Right) { newX += MOVE_SPEED; moved = true; }

            int minX = GetMinOpponentX();
            int maxX = CanvasWidth - (_opponent.CurrentFrame?.Width ?? 100);
            newX = Clamp(newX, minX, maxX);

            if (newX != _opponent.Position.X)
            {
                _opponent.Position = new Point(newX, _opponent.Position.Y);
            }
            else
            {
                moved = false; // collision wall
            }

            if (moved)
            {
                if (p2Left)  _opponent.MoveForward();   // left movement → play right-walk anim
                else if (p2Right) _opponent.MoveBackward();
            }
            else if (!p2Left && !p2Right)
            {
                _opponent.ReturnToIdle();
            }
        }

        /// <summary>Move AI opponent based on direction (-1, 0, 1).</summary>
        public void MoveOpponentAi(int aiDirection)
        {
            bool moved = false;
            int newX   = _opponent.Position.X;

            if (aiDirection == -1) { newX -= MOVE_SPEED; moved = true; }
            if (aiDirection ==  1) { newX += MOVE_SPEED; moved = true; }

            int minX = GetMinOpponentX();
            int maxX = CanvasWidth - (_opponent.CurrentFrame?.Width ?? 100);
            newX = Clamp(newX, minX, maxX);

            if (newX != _opponent.Position.X)
                _opponent.Position = new Point(newX, _opponent.Position.Y);
            else
                moved = false;

            if (moved)
            {
                if (aiDirection == -1) _opponent.MoveForward();
                else if (aiDirection == 1) _opponent.MoveBackward();
            }
            else if (aiDirection == 0)
            {
                _opponent.ReturnToIdle();
            }
        }

        // ── Boundary helpers (identical math to original) ──────────────────

        private int GetMaxPlayerX()
        {
            if (_player.CurrentFrame == null) return CanvasWidth;
            int playerWidth  = _player.CurrentFrame.Width;
            int opponentLeft = _opponent.Position.X;
            int maxOverlap   = (int)(playerWidth * MAX_OVERLAP_PERCENT);
            return opponentLeft - (playerWidth - maxOverlap);
        }

        private int GetMinOpponentX()
        {
            if (_opponent.CurrentFrame == null) return 0;
            int opponentWidth = _opponent.CurrentFrame.Width;
            int playerRight   = _player.Position.X + (_player.CurrentFrame?.Width ?? 0);
            int maxOverlap    = (int)(opponentWidth * MAX_OVERLAP_PERCENT);
            return playerRight - (opponentWidth - maxOverlap);
        }

        private static int Clamp(int value, int min, int max) =>
            value < min ? min : (value > max ? max : value);
    }
}
