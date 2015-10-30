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
        
        #endregion

        #region Ctor
        public DeleteObject()
        {

        }
        #endregion

        #region IUndo
        public void Redo()
        {
            throw new NotImplementedException();
        }

        public void Undo()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
