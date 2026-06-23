using System.Drawing;

namespace Game_Project
{
    public class Ken : BaseCharacter
    {
        private const string ANIM_HURRICANE = "HurricaneKick";

        public Ken(Point startPosition, bool mirrored = false)
            : base("Ken", startPosition, mirrored, Color.Magenta) 
        {
            SetHitDistance(ANIM_LEFT_PUNCH, 30);
            SetHitDistance(ANIM_RIGHT_PUNCH, 30);
            SetHitDistance(ANIM_UPPERCUT_LEFT, 40);
            SetHitDistance(ANIM_UPPERCUT_RIGHT, 50);
            SetHitDistance(ANIM_HIGH_KICK_LEFT, 40); 
            SetHitDistance(ANIM_HIGH_KICK_RIGHT, 40);
            SetHitDistance(ANIM_CROUCH_LEFT_KICK, 65);
            SetHitDistance(ANIM_CROUCH_RIGHT_KICK, 65);
            SetHitDistance(ANIM_HURRICANE, 30);

            RegisterAnimation(ANIM_HURRICANE, "ken-hurricane-loop.gif", 100);
            RegisterAnimation(ANIM_HIGH_KICK_RIGHT, "Ken-High_Kick-Right.gif", 80);
        }

        public override void DoSpecial()
        {
            ChangeAnimation(ANIM_HURRICANE);
        }
    }
}