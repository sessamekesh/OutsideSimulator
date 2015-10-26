using System.Windows.Forms;

namespace OutsideSimulator.Commands.Events
{
    public interface MouseWheelSubscriber
    {
        void OnMouseWheel(MouseEventArgs e);
    }
}
