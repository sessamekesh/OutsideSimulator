using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OutsideSimulator.Commands.Events;

namespace OutsideSimulator.Commands
{
    public class CommandStack : KeyDownSubscriber
    {
        protected Stack<IUndo> UndoStack;
        protected Stack<IUndo> RedoStack;

        public CommandStack()
        {
            UndoStack = new Stack<IUndo>();
            RedoStack = new Stack<IUndo>();
        }

        public void Push(IUndo action)
        {
            UndoStack.Push(action);
            RedoStack.Clear();
        }

        public void Undo()
        {
            if (UndoStack.Count > 0)
            {
                var tr = UndoStack.Pop();
                RedoStack.Push(tr);
                tr.Undo();
            }
        }

        public void Redo()
        {
            if (RedoStack.Count > 0)
            {
                var tr = RedoStack.Pop();
                UndoStack.Push(tr);
                tr.Redo();
            }
        }

        public void OnKeyPress(KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z)
            {
                Undo();
            }
            else if (e.Control && e.KeyCode == Keys.Y)
            {
                Redo();
            }
        }
    }
}
