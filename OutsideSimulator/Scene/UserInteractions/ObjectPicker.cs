﻿using System;
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
    public class ObjectPicker : MouseDownSubscriber, MouseUpSubscriber, KeyUpSubscriber
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
            if (OutsideSimulatorApp.GetInstance().MainMenu != null &&
                OutsideSimulatorApp.GetInstance().MainMenu.ActiveMenu != null) return;

            if (e.Button == MouseButtons.Left)
                ClickedNode = Picker.PickClosestObject(OutsideSimulatorApp.GetInstance().SceneRootNode, OutsideSimulatorApp.GetInstance().SceneCamera,
                    OutsideSimulatorApp.GetInstance().ProjectionMatrix, new SlimDX.Vector2(e.X, e.Y), new SlimDX.Vector2(
                        OutsideSimulatorApp.GetInstance().Width, OutsideSimulatorApp.GetInstance().Height));
        }

        public void OnMouseUp(MouseEventArgs e)
        {
            if (OutsideSimulatorApp.GetInstance().MainMenu != null &&
                OutsideSimulatorApp.GetInstance().MainMenu.ActiveMenu != null)
                return;

            if (e.Button == MouseButtons.Left)
                if (ClickedNode == Picker.PickClosestObject(OutsideSimulatorApp.GetInstance().SceneRootNode, OutsideSimulatorApp.GetInstance().SceneCamera,
                    OutsideSimulatorApp.GetInstance().ProjectionMatrix, new SlimDX.Vector2(e.X, e.Y), new SlimDX.Vector2(
                        OutsideSimulatorApp.GetInstance().Width, OutsideSimulatorApp.GetInstance().Height)))
                {
                    // The clicked node is the selected node!
                }
        }

        public void OnKeyUp(KeyEventArgs e)
        {
            if (ClickedNode == null) return;

            if (e.Control == false && e.Shift == false && e.Alt == false)
            {
                if (e.KeyCode == Keys.Delete)
                {
                    DeleteSelected();
                }
                else if (e.KeyCode == Keys.Tab)
                {
                    GetNextRenderable();
                }
            }
        }

        public void DeleteSelected()
        {
            //
            // Delete selected node
            //

            // Get name of descendant
            var childName = OutsideSimulatorApp.GetInstance().SceneRootNode.Children.First((x) => x.Value == ClickedNode).Key;

            var removeAction = new Commands.Undoables.DeleteObject(childName);
            removeAction.Redo();

            OutsideSimulatorApp.GetInstance().CommandStack.Push(removeAction);
        }

        public void GetNextRenderable()
        {
            //
            // Cycle through generation chain, swap out objects.
            //
            if (!(OutsideSimulatorApp.GetInstance().SceneRootNode.Children.Count((x) => x.Value == ClickedNode) > 0)) return;

            var childName = OutsideSimulatorApp.GetInstance().SceneRootNode.Children.First((x) => x.Value == ClickedNode).Key;
            var nextChain = Renderable.RenderableFactory.GetNextRenderableInModificationChain(ClickedNode.Renderable);

            if (nextChain != null)
            {
                var swapoutaction = new Commands.Undoables.ReplaceRenderable(childName, nextChain);
                swapoutaction.Redo();
                OutsideSimulatorApp.GetInstance().CommandStack.Push(swapoutaction);
            }
        }
    }
}
