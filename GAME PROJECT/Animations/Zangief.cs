using System.Drawing;

namespace Game_Project
{
    public class Zangief : BaseCharacter
    {
        private const string ANIM_SPINNING = "SpinningPiledriver";

        public Zangief(Point startPosition, bool mirrored = false)
            : base("Zangief", startPosition, mirrored, Color.Magenta)
        {
            SetHitDistance(ANIM_LEFT_PUNCH, 10);   
            SetHitDistance(ANIM_RIGHT_PUNCH, 10);
            SetHitDistance(ANIM_UPPERCUT_LEFT, 130);
            SetHitDistance(ANIM_UPPERCUT_RIGHT, 130);
            SetHitDistance(ANIM_HIGH_KICK_LEFT, 100);
            SetHitDistance(ANIM_HIGH_KICK_RIGHT, 100);
            SetHitDistance(ANIM_SPINNING, 60);

            RegisterAnimation(ANIM_SPINNING, "Zangief-spinning.gif", 200);
            RegisterAnimation(ANIM_BACKWARD, "Zangief-Backward.gif", 250);
            RegisterAnimation(ANIM_CROUCH_LEFT_KICK, "Zangief-crouch-left.gif", 170);
            RegisterAnimation(ANIM_CROUCH_RIGHT_KICK, "Zangief-crouch-right.gif", 170);
            RegisterAnimation(ANIM_HIGH_KICK_LEFT, "Zangief-High_Kick-Left.gif", 170);
            RegisterAnimation(ANIM_HIGH_KICK_RIGHT, "Zangief-High_Kick-Right.gif", 170);
            RegisterAnimation(ANIM_LEFT_PUNCH, "Zangief-LeftPunch.gif", 450);
            RegisterAnimation(ANIM_RIGHT_PUNCH, "Zangief-RightPunch.gif", 450);
        }

        public override void DoSpecial()
        {
            ChangeAnimation(ANIM_SPINNING);
        }
    }
}