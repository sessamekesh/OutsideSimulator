using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OutsideSimulator.Scene;
using OutsideSimulator.Renderable;

using SlimDX;

namespace OutsideSimulator.Commands.Undoables
{
    public class CreateObject : IUndo
    {
        #region Logical Members
        public SceneGraph ParentNode { get; protected set; }
        public string ChildName { get; protected set; }
        public IRenderable Renderable { get; protected set; }
        public Matrix Transform { get; protected set; }

        protected bool IsPerformed;
        #endregion

        /// <summary>
        /// Create the action to create an object
        /// </summary>
        /// <param name="parent">The parent scene node to which to attach the object</param>
        /// <param name="renderable">The renderable to provide to the node</param>
        public CreateObject(IRenderable renderable, Matrix transform)
        {
            IsPerformed = false;
            ParentNode = OutsideSimulatorApp.GetInstance().SceneRootNode;
            ChildName = (ParentNode.GetHashCode() + DateTime.Now.GetHashCode()).ToString();
            Renderable = renderable;
            Transform = transform;
        }

        #region Interface Implementations
        public void Redo()
        {
            if (IsPerformed)
            {
                throw new InvalidOperationException("CreateObject on " + ChildName + " already performed!");
            }
            else
            {
                ParentNode.AttachChild(ChildName, new SceneGraph(Transform, Renderable));
                IsPerformed = true;
            }
        }

        public void Undo()
        {
            if (IsPerformed)
            {
                ParentNode.Children.Remove(ChildName);
                IsPerformed = false;
            }
            else
            {
                throw new InvalidOperationException("Undo CreateObject on " + ChildName + " cannot be performed - action has not been performed");
            }
        }
        #endregion
    }
}
