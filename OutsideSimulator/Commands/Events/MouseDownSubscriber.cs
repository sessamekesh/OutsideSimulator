using System.Windows.Forms;

namespace OutsideSimulator.Commands.Events
{
    public interface MouseDownSubscriber
    {
        void OnMouseDown(MouseEventArgs e);
    }
}
