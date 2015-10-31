using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutsideSimulator.Renderable
{
    public class SharpRockDecorator : RockRenderable
    {
        protected RockRenderable BaseRenderable;

        public SharpRockDecorator(RockRenderable baseRenderable) : base()
        {
            BaseRenderable = baseRenderable;
        }

        public override string GetAssetPath()
        {
            return "../../assets/Rocks/rock_5.obj";
        }
    }
}
