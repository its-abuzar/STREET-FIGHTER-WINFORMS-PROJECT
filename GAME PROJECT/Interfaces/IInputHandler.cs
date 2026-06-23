using System.Windows.Forms;

namespace Game_Project.Interfaces
{
    /// <summary>
    /// Defines the contract for processing raw keyboard input into game actions.
    /// Follows Interface Segregation Principle – consumers depend only on what they need.
    /// </summary>
    public interface IInputHandler
    {
        bool HandleKeyDown(Keys keyData);
        void HandleKeyUp(Keys keyCode);
    }
}
