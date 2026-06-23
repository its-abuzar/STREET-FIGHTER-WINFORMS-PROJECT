using System.Drawing;

namespace Game_Project
{
    public class Blanka : BaseCharacter
    {
        private const string ANIM_ELECTRICITY = "Electricity";

        public Blanka(Point startPosition, bool mirrored = false)
            : base("Blanka", startPosition, mirrored, Color.Magenta)
        {
            SetHitDistance(ANIM_LEFT_PUNCH, 70);
            SetHitDistance(ANIM_RIGHT_PUNCH, 70);
            SetHitDistance(ANIM_UPPERCUT_LEFT, 100);
            SetHitDistance(ANIM_UPPERCUT_RIGHT, 40);
            SetHitDistance(ANIM_HIGH_KICK_LEFT, 25);
            SetHitDistance(ANIM_HIGH_KICK_RIGHT, 30);
            SetHitDistance(ANIM_CROUCH_LEFT_KICK, 80);
            SetHitDistance(ANIM_CROUCH_RIGHT_KICK, 80);
            SetHitDistance(ANIM_ELECTRICITY, 1);

            RegisterAnimation(ANIM_ELECTRICITY, "Blanka-electricity.gif", 100);
            RegisterAnimation(ANIM_UPPERCUT_RIGHT, "Blanka-UpperCut-Right.gif", 350);
            RegisterAnimation(ANIM_UPPERCUT_LEFT, "Blanka-UpperCut-Left.gif", 350);
        }

        public override void DoSpecial()
        {
            ChangeAnimation(ANIM_ELECTRICITY);
        }
    }
}