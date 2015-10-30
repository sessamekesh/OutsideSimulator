using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SlimDX;

using OutsideSimulator.D3DCore;

using System.Xml.Linq;

namespace OutsideSimulator.Commands
{
    public static class UndoSerializer
    {
        public static XElement Serialize(IUndo undo)
        {
            if (undo is CreateObject)
            {
                var co = undo as CreateObject;

                var xe = new XElement("CreateObject");

                xe.Add(new XElement("Transformation", Util.SerializeMatrix(co.Transform)));
                xe.Add(new XElement("Renderable", Renderable.RenderableFactory.Serialize(co.Renderable)));

                return xe;
            }
            else
            {
                return null;
            }
        }

        public static IUndo Deserialize(string XMLString)
        {
            var xe = XElement.Parse(XMLString);

            if (xe.Name == "CreateObject")
            {
                SlimDX.Matrix transform = Util.DeserializeMatrix((xe.Nodes().First((x) => (x as XElement).Name == "Transformation") as XElement).FirstNode.ToString()).Value;
                Renderable.IRenderable renderable = Renderable.RenderableFactory.Deserialize((xe.Nodes().First((x) => (x as XElement).Name == "Renderable") as XElement).FirstNode.ToString());

                return new CreateObject(renderable, transform);
            }
            else
            {
                return null;
            }
        }
    }
}
