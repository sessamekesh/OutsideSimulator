using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SlimDX;

using OutsideSimulator.Commands.Events;
using OutsideSimulator.Renderable;

namespace OutsideSimulator.Scene.UserInteractions
{
    public class ObjectSpawner : KeyDownSubscriber, MouseDownSubscriber, TimerTickSubscriber
    {
        public static readonly List<Type> RenderableList = new List<Type>(new Type[]
        {
            typeof(TestRenderable)
        });

        #region State Information
        public bool IsPlacing { get; protected set; }
        public int PlacingID { get; protected set; }
        #endregion

        #region Logical Members
        public Commands.CreateObject CreateObject { get; protected set; }
        #endregion

        #region Listener Implementations
        public void OnKeyPress(KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.N)
            {
                // New Object if not placing, otherwise cycle through renderable types
                if (IsPlacing)
                {
                    PlacingID++;
                    PlacingID %= RenderableList.Count;

                    IRenderable CreatedRenderable;
                    switch (PlacingID)
                    {
                        case 0:
                            CreatedRenderable = new TestRenderable();
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    CreateObject.Undo();
                    Cameras.Camera SceneCamera = OutsideSimulatorApp.GetInstance().SceneCamera;
                    Vector3 lav = (SceneCamera.LookAt - SceneCamera.Position);
                    lav.Normalize();
                    CreateObject = new Commands.CreateObject(OutsideSimulatorApp.GetInstance().ObjectPicker.ClickedNode ?? OutsideSimulatorApp.GetInstance().SceneRootNode,
                        CreatedRenderable,
                        Matrix.Translation(SceneCamera.Position
                        + lav * 3.0f));
                    CreateObject.Redo();
                    OutsideSimulatorApp.GetInstance().ObjectPicker.ClickedNode = CreateObject.ParentNode.Children[CreateObject.ChildName];
                }
                else
                {
                    IsPlacing = true;
                    PlacingID = 0;

                    IRenderable CreatedRenderable;
                    switch (PlacingID)
                    {
                        case 0:
                            CreatedRenderable = new TestRenderable();
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    Cameras.Camera SceneCamera = OutsideSimulatorApp.GetInstance().SceneCamera;
                    Vector3 lav = (SceneCamera.LookAt - SceneCamera.Position);
                    lav.Normalize();
                    CreateObject = new Commands.CreateObject(OutsideSimulatorApp.GetInstance().ObjectPicker.ClickedNode ?? OutsideSimulatorApp.GetInstance().SceneRootNode,
                        CreatedRenderable,
                        Matrix.Translation(SceneCamera.Position
                        + lav * 3.0f));
                    CreateObject.Redo();
                    OutsideSimulatorApp.GetInstance().ObjectPicker.ClickedNode = CreateObject.ParentNode.Children[CreateObject.ChildName];
                }
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (IsPlacing)
                {
                    PlaceObject();
                }
            }
        }

        public void OnMouseDown(MouseEventArgs e)
        {
            if (IsPlacing && e.Button == MouseButtons.Left)
            {
                PlaceObject();
            }
        }

        public void Update(float dt)
        {
            if (IsPlacing && false)
            {
                CreateObject.ParentNode.Children[CreateObject.ChildName].Translation =
                    OutsideSimulatorApp.GetInstance().SceneCamera.Position + OutsideSimulatorApp.GetInstance().SceneCamera.LookAt * 15.0f;
            }
        }
        #endregion

        protected void PlaceObject()
        {
            // Place new object (undo-able action) in our scene,
            //  at its current location (15 units in front of camera, axis-aligned to world)
        }
    }
}
