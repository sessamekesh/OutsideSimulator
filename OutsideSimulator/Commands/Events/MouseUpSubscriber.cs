using System.Windows.Forms;

namespace OutsideSimulator.Commands.Events
{
    public interface MouseUpSubscriber
    {
        void OnMouseUp(MouseEventArgs e);
    }
}
