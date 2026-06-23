using System.Drawing;
using System.Windows.Forms;

namespace Game_Project.Interfaces
{
    /// <summary>
    /// Contract for anything that can paint itself onto a Graphics context.
    /// Allows the renderer to work with any renderable component without knowing
    /// the concrete type (Dependency Inversion Principle).
    /// </summary>
    public interface IRenderable
    {
        void Render(Graphics g, int canvasWidth, int canvasHeight);
    }
}
