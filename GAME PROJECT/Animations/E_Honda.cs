using System.Drawing;

namespace Game_Project
{
    public class E_Honda : BaseCharacter
    {
        private const string ANIM_HANDSLAP = "HandSlap";

        public E_Honda(Point startPosition, bool mirrored = false)
            : base("E. Honda", startPosition, mirrored, Color.Magenta)
        {
            // Standing width: 155px
            SetHitDistance(ANIM_LEFT_PUNCH, 70);    // punch width 248 → reach 93
            SetHitDistance(ANIM_RIGHT_PUNCH, 70);
            SetHitDistance(ANIM_UPPERCUT_LEFT, 30);
            SetHitDistance(ANIM_UPPERCUT_RIGHT, 30);
            SetHitDistance(ANIM_HIGH_KICK_LEFT, 60); // kick width 245 → reach 90
            SetHitDistance(ANIM_HIGH_KICK_RIGHT, 60);
            SetHitDistance(ANIM_CROUCH_LEFT_KICK, 60);
            SetHitDistance(ANIM_CROUCH_RIGHT_KICK, 60);
            SetHitDistance(ANIM_HANDSLAP, 30);

            RegisterAnimation(ANIM_HANDSLAP, "E. Honda-handSlap.gif", 100);
            RegisterAnimation(ANIM_LEFT_PUNCH, "E. Honda-LeftPunch.gif", 250);
            RegisterAnimation(ANIM_RIGHT_PUNCH, "E. Honda-RightPunch.gif", 250);
            RegisterAnimation(ANIM_UPPERCUT_RIGHT, "E. Honda-UpperCut-Right.gif", 200);
        }

        public override void DoSpecial()
        {
            ChangeAnimation(ANIM_HANDSLAP);
        }
    }
}