using System.Drawing;

namespace Game_Project.Managers
{
    /// <summary>
    /// Factory that instantiates the correct BaseCharacter subclass by name.
    /// Open/Closed Principle: add a new character by adding a case here,
    /// with no changes to the consumer (FightScreen).
    /// Liskov Substitution: every concrete character IS-A BaseCharacter and
    /// is fully substitutable wherever BaseCharacter is expected.
    /// </summary>
    public static class CharacterFactory
    {
        public static BaseCharacter Create(string name, Point startPos, bool mirrored)
        {
            switch (name)
            {
                case "Ryu":     return new Ryu(startPos, mirrored);
                case "Ken":     return new Ken(startPos, mirrored);
                case "E. Honda": return new E_Honda(startPos, mirrored);
                case "Blanka":  return new Blanka(startPos, mirrored);
                case "Chun-Li": return new ChunLi(startPos, mirrored);
                case "Guile":   return new Guile(startPos, mirrored);
                case "Zangief": return new Zangief(startPos, mirrored);
                case "Dhalsim": return new Dhalsim(startPos, mirrored);
                default:        return new Ryu(startPos, mirrored);
            }
        }
    }
}
