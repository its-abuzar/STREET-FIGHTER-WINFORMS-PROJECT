using System.Drawing;

namespace Game_Project
{
    public class ChunLi : BaseCharacter
    {
        private const string ANIM_SPINNING_BIRD = "SpinningBird";

        public ChunLi(Point startPosition, bool mirrored = false)
            : base("Chun-Li", startPosition, mirrored, Color.Magenta)
        {
            // Standing width: 120px
            SetHitDistance(ANIM_LEFT_PUNCH, 60);    // punch width 213 → reach 93
            SetHitDistance(ANIM_RIGHT_PUNCH, 60);
            SetHitDistance(ANIM_UPPERCUT_LEFT, 40);
            SetHitDistance(ANIM_UPPERCUT_RIGHT, 40);
            SetHitDistance(ANIM_HIGH_KICK_LEFT, 60);
            SetHitDistance(ANIM_HIGH_KICK_RIGHT, 60);
            SetHitDistance(ANIM_CROUCH_LEFT_KICK, 80);
            SetHitDistance(ANIM_CROUCH_RIGHT_KICK, 80);
            SetHitDistance(ANIM_SPINNING_BIRD, 30);

            RegisterAnimation(ANIM_SPINNING_BIRD, "Chun-Li-spinningbird.gif", 400);
            RegisterAnimation(ANIM_HIGH_KICK_RIGHT, "Chun-Li-High_Kick-Right.gif", 60);
        }

        public override void DoSpecial()
        {
            ChangeAnimation(ANIM_SPINNING_BIRD);
        }
    }
}