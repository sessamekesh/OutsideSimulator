using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SlimDX;

namespace OutsideSimulator.Commands.Undoables
{
    public class MoveObject : IUndo
    {
        #region Logic
        protected string Name;
        public Vector3 OldPos;
        public Vector3 NewPos;

        private bool _isImplemented;
        #endregion

        public MoveObject(string name, Vector3 oldPos, Vector3 newPos)
        {
            Name = name;
            OldPos = oldPos;
            NewPos = newPos;
            _isImplemented = false;
        }

        #region IUndo
        public void Redo()
        {
            if (_isImplemented) throw new InvalidOperationException("MoveObject action already performed!");

            OutsideSimulatorApp.GetInstance().SceneRootNode.GetDescendant(Name).Translation = NewPos;

            _isImplemented = true;
        }

        public void Undo()
        {
            if (!_isImplemented) throw new InvalidOperationException("MoveObject action has not yet been performed!");

            OutsideSimulatorApp.GetInstance().SceneRootNode.GetDescendant(Name).Translation = OldPos;

            _isImplemented = false;
        }
        #endregion
    }
}
