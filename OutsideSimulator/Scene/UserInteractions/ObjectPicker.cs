using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OutsideSimulator.Commands.Events;
using OutsideSimulator.Scene;
using OutsideSimulator.D3DCore;

namespace OutsideSimulator.Scene.UserInteractions
{
    /// <summary>
    /// Class that handles picking of objects via user clicking
    ///  Picks an object if and only if the mouse was pressed and released on that object.
    /// </summary>
    public class ObjectPicker : MouseDownSubscriber, MouseUpSubscriber
    {
        #region Logical Members
        public SceneGraph ClickedNode { get; set; }
        #endregion

        public ObjectPicker()
        {
            ClickedNode = null;
            OutsideSimulatorApp.GetInstance().Subscribe(this);
        }

        public void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                ClickedNode = Picker.PickClosestObject(OutsideSimulatorApp.GetInstance().SceneRootNode, OutsideSimulatorApp.GetInstance().SceneCamera,
                    OutsideSimulatorApp.GetInstance().ProjectionMatrix, new SlimDX.Vector2(e.X, e.Y), new SlimDX.Vector2(
                        OutsideSimulatorApp.GetInstance().Width, OutsideSimulatorApp.GetInstance().Height));
        }

        public void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                if (ClickedNode == Picker.PickClosestObject(OutsideSimulatorApp.GetInstance().SceneRootNode, OutsideSimulatorApp.GetInstance().SceneCamera,
                    OutsideSimulatorApp.GetInstance().ProjectionMatrix, new SlimDX.Vector2(e.X, e.Y), new SlimDX.Vector2(
                        OutsideSimulatorApp.GetInstance().Width, OutsideSimulatorApp.GetInstance().Height)))
                {
                    // The clicked node is the selected node!
                }
        }
    }
}
