using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OutsideSimulator.Commands.Events;

using SlimDX;

namespace OutsideSimulator.Scene.UserInteractions
{
    /// <summary>
    /// Handles all the moving of an object
    /// </summary>
    public class ObjectMover : MouseDownSubscriber, KeyDownSubscriber, TimerTickSubscriber, MouseWheelSubscriber
    {
        #region Logical
        public SceneGraph Node;
        public Vector3 StartPos;
        protected static ObjectMover _inst;
        public static float frontDist = 15.0f;
        #endregion

        /// <summary>
        /// </summary>
        /// <param name="Node">The object which to move</param>
        private ObjectMover()
        { }

        public static ObjectMover getInstance()
        {
            if (_inst == null)
            {
                _inst = new ObjectMover();
                OutsideSimulatorApp.GetInstance().Subscribe(_inst);
            }

            return _inst;
        }

        public void SetNode(SceneGraph Node)
        {
            this.Node = Node;
            StartPos = Node.Translation;
        }

        #region Subscriptions
        public void OnKeyPress(KeyEventArgs e)
        {
            if (!e.Control && !e.Alt && !e.Shift && e.KeyCode == Keys.Enter)
            {
                FinalizeMovement();
            }
        }

        public void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                FinalizeMovement();
            }
        }

        public void Update(float dt)
        {
            // Move the object to be with the camera
            if(Node != null) MoveObjectToInFrontOfCamera(Node);
        }

        public void OnMouseWheel(MouseEventArgs e)
        {
            frontDist += e.Delta * 0.008f;
        }
        #endregion

        #region Logical Methods
        /// <summary>
        /// Create a MoveAction for this node
        /// </summary>
        public void FinalizeMovement()
        {
            if (Node == null) return;

            var mo = new Commands.Undoables.MoveObject(
                OutsideSimulatorApp.GetInstance().SceneRootNode.GetDescendentName(Node),
                StartPos,
                Node.Translation
            );

            mo.Redo();
            OutsideSimulatorApp.GetInstance().CommandStack.Push(mo);

            Node = null;
        }

        public void MoveObjectToInFrontOfCamera(SceneGraph Node)
        {
            Cameras.Camera SceneCamera = OutsideSimulatorApp.GetInstance().SceneCamera;
            Vector3 lav = (SceneCamera.LookAt - SceneCamera.Position);
            lav.Normalize();
            Node.Translation = OutsideSimulatorApp.GetInstance().SceneCamera.Position + lav * frontDist;
        }
        #endregion
    }
}
