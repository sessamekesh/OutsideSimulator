using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutsideSimulator.Renderable
{
    public class BenchRenderable : FromOBJRenderable
    {
        public override string GetAssetPath()
        {
            return "../../assets/Bench/tscn_bench.obj";
        }

        public override string GetTexturePath()
        {
            return "../../assets/Bench/benchmap01.jpg";
        }

        public override string RenderableName()
        {
            return "BenchRenderable";
        }
    }
}
