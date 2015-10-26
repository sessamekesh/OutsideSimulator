using System.Windows.Forms;

namespace OutsideSimulator.Commands.Events
{
    public interface MouseMoveSubscriber
    {
        void OnMouseMove(MouseEventArgs e);
    }
}
