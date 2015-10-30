using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
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

        public XElement Serialize()
        {
            var tr = new XElement("CommandStack");
            tr.Add(new XElement("Count", UndoStack.Count));
            int i = 0;

            foreach (var undoable in UndoStack)
            {
                tr.Add(new XElement("c" + i.ToString(), UndoSerializer.Serialize(undoable)));
                i++;
            }

            return tr;
        }

        public static CommandStack Deserialize(string XMLString)
        {
            CommandStack tr = new CommandStack();

            var xe = XElement.Parse(XMLString);

            if (xe.Name != "CommandStack")
            {
                return null;
            }

            int count = int.Parse((xe.Nodes().First((x) => (x as XElement).Name == "Count") as XElement).Value);

            foreach (var node in xe.Nodes())
            {
                if ((node as XElement).Name == "Count") continue;
                --count;
                tr.Push(UndoSerializer.Deserialize((xe.Nodes().First((x) => (x as XElement).Name == "c" + count.ToString()) as XElement).FirstNode.ToString()));
            }

            return tr;
        }
    }
}
