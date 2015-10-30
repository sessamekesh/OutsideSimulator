using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OutsideSimulator.Commands
{
    public interface IUndo
    {
        void Undo();
        void Redo();
    }
}
