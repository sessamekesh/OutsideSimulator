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

        public static XElement SerializeVector(SlimDX.Vector3 Vector)
        {
            return new XElement("Vector3",
                new XElement("X", Vector.X),
                new XElement("Y", Vector.Y),
                new XElement("Z", Vector.Z)
            );
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

        public static XElement SerializeQuaternion(SlimDX.Quaternion Quat)
        {
            return new XElement("Quaternion",
                new XElement("X", Quat.X),
                new XElement("Y", Quat.Y),
                new XElement("Z", Quat.Z),
                new XElement("W", Quat.W)
            );
        }
        public static SlimDX.Quaternion DeserializeQuaternion(string XMLString)
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

        public static XElement SerializeMatrix(SlimDX.Matrix Matrix)
        {
            return new XElement("Matrix",
                new XElement("M11", Matrix.M11),
                new XElement("M12", Matrix.M12),
                new XElement("M13", Matrix.M13),
                new XElement("M14", Matrix.M14),

                new XElement("M21", Matrix.M21),
                new XElement("M22", Matrix.M22),
                new XElement("M23", Matrix.M23),
                new XElement("M24", Matrix.M24),

                new XElement("M31", Matrix.M31),
                new XElement("M32", Matrix.M32),
                new XElement("M33", Matrix.M33),
                new XElement("M34", Matrix.M34),

                new XElement("M41", Matrix.M41),
                new XElement("M42", Matrix.M42),
                new XElement("M43", Matrix.M43),
                new XElement("M44", Matrix.M44)
            );
        }
        public static SlimDX.Matrix? DeserializeMatrix(string XMLString)
        {
            var xe = XElement.Parse(XMLString);

            if (xe.Name != "Matrix")
            {
                return null;
            }
            else
            {
                SlimDX.Matrix mtr = new SlimDX.Matrix();

                mtr.M11 = float.Parse((xe.Nodes().First((x) => (x as XElement).Name == "M11") as XElement).Value);
                mtr.M12 = float.Parse((xe.Nodes().First((x) => (x as XElement).Name == "M12") as XElement).Value);
                mtr.M13 = float.Parse((xe.Nodes().First((x) => (x as XElement).Name == "M13") as XElement).Value);
                mtr.M14 = float.Parse((xe.Nodes().First((x) => (x as XElement).Name == "M14") as XElement).Value);

                mtr.M21 = float.Parse((xe.Nodes().First((x) => (x as XElement).Name == "M21") as XElement).Value);
                mtr.M22 = float.Parse((xe.Nodes().First((x) => (x as XElement).Name == "M22") as XElement).Value);
                mtr.M23 = float.Parse((xe.Nodes().First((x) => (x as XElement).Name == "M23") as XElement).Value);
                mtr.M24 = float.Parse((xe.Nodes().First((x) => (x as XElement).Name == "M24") as XElement).Value);

                mtr.M31 = float.Parse((xe.Nodes().First((x) => (x as XElement).Name == "M31") as XElement).Value);
                mtr.M32 = float.Parse((xe.Nodes().First((x) => (x as XElement).Name == "M32") as XElement).Value);
                mtr.M33 = float.Parse((xe.Nodes().First((x) => (x as XElement).Name == "M33") as XElement).Value);
                mtr.M34 = float.Parse((xe.Nodes().First((x) => (x as XElement).Name == "M34") as XElement).Value);

                mtr.M41 = float.Parse((xe.Nodes().First((x) => (x as XElement).Name == "M41") as XElement).Value);
                mtr.M42 = float.Parse((xe.Nodes().First((x) => (x as XElement).Name == "M42") as XElement).Value);
                mtr.M43 = float.Parse((xe.Nodes().First((x) => (x as XElement).Name == "M43") as XElement).Value);
                mtr.M44 = float.Parse((xe.Nodes().First((x) => (x as XElement).Name == "M44") as XElement).Value);

                return mtr;
            }
        }
    }
}
