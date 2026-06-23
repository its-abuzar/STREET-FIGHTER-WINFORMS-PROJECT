using System.IO;
using System.Windows.Forms;

namespace Game_Project
{
    public class KeyBindings
    {
        // Player 1
        public Keys P1_Left { get; set; } = Keys.Left;
        public Keys P1_Right { get; set; } = Keys.Right;
        public Keys P1_Up { get; set; } = Keys.Up;
        public Keys P1_Down { get; set; } = Keys.Down;
        public Keys P1_LightPunch { get; set; } = Keys.A;
        public Keys P1_HeavyPunch { get; set; } = Keys.S;
        public Keys P1_LightKick { get; set; } = Keys.Z;
        public Keys P1_HeavyKick { get; set; } = Keys.X;
        public Keys P1_Special { get; set; } = Keys.D;

        // Player 2
        public Keys P2_Left { get; set; } = Keys.G;
        public Keys P2_Right { get; set; } = Keys.J;
        public Keys P2_Up { get; set; } = Keys.Y;
        public Keys P2_Down { get; set; } = Keys.H;
        public Keys P2_LightPunch { get; set; } = Keys.B;
        public Keys P2_HeavyPunch { get; set; } = Keys.N;
        public Keys P2_LightKick { get; set; } = Keys.M;
        public Keys P2_HeavyKick { get; set; } = Keys.Oemcomma;   // ,
        public Keys P2_Special { get; set; } = Keys.OemPeriod;     // .

        public void Save(string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine((int)P1_Left);
                sw.WriteLine((int)P1_Right);
                sw.WriteLine((int)P1_Up);
                sw.WriteLine((int)P1_Down);
                sw.WriteLine((int)P1_LightPunch);
                sw.WriteLine((int)P1_HeavyPunch);
                sw.WriteLine((int)P1_LightKick);
                sw.WriteLine((int)P1_HeavyKick);
                sw.WriteLine((int)P1_Special);
                sw.WriteLine((int)P2_Left);
                sw.WriteLine((int)P2_Right);
                sw.WriteLine((int)P2_Up);
                sw.WriteLine((int)P2_Down);
                sw.WriteLine((int)P2_LightPunch);
                sw.WriteLine((int)P2_HeavyPunch);
                sw.WriteLine((int)P2_LightKick);
                sw.WriteLine((int)P2_HeavyKick);
                sw.WriteLine((int)P2_Special);
            }
        }

        public void Load(string path)
        {
            if (!File.Exists(path)) return;
            using (StreamReader sr = new StreamReader(path))
            {
                P1_Left = (Keys)int.Parse(sr.ReadLine());
                P1_Right = (Keys)int.Parse(sr.ReadLine());
                P1_Up = (Keys)int.Parse(sr.ReadLine());
                P1_Down = (Keys)int.Parse(sr.ReadLine());
                P1_LightPunch = (Keys)int.Parse(sr.ReadLine());
                P1_HeavyPunch = (Keys)int.Parse(sr.ReadLine());
                P1_LightKick = (Keys)int.Parse(sr.ReadLine());
                P1_HeavyKick = (Keys)int.Parse(sr.ReadLine());
                P1_Special = (Keys)int.Parse(sr.ReadLine());
                P2_Left = (Keys)int.Parse(sr.ReadLine());
                P2_Right = (Keys)int.Parse(sr.ReadLine());
                P2_Up = (Keys)int.Parse(sr.ReadLine());
                P2_Down = (Keys)int.Parse(sr.ReadLine());
                P2_LightPunch = (Keys)int.Parse(sr.ReadLine());
                P2_HeavyPunch = (Keys)int.Parse(sr.ReadLine());
                P2_LightKick = (Keys)int.Parse(sr.ReadLine());
                P2_HeavyKick = (Keys)int.Parse(sr.ReadLine());
                P2_Special = (Keys)int.Parse(sr.ReadLine());
            }
        }
    }
}