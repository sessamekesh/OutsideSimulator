using System.Windows.Forms;

namespace OutsideSimulator.Commands.Events
{
    public interface KeyDownSubscriber
    {
        void OnKeyPress(KeyEventArgs e);
    }
}
