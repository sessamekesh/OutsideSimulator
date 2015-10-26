using System.Windows.Forms;

namespace OutsideSimulator.Commands.Events
{
    public interface KeyUpSubscriber
    {
        void OnKeyUp(KeyEventArgs e);
    }
}
