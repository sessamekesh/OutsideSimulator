using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutsideSimulator.Renderable
{
    public class MetalWoodTextureDecorator : TestRenderable
    {
        #region Decorator Pattern
        protected TestRenderable Base;
        #endregion

        public MetalWoodTextureDecorator(TestRenderable BaseRenderable) : base()
        {
            Base = BaseRenderable;
        }

        public override string GetTexturePath()
        {
            return "../../assets/Crates/MetalWood.dds";
        }
    }
}
