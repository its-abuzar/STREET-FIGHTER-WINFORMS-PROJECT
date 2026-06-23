using System.Drawing;

namespace Game_Project
{
    public class Ryu : BaseCharacter
    {
        private const string ANIM_FIREBALL_THROW = "FireballThrow";

        public Ryu(Point startPosition, bool mirrored = false)
            : base("Ryu", startPosition, mirrored, Color.Black)
        {
            // Standing width: 100px
            SetHitDistance(ANIM_LEFT_PUNCH, 30);
            SetHitDistance(ANIM_RIGHT_PUNCH, 30);
            SetHitDistance(ANIM_UPPERCUT_LEFT, 30);
            SetHitDistance(ANIM_UPPERCUT_RIGHT, 20);
            SetHitDistance(ANIM_HIGH_KICK_LEFT, 80);
            SetHitDistance(ANIM_HIGH_KICK_RIGHT, 40);
            SetHitDistance(ANIM_CROUCH_LEFT_KICK, 60);
            SetHitDistance(ANIM_CROUCH_RIGHT_KICK, 60);

            RegisterAnimation(ANIM_FIREBALL_THROW, "Ryu-fireballs.gif", 100);
        }

        public override void DoSpecial()
        {
            ChangeAnimation(ANIM_FIREBALL_THROW);
        }
    }
}