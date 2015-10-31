using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OutsideSimulator.Scene;

namespace OutsideSimulator.Commands.Undoables
{
    public class DeleteObject : IUndo
    {
        #region Members
        public string ObjectName { get; protected set; }
        public SceneGraph RemovedObject { get; protected set; }

        private bool _isExecuted;
        #endregion

        #region Ctor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectName">The name of the node to be deleted, as attached to the master parent node</param>
        public DeleteObject(string objectName)
        {
            ObjectName = objectName;
            RemovedObject = null;
            _isExecuted = false;
        }
        #endregion

        #region IUndo
        public void Redo()
        {
            if (_isExecuted)
            {
                throw new InvalidOperationException("Cannot redo an acction that has already been performed!");
            }

            RemovedObject = OutsideSimulatorApp.GetInstance().SceneRootNode.GetDescendant(ObjectName);

            OutsideSimulatorApp.GetInstance().SceneRootNode.RemoveDescendent(ObjectName);

            _isExecuted = true;
        }

        public void Undo()
        {
            if (!_isExecuted)
            {
                throw new InvalidOperationException("Cannot undo an action that has yet to be performed!");
            }

            _isExecuted = false;

            OutsideSimulatorApp.GetInstance().SceneRootNode.AttachChild(ObjectName, RemovedObject);

            RemovedObject = null;
        }
        #endregion
    }
}
