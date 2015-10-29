using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using System.Threading.Tasks;

namespace OutsideSimulator.D3DCore
{
    public class Util
    {
        public static void ReleaseCom<T>(ref T x) where T : class, IDisposable
        {
            if (x != null)
            {
                x.Dispose();
                x = null;
            }
        }

        public static string SerializeVector(SlimDX.Vector3 Vector)
        {
            return new XElement("Vector3",
                new XElement("X", Vector.X),
                new XElement("Y", Vector.Y),
                new XElement("Z", Vector.Z)
            ).ToString();
        }
        public static SlimDX.Vector3 DeserializeVector(string XMLString)
        {
            XElement xe = XElement.Parse(XMLString);

            if (xe.Name != "Vector3") throw new FormatException("Invalid element " + xe.Name);
            else
            {
                return new SlimDX.Vector3(
                    float.Parse((xe.Nodes().First((x) => (x as XElement).Name == "X") as XElement).Value),
                    float.Parse((xe.Nodes().First((x) => (x as XElement).Name == "Y") as XElement).Value),
                    float.Parse((xe.Nodes().First((x) => (x as XElement).Name == "Z") as XElement).Value)
                );
            }
        }

        public static string SerializeQuaternion(SlimDX.Quaternion Quat)
        {
            return new XElement("Quaternion",
                new XElement("X", Quat.X),
                new XElement("Y", Quat.Y),
                new XElement("Z", Quat.Z),
                new XElement("W", Quat.W)
            ).ToString();
        }
        public SlimDX.Quaternion DeserializeQuaternion(string XMLString)
        {
            XElement xe = XElement.Parse(XMLString);
            if (xe.Name != "Quaternion") throw new FormatException("Invalid element " + xe.Name);
            else
            {
                return new SlimDX.Quaternion(
                    float.Parse((xe.Nodes().First((x) => (x as XElement).Name == "X") as XElement).Value),
                    float.Parse((xe.Nodes().First((x) => (x as XElement).Name == "Y") as XElement).Value),
                    float.Parse((xe.Nodes().First((x) => (x as XElement).Name == "Z") as XElement).Value),
                    float.Parse((xe.Nodes().First((x) => (x as XElement).Name == "W") as XElement).Value)
                );
            }
        }
    }
}
