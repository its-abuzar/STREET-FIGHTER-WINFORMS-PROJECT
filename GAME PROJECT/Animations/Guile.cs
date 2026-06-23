using System.Drawing;

namespace Game_Project
{
    public class Guile : BaseCharacter
    {
        private const string ANIM_SONIC_BOOM = "SonicBoom";

        public Guile(Point startPosition, bool mirrored = false)
            : base("Guile", startPosition, mirrored, Color.Magenta)
        {
            SetHitDistance(ANIM_LEFT_PUNCH, 60);
            SetHitDistance(ANIM_RIGHT_PUNCH, 60);
            SetHitDistance(ANIM_UPPERCUT_LEFT, 30);
            SetHitDistance(ANIM_UPPERCUT_RIGHT, 30);
            SetHitDistance(ANIM_HIGH_KICK_LEFT, 70);
            SetHitDistance(ANIM_HIGH_KICK_RIGHT, 70);
            SetHitDistance(ANIM_CROUCH_LEFT_KICK, 75);
            SetHitDistance(ANIM_CROUCH_RIGHT_KICK, 80);
            SetHitDistance(ANIM_SONIC_BOOM, 150);

            RegisterAnimation(ANIM_SONIC_BOOM, "Guile-sonicboom.gif", 200);
            RegisterAnimation(ANIM_RIGHT_PUNCH, "Guile-RightPunch.gif", 200);
            RegisterAnimation(ANIM_LEFT_PUNCH, "Guile-LeftPunch.gif", 220);
            RegisterAnimation(ANIM_HIGH_KICK_RIGHT, "Guile-High_Kick-Right.gif", 230);
            RegisterAnimation(ANIM_UPPERCUT_RIGHT, "Guile-UpperCut-Right.gif", 70);
            RegisterAnimation(ANIM_UPPERCUT_LEFT, "Guile-UpperCut-Left.gif", 80);
            RegisterAnimation(ANIM_CROUCH_LEFT_KICK, "Guile-crouch-left.gif", 220);
        }

        public override void DoSpecial()
        {
            ChangeAnimation(ANIM_SONIC_BOOM);
        }
    }
}