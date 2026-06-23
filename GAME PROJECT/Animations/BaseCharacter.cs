using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Game_Project
{
    public abstract class BaseCharacter
    {
        public string Name { get; private set; }
        public Point Position { get; set; }
        public bool IsMirrored { get; private set; }
        public Image CurrentFrame { get; set; }

        public string CurrentAnimation { get; private set; }

        private Dictionary<string, GifSpeedController> animations = new Dictionary<string, GifSpeedController>();

        // Universal animation keys
        protected const string ANIM_IDLE = "Idle";
        protected const string ANIM_FORWARD = "Forward";
        protected const string ANIM_BACKWARD = "Backward";
        protected const string ANIM_LEFT_PUNCH = "LeftPunch";
        protected const string ANIM_RIGHT_PUNCH = "RightPunch";
        protected const string ANIM_HIGH_KICK_LEFT = "HighKickLeft";
        protected const string ANIM_HIGH_KICK_RIGHT = "HighKickRight";
        protected const string ANIM_CROUCH_LEFT_KICK = "CrouchLeftKick";
        protected const string ANIM_CROUCH_RIGHT_KICK = "CrouchRightKick";
        protected const string ANIM_UPPERCUT_LEFT = "UppercutLeft";
        protected const string ANIM_UPPERCUT_RIGHT = "UppercutRight";

        private static int groundLevel = 430;
        private Color transparentColor;

        // Hit distance system
        private Dictionary<string, int> hitDistances = new Dictionary<string, int>();

        // Hit reaction properties
        private Timer hitStunTimer;
        private Timer invincibleTimer;
        public bool IsHitStunned { get; private set; }
        public bool IsInvincible { get; private set; }
        public int Knockback { get; set; } = 20;
        public int HitStunDuration { get; set; } = 300;
        public int InvincibleDuration { get; set; } = 500;

        protected void SetHitDistance(string animationKey, int distance)
        {
            hitDistances[animationKey] = distance;
        }

        public int GetCurrentHitDistance()
        {
            if (hitDistances.TryGetValue(CurrentAnimation, out int dist))
                return dist;
            return 0;
        }

        public BaseCharacter(string name, Point startPosition, bool mirrored = false, Color? transparentColor = null)
        {
            Name = name;
            Position = startPosition;
            IsMirrored = mirrored;
            this.transparentColor = transparentColor ?? Color.Magenta;
            try
            {
                LoadAllAnimations();
                SetIdle();
            }
            catch
            {
                Name = "Ryu";
                LoadAllAnimations();
                SetIdle();
            }
        }

        protected void RegisterAnimation(string animationKey, string fileName, int speedPercent = 100)
        {
            string basePath = Path.Combine(Application.StartupPath, "Assets", "Characters", Name);
            string path = FindFile(basePath, fileName);
            if (!string.IsNullOrEmpty(path))
            {
                var ctrl = new GifSpeedController();
                ctrl.TransparentColor = transparentColor;
                ctrl.Load(path, speedPercent, IsMirrored);
                ctrl.OnFrameChanged += (img) => { if (CurrentAnimation == animationKey) CurrentFrame = img; };
                animations[animationKey] = ctrl;
            }
        }

        private string FindFile(string directory, string fileName)
        {
            if (!Directory.Exists(directory)) return null;
            var files = Directory.GetFiles(directory, fileName, SearchOption.TopDirectoryOnly);
            if (files.Length > 0) return files[0];
            files = Directory.GetFiles(directory, "*" + Path.GetExtension(fileName))
                             .Where(f => Path.GetFileNameWithoutExtension(f)
                                         .Equals(Path.GetFileNameWithoutExtension(fileName), StringComparison.OrdinalIgnoreCase))
                             .ToArray();
            return files.Length > 0 ? files[0] : null;
        }

        protected virtual void LoadAllAnimations()
        {
            string basePath = Path.Combine(Application.StartupPath, "Assets", "Characters", Name);
            if (!Directory.Exists(basePath)) return;

            var animDefs = new Dictionary<string, (string fileName, int speed)>
            {
                [ANIM_IDLE] = ($"{Name}-Standing.gif", 100),
                [ANIM_FORWARD] = ($"{Name}-Forward.gif", 100),
                [ANIM_BACKWARD] = ($"{Name}-Backward.gif", 100),
                [ANIM_LEFT_PUNCH] = ($"{Name}-LeftPunch.gif", 50),
                [ANIM_RIGHT_PUNCH] = ($"{Name}-RightPunch.gif", 70),
                [ANIM_HIGH_KICK_LEFT] = ($"{Name}-High_Kick-Left.gif", 120),
                [ANIM_HIGH_KICK_RIGHT] = ($"{Name}-High_Kick-Right.gif", 120),
                [ANIM_CROUCH_LEFT_KICK] = ($"{Name}-crouch-left.gif", 50),
                [ANIM_CROUCH_RIGHT_KICK] = ($"{Name}-crouch-right.gif", 70),
                [ANIM_UPPERCUT_LEFT] = ($"{Name}-UpperCut-Left.gif", 110),
                [ANIM_UPPERCUT_RIGHT] = ($"{Name}-UpperCut-Right.gif", 140)
            };

            foreach (var kv in animDefs)
            {
                string path = FindFile(basePath, kv.Value.fileName);
                if (!string.IsNullOrEmpty(path))
                {
                    var ctrl = new GifSpeedController();
                    ctrl.TransparentColor = transparentColor;
                    ctrl.Load(path, kv.Value.speed, IsMirrored);
                    ctrl.OnFrameChanged += (img) => { if (CurrentAnimation == kv.Key) CurrentFrame = img; };
                    animations[kv.Key] = ctrl;
                }
            }

            if (animations.Count == 0)
                throw new Exception($"No animations found for {Name}");
        }

        public void ChangeAnimation(string animationName)
        {
            if (CurrentAnimation == animationName) return;

            if (CurrentAnimation != null && animations.TryGetValue(CurrentAnimation, out var oldCtrl))
                oldCtrl.Stop();

            if (animations.TryGetValue(animationName, out var newCtrl))
            {
                newCtrl.Play(this);
                CurrentAnimation = animationName;
            }
            else if (animations.TryGetValue(ANIM_IDLE, out var idleCtrl))
            {
                idleCtrl.Play(this);
                CurrentAnimation = ANIM_IDLE;
            }

            AlignToGround();
        }

        public void SetIdle() => ChangeAnimation(ANIM_IDLE);

        public virtual void PunchLeft() => ChangeAnimation(ANIM_LEFT_PUNCH);
        public virtual void PunchRight() => ChangeAnimation(ANIM_RIGHT_PUNCH);
        public virtual void KickHighLeft() => ChangeAnimation(ANIM_HIGH_KICK_LEFT);
        public virtual void KickHighRight() => ChangeAnimation(ANIM_HIGH_KICK_RIGHT);
        public virtual void MoveForward() => ChangeAnimation(ANIM_FORWARD);
        public virtual void MoveBackward() => ChangeAnimation(ANIM_BACKWARD);
        public virtual void CrouchKickLeft() => ChangeAnimation(ANIM_CROUCH_LEFT_KICK);
        public virtual void CrouchKickRight() => ChangeAnimation(ANIM_CROUCH_RIGHT_KICK);
        public virtual void UppercutLeft() => ChangeAnimation(ANIM_UPPERCUT_LEFT);
        public virtual void UppercutRight() => ChangeAnimation(ANIM_UPPERCUT_RIGHT);
        public virtual void ReturnToIdle() => SetIdle();
        public virtual void DoSpecial() { }

        public void AlignToGround()
        {
            if (CurrentFrame == null) return;
            Position = new Point(Position.X, groundLevel - CurrentFrame.Height);
        }

        public void ApplyHitReaction(int knockback, int hitStunMs, int invincibleMs)
        {
            if (IsInvincible) return;

            Knockback = knockback;
            HitStunDuration = hitStunMs;
            InvincibleDuration = invincibleMs;

            // Stop current animation
            if (animations.TryGetValue(CurrentAnimation, out var currentCtrl))
                currentCtrl.Stop();

            IsHitStunned = true;
            IsInvincible = true;

            // If a hit animation exists, play it; otherwise freeze current frame
            if (animations.ContainsKey("Hit"))
                ChangeAnimation("Hit");
            else
            {
                // Keep CurrentFrame unchanged (freeze)
            }

            // Clear existing timers if any
            hitStunTimer?.Stop();
            hitStunTimer?.Dispose();
            invincibleTimer?.Stop();
            invincibleTimer?.Dispose();

            // End hit stun
            hitStunTimer = new Timer();
            hitStunTimer.Interval = HitStunDuration;
            hitStunTimer.Tick += (s, e) =>
            {
                IsHitStunned = false;
                ReturnToIdle();
                hitStunTimer.Stop();
                hitStunTimer.Dispose();
                hitStunTimer = null;
            };
            hitStunTimer.Start();

            // End invincibility
            invincibleTimer = new Timer();
            invincibleTimer.Interval = InvincibleDuration;
            invincibleTimer.Tick += (s, e) =>
            {
                IsInvincible = false;
                invincibleTimer.Stop();
                invincibleTimer.Dispose();
                invincibleTimer = null;
            };
            invincibleTimer.Start();
        }

        public void Dispose()
        {
            foreach (var ctrl in animations.Values)
                ctrl?.Dispose();
            animations.Clear();
            hitStunTimer?.Stop();
            hitStunTimer?.Dispose();
            invincibleTimer?.Stop();
            invincibleTimer?.Dispose();
        }
    }
}