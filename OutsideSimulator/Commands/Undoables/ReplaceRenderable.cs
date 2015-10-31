using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OutsideSimulator.Renderable;

namespace OutsideSimulator.Commands.Undoables
{
    public class ReplaceRenderable : IUndo
    {
        #region Logic
        public string NodeName { get; protected set; }
        public IRenderable OldRenderable { get; protected set; }
        public IRenderable NewRenderable { get; protected set; }

        private bool _isPerformed;
        #endregion

        public ReplaceRenderable(string NodeName, IRenderable NewRenderable)
        {
            this.NodeName = NodeName;
            this.NewRenderable = NewRenderable;
            OldRenderable = OutsideSimulatorApp.GetInstance().SceneRootNode.GetDescendant(NodeName).Renderable;
            _isPerformed = false;
        }

        #region IUndo
        public void Redo()
        {
            if (_isPerformed)
            {
                throw new InvalidOperationException("This action has already been performed");
            }

            _isPerformed = true;

            OutsideSimulatorApp.GetInstance().SceneRootNode.GetDescendant(NodeName).Renderable = NewRenderable;
        }

        public void Undo()
        {
            if (!_isPerformed)
            {
                throw new InvalidOperationException("This action has not yet been performed");
            }

            _isPerformed = false;

            OutsideSimulatorApp.GetInstance().SceneRootNode.GetDescendant(NodeName).Renderable = OldRenderable;
        }
        #endregion IUndo
    }
}
