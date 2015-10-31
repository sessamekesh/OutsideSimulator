using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutsideSimulator.Renderable
{
    public class TableRenderable : FromOBJRenderable
    {
        public override string GetAssetPath()
        {
            return "../../assets/Bench/tscn_table.obj";
        }

        public override string GetTexturePath()
        {
            return "../../assets/Bench/tablemap01.jpg";
        }

        public override string RenderableName()
        {
            return "TableRenderable";
        }
    }
}
