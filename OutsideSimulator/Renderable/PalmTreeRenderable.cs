using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutsideSimulator.Renderable
{
    public class PalmTreeRenderable : TreeRenderable
    {
        protected TreeRenderable Base;

        public PalmTreeRenderable(TreeRenderable baseElement) : base()
        {
            Base = baseElement;
        }

        public override string RenderableName()
        {
            return "PalmTreeRenderable";
        }

        public override string GetTexturePath()
        {
            return "../../assets/SimpleTree/Combined2.png";
        }
    }
}
