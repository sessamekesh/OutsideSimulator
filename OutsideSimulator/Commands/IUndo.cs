using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutsideSimulator.Commands
{
    public interface IUndo
    {
        void Undo();
        void Redo();
    }
}
