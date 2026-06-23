using System;

namespace Game_Project.Controllers
{
    /// <summary>
    /// Single Responsibility: encapsulates the entire AI state machine.
    /// All values (distances, probabilities, timers) are identical to the original.
    /// Injected CombatController is used to trigger attacks (Dependency Injection).
    /// </summary>
    public class AIController
    {
        // ── State machine ──────────────────────────────────────────────────
        private int _aiState;
        private int _aiStateTimer;
        private int _aiAttackCooldown;
        private int _aiRetreatBurst;
        private int _aiDecisionCounter;

        /// <summary>
        /// Current movement direction: -1 = move left (forward), 0 = stop, 1 = move right (backward).
        /// </summary>
        public int AiDirection { get; private set; }

        private readonly Random           _rand;
        private readonly CombatController _combat;
        private readonly BaseCharacter    _opponent;  // the AI character

        public AIController(Random rand, CombatController combat, BaseCharacter opponent)
        {
            _rand     = rand;
            _combat   = combat;
            _opponent = opponent;
        }

        /// <summary>
        /// Called once per game-loop tick. Identical logic to original FightScreen AI block.
        /// </summary>
        public void Update(int playerX, int opponentX, bool opponentHitStunned)
        {
            if (opponentHitStunned) return;

            int diff = playerX - opponentX;
            int dist = Math.Abs(diff);

            if (_aiAttackCooldown > 0) _aiAttackCooldown--;
            if (_aiRetreatBurst   > 0) _aiRetreatBurst--;
            _aiStateTimer--;

            // ── State transition ───────────────────────────────────────────
            if (_aiStateTimer <= 0 || (_aiState == 1 && dist > 350))
            {
                int roll = _rand.Next(100);
                if (dist < 80)
                {
                    if      (roll < 50) { _aiState = 1; _aiStateTimer = _rand.Next(20, 50); }
                    else if (roll < 80) { _aiState = 2; _aiStateTimer = _rand.Next(15, 35); }
                    else                { _aiState = 3; _aiStateTimer = _rand.Next(10, 25); }
                }
                else if (dist < 200)
                {
                    if      (roll < 40) { _aiState = 0; _aiStateTimer = _rand.Next(15, 40); }
                    else if (roll < 60) { _aiState = 2; _aiStateTimer = _rand.Next(20, 45); }
                    else if (roll < 75) { _aiState = 1; _aiStateTimer = _rand.Next(10, 25); }
                    else                { _aiState = 3; _aiStateTimer = _rand.Next(15, 30); }
                }
                else
                {
                    if      (roll < 70) { _aiState = 0; _aiStateTimer = _rand.Next(25, 60); }
                    else if (roll < 85) { _aiState = 2; _aiStateTimer = _rand.Next(15, 35); }
                    else                { _aiState = 3; _aiStateTimer = _rand.Next(10, 20); }
                }
            }

            if (_aiRetreatBurst > 0) { _aiState = 1; _aiStateTimer = Math.Max(_aiStateTimer, _aiRetreatBurst); }

            // ── Direction from state ───────────────────────────────────────
            switch (_aiState)
            {
                case 0:
                    AiDirection = (diff > 0) ? 1 : -1;
                    if (dist < 60) AiDirection = 0;
                    break;
                case 1:
                    AiDirection = (diff > 0) ? -1 : 1;
                    if (dist > 400) AiDirection = 0;
                    break;
                case 2:
                    AiDirection = (_aiStateTimer % 12 < 6)
                        ? ((diff > 0) ? -1 : 1)
                        : ((diff > 0) ?  1 : -1);
                    break;
                case 3:
                    AiDirection = 0;
                    if (_aiStateTimer % 30 < 5 && dist > 120)
                        AiDirection = (diff > 0) ? 1 : -1;
                    break;
            }

            // ── Attack decision ────────────────────────────────────────────
            _aiDecisionCounter++;
            if (_aiDecisionCounter >= 8)
            {
                _aiDecisionCounter = 0;
                if (dist < 150 && _aiAttackCooldown <= 0)
                {
                    if (_rand.Next(100) < 35)
                    {
                        // Regular attack
                        int atk = _rand.Next(6);
                        Action attackAction;
                        switch (atk)
                        {
                            case 0: attackAction = () => _opponent.PunchLeft();     break;
                            case 1: attackAction = () => _opponent.PunchRight();    break;
                            case 2: attackAction = () => _opponent.KickHighLeft();  break;
                            case 3: attackAction = () => _opponent.KickHighRight(); break;
                            case 4: attackAction = () => _opponent.UppercutLeft();  break;
                            default: attackAction = () => _opponent.UppercutRight(); break;
                        }
                        AiDirection = 0;
                        _combat.AiPerformAttack(attackAction, 7, dist);
                        _aiAttackCooldown = _rand.Next(18, 40);
                    }
                    else if (_rand.Next(100) < 15 && IsSpecialCapable())
                    {
                        // Special attack
                        _opponent.DoSpecial();
                        bool fullScreen = (_opponent is Blanka);
                        _combat.AiPerformSpecial(fullScreen, dist);
                        _aiAttackCooldown = _rand.Next(25, 55);
                    }
                }
            }
        }

        private bool IsSpecialCapable()
        {
            return _opponent is Ryu    ||
                   _opponent is Ken    ||
                   _opponent is Blanka ||
                   _opponent is ChunLi ||
                   _opponent is E_Honda||
                   _opponent is Guile  ||
                   _opponent is Zangief||
                   _opponent is Dhalsim;
        }
    }
}
