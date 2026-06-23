using System.Drawing;

namespace Game_Project
{
    public class Dhalsim : BaseCharacter
    {
        private const string ANIM_YOGA_FIRE = "YogaFire";

        public Dhalsim(Point startPosition, bool mirrored = false)
            : base("Dhalsim", startPosition, mirrored, Color.Magenta)
        {
            SetHitDistance(ANIM_LEFT_PUNCH, 20);
            SetHitDistance(ANIM_RIGHT_PUNCH, 20);
            SetHitDistance(ANIM_UPPERCUT_LEFT, 25);
            SetHitDistance(ANIM_UPPERCUT_RIGHT, 25);
            SetHitDistance(ANIM_HIGH_KICK_LEFT, 30);
            SetHitDistance(ANIM_HIGH_KICK_RIGHT, 30);
            SetHitDistance(ANIM_CROUCH_LEFT_KICK, 25);
            SetHitDistance(ANIM_CROUCH_RIGHT_KICK, 25);
            
            SetHitDistance(ANIM_YOGA_FIRE, 5);

            RegisterAnimation(ANIM_YOGA_FIRE, "Dhalsim-yogafire.gif", 1000);
            RegisterAnimation(ANIM_UPPERCUT_RIGHT, "Dhalsim-UpperCut-Right.gif", 70);
            RegisterAnimation(ANIM_UPPERCUT_LEFT, "Dhalsim-UpperCut-Left.gif", 70);
            RegisterAnimation(ANIM_HIGH_KICK_LEFT, "Dhalsim-High_Kick-Left.gif", 2500);
            RegisterAnimation(ANIM_HIGH_KICK_RIGHT, "Dhalsim-High_Kick-Right.gif", 2500);
        }

        public override void DoSpecial()
        {
            ChangeAnimation(ANIM_YOGA_FIRE);
        }
    }
}