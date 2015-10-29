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
    public class ObjectSpawner : KeyDownSubscriber, MouseDownSubscriber, MouseWheelSubscriber, TimerTickSubscriber
    {
        public static readonly List<Type> RenderableList = new List<Type>(new Type[]
        {
            typeof(TestRenderable)
        });

        protected static float frontDist = 15.0f;

        #region State Information
        public bool IsPlacing { get; protected set; }
        public int PlacingID { get; protected set; }
        #endregion

        #region Logical Members
        public Commands.CreateObject CreateObject { get; protected set; }
        public SceneGraph LastPickedObject { get; protected set; }
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
                    CreatedRenderable = System.Activator.CreateInstance(RenderableList[PlacingID]) as IRenderable;
                    CreateObject.Undo();
                    OutsideSimulatorApp.GetInstance().ObjectPicker.ClickedNode = LastPickedObject;
                    Cameras.Camera SceneCamera = OutsideSimulatorApp.GetInstance().SceneCamera;
                    Vector3 lav = (SceneCamera.LookAt - SceneCamera.Position);
                    lav.Normalize();
                    CreateObject = new Commands.CreateObject(OutsideSimulatorApp.GetInstance().ObjectPicker.ClickedNode ?? OutsideSimulatorApp.GetInstance().SceneRootNode,
                        CreatedRenderable,
                        Matrix.Translation(SceneCamera.Position
                        + lav * frontDist));
                    CreateObject.Redo();
                    OutsideSimulatorApp.GetInstance().ObjectPicker.ClickedNode = CreateObject.ParentNode.Children[CreateObject.ChildName];
                }
                else
                {
                    IsPlacing = true;
                    PlacingID = 0;

                    IRenderable CreatedRenderable;
                    CreatedRenderable = System.Activator.CreateInstance(RenderableList[PlacingID]) as IRenderable;
                    Cameras.Camera SceneCamera = OutsideSimulatorApp.GetInstance().SceneCamera;
                    Vector3 lav = (SceneCamera.LookAt - SceneCamera.Position);
                    lav.Normalize();
                    LastPickedObject = OutsideSimulatorApp.GetInstance().ObjectPicker.ClickedNode;
                    CreateObject = new Commands.CreateObject(OutsideSimulatorApp.GetInstance().ObjectPicker.ClickedNode ?? OutsideSimulatorApp.GetInstance().SceneRootNode,
                        CreatedRenderable,
                        Matrix.Translation(SceneCamera.Position
                        + lav * frontDist));
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
            if (IsPlacing)
            {
                Cameras.Camera SceneCamera = OutsideSimulatorApp.GetInstance().SceneCamera;
                Vector3 lav = (SceneCamera.LookAt - SceneCamera.Position);
                lav.Normalize();
                CreateObject.ParentNode.Children[CreateObject.ChildName].Translation =
                    OutsideSimulatorApp.GetInstance().SceneCamera.Position + lav * frontDist;
            }
        }

        public void OnMouseWheel(MouseEventArgs e)
        {
            frontDist += e.Delta * 0.008f;
        }
        #endregion

        protected void PlaceObject()
        {
            OutsideSimulatorApp.GetInstance().CommandStack.Push(CreateObject);
            IsPlacing = false;

            OutsideSimulatorApp.GetInstance().ObjectPicker.ClickedNode = null;
            // Place new object (undo-able action) in our scene,
            //  at its current location (15 units in front of camera, axis-aligned to world)
        }
    }
}
