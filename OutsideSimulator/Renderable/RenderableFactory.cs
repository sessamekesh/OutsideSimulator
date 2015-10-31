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
            else if (toSave is MetalWoodTextureDecorator)
            {
                MetalWoodTextureDecorator tr = toSave as MetalWoodTextureDecorator;
                XElement ts = new XElement("MetalWoodTextureCrate");
                return ts;
            }
            else if (toSave is TestRenderable)
            {
                TestRenderable tr = toSave as TestRenderable;
                XElement ts = new XElement("TestRenderable");
                return ts;
            }
            else if (toSave is SharpRockDecorator)
            {
                SharpRockDecorator tr = toSave as SharpRockDecorator;
                XElement ts = new XElement("SharpRockRenderable");
                return ts;
            }
            else if (toSave is RockRenderable)
            {
                return new XElement("RockRenderable");
            }
            else
            {
                throw new InvalidOperationException("Cannot serialize - did not recognize renderable type");
            }
        }

        /// <summary>
        /// JAVA, YOU DON'T HAVE A MONOPOLY ON LONG VARIABLE NAMES
        /// </summary>
        /// <param name="oldRenderable">The existing renderable</param>
        /// <returns></returns>
        public static IRenderable GetNextRenderableInModificationChain(IRenderable oldRenderable)
        {
            // Cycle to next decorator in textures
            if (oldRenderable is TestRenderable)
            {
                if (oldRenderable is MetalWoodTextureDecorator)
                {
                    return new TestRenderable();
                }
                else
                {
                    return new MetalWoodTextureDecorator(oldRenderable as TestRenderable);
                }
            }
            else if (oldRenderable is TreeRenderable)
            {
                if (oldRenderable is PalmTreeRenderable)
                {
                    return new TreeRenderable();
                }
                else
                {
                    return new PalmTreeRenderable(oldRenderable as TreeRenderable);
                }
            }
            else if (oldRenderable is TerrainRenderable)
            {
                if (oldRenderable is SandTextureTerrain)
                {
                    return new TerrainRenderable();
                }
                else
                {
                    return new SandTextureTerrain(new TerrainRenderable());
                }
            }
            else if (oldRenderable is RockRenderable)
            {
                if (oldRenderable is SharpRockDecorator)
                {
                    return new RockRenderable();
                }
                else
                {
                    return new SharpRockDecorator(oldRenderable as RockRenderable);
                }
            }
            else
            {
                return null;
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
                case "MetalWoodTextureCrate":
                    return new MetalWoodTextureDecorator(new TestRenderable());
                case "RockRenderable":
                    return new RockRenderable();
                case "SharpRockRenderable":
                    return new SharpRockDecorator(new RockRenderable());
                default:
                    return null;
            }
        }
    }
}
