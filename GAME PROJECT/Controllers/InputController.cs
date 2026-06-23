using System.Windows.Forms;
using Game_Project.Interfaces;
using Game_Project.Models;

namespace Game_Project.Controllers
{
    /// <summary>
    /// Single Responsibility: maps raw keyboard events to structured PlayerInputState objects.
    /// Knows nothing about characters, health, rendering, or game logic.
    /// Injected into FightScreen via constructor (Dependency Injection).
    /// </summary>
    public class InputController : IInputHandler
    {
        private readonly KeyBindings _bindings;
        private readonly PlayerInputState _p1;
        private readonly PlayerInputState _p2;
        private readonly bool _vsMode;

        public PlayerInputState P1 => _p1;
        public PlayerInputState P2 => _p2;

        public InputController(KeyBindings bindings, bool vsMode)
        {
            _bindings = bindings;
            _vsMode   = vsMode;
            _p1       = new PlayerInputState();
            _p2       = new PlayerInputState();
        }

        /// <summary>
        /// Called from ProcessCmdKey. Returns true if the key was consumed.
        /// </summary>
        public bool HandleKeyDown(Keys keyData)
        {
            // Player 1 movement
            if (keyData == _bindings.P1_Left)  { _p1.Left  = true; return true; }
            if (keyData == _bindings.P1_Right) { _p1.Right = true; return true; }
            if (keyData == _bindings.P1_Up)    { _p1.Up    = true; return true; }
            if (keyData == _bindings.P1_Down)  { _p1.Down  = true; return true; }

            // Player 1 attacks (only register if not already attack-locked)
            if (!_p1.AttackLocked)
            {
                if (keyData == _bindings.P1_LightPunch && !_p1.LightPunch) { _p1.LightPunch = true; return true; }
                if (keyData == _bindings.P1_HeavyPunch && !_p1.HeavyPunch) { _p1.HeavyPunch = true; return true; }
                if (keyData == _bindings.P1_LightKick  && !_p1.LightKick)  { _p1.LightKick  = true; return true; }
                if (keyData == _bindings.P1_HeavyKick  && !_p1.HeavyKick)  { _p1.HeavyKick  = true; return true; }
                if (keyData == _bindings.P1_Special    && !_p1.Special)    { _p1.Special    = true; return true; }
            }

            if (!_vsMode) return false;

            // Player 2 movement
            if (keyData == _bindings.P2_Left)  { _p2.Left  = true; return true; }
            if (keyData == _bindings.P2_Right) { _p2.Right = true; return true; }
            if (keyData == _bindings.P2_Up)    { _p2.Up    = true; return true; }
            if (keyData == _bindings.P2_Down)  { _p2.Down  = true; return true; }

            // Player 2 attacks
            if (!_p2.AttackLocked)
            {
                if (keyData == _bindings.P2_LightPunch && !_p2.LightPunch) { _p2.LightPunch = true; return true; }
                if (keyData == _bindings.P2_HeavyPunch && !_p2.HeavyPunch) { _p2.HeavyPunch = true; return true; }
                if (keyData == _bindings.P2_LightKick  && !_p2.LightKick)  { _p2.LightKick  = true; return true; }
                if (keyData == _bindings.P2_HeavyKick  && !_p2.HeavyKick)  { _p2.HeavyKick  = true; return true; }
                if (keyData == _bindings.P2_Special    && !_p2.Special)    { _p2.Special    = true; return true; }
            }

            return false;
        }

        /// <summary>
        /// Called from OnKeyUp. Always processes both players.
        /// </summary>
        public void HandleKeyUp(Keys keyCode)
        {
            // Player 1
            if (keyCode == _bindings.P1_Left)       _p1.Left       = false;
            if (keyCode == _bindings.P1_Right)      _p1.Right      = false;
            if (keyCode == _bindings.P1_Up)         _p1.Up         = false;
            if (keyCode == _bindings.P1_Down)       _p1.Down       = false;
            if (keyCode == _bindings.P1_LightPunch) _p1.LightPunch = false;
            if (keyCode == _bindings.P1_HeavyPunch) _p1.HeavyPunch = false;
            if (keyCode == _bindings.P1_LightKick)  _p1.LightKick  = false;
            if (keyCode == _bindings.P1_HeavyKick)  _p1.HeavyKick  = false;
            if (keyCode == _bindings.P1_Special)    _p1.Special    = false;

            if (!_vsMode) return;

            // Player 2
            if (keyCode == _bindings.P2_Left)       _p2.Left       = false;
            if (keyCode == _bindings.P2_Right)      _p2.Right      = false;
            if (keyCode == _bindings.P2_Up)         _p2.Up         = false;
            if (keyCode == _bindings.P2_Down)       _p2.Down       = false;
            if (keyCode == _bindings.P2_LightPunch) _p2.LightPunch = false;
            if (keyCode == _bindings.P2_HeavyPunch) _p2.HeavyPunch = false;
            if (keyCode == _bindings.P2_LightKick)  _p2.LightKick  = false;
            if (keyCode == _bindings.P2_HeavyKick)  _p2.HeavyKick  = false;
            if (keyCode == _bindings.P2_Special)    _p2.Special    = false;
        }
    }
}
