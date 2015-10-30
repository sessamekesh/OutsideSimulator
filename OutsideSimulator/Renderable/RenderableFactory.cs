using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OutsideSimulator.Renderable
{
    public class RenderableSerializeException : FormatException
    {
        public RenderableSerializeException() : base() { }
        public RenderableSerializeException(string message) : base(message) { }
    }

    public static class RenderableFactory
    {
        /// <summary>
        /// Serialize into an XML element the IRenderable
        /// </summary>
        /// <param name="toSave">The renderable to save</param>
        /// <returns>An XML string (self-contained)</returns>
        public static XElement Serialize(IRenderable toSave)
        {
            if (toSave == null)
            {
                return null;
            }
            else if (toSave is TerrainRenderable)
            {
                TerrainRenderable tr = toSave as TerrainRenderable;
                XElement ts = new XElement("TerrainRenderable",
                    new XElement("width", tr.Width),
                    new XElement("depth", tr.Depth),
                    new XElement("xSubdivisions", tr.XSubdivisions),
                    new XElement("zSubdivisions", tr.ZSubdivisions));

                return ts;
            }
            else if (toSave is TestRenderable)
            {
                TestRenderable tr = toSave as TestRenderable;
                XElement ts = new XElement("TestRenderable", "");
                return ts;
            }
            else
            {
                throw new InvalidOperationException("Cannot serialize - did not recognize renderable type");
            }
        }

        /// <summary>
        /// Create a renderable object from an XML tag
        /// </summary>
        /// <param name="XMLString">The string to parse into an IRenderable</param>
        /// <returns>The corresponding IRenderable</returns>
        public static IRenderable Deserialize(string XMLString)
        {
            if (XMLString == "" || XMLString == null)
            {
                return null;
            }

            // Read in everything after opening '<' to closing '>'
            XElement data = XElement.Parse(XMLString);

            switch (data.Name.LocalName)
            {
                case "TerrainRenderable":
                    float width = float.Parse((data.Nodes().First((x) => (x as XElement).Name == "width") as XElement).Value);
                    float depth  = float.Parse((data.Nodes().First((x) => (x as XElement).Name == "depth") as XElement).Value);
                    uint xsubs = uint.Parse((data.Nodes().First((x) => (x as XElement).Name == "xSubdivisions") as XElement).Value);
                    uint zsubs = uint.Parse((data.Nodes().First((x) => (x as XElement).Name == "zSubdivisions") as XElement).Value);
                    return new TerrainRenderable(width, depth, xsubs, zsubs);
                case "TestRenderable":
                    return new TestRenderable();
                default:
                    return null;
            }
        }
    }
}
