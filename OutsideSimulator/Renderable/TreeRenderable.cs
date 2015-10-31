using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutsideSimulator.Renderable
{
    public class TreeRenderable : FromOBJRenderable
    {
        public override string GetAssetPath()
        {
            return "../../assets/SimpleTree/SimpleTree.obj";
        }

        public override string GetTexturePath()
        {
            return "../../assets/SimpleTree/Combined.png";
        }

        public override string RenderableName()
        {
            return "SimpleTree";
        }
    }
}
