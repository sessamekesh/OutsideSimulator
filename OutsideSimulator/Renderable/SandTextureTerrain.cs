using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutsideSimulator.Renderable
{
    public class SandTextureTerrain : TerrainRenderable
    {
        protected TerrainRenderable Base;

        public SandTextureTerrain(TerrainRenderable baseRenderable) : base()
        {
            Base = baseRenderable;
        }

        public override uint[] GetIndexList(string EffectName)
        {
            return Base.GetIndexList(EffectName);
        }

        public override object[] GetVertexList(string EffectName)
        {
            return Base.GetVertexList(EffectName);
        }

        public override string GetTexturePath()
        {
            return "../../assets/Terrains/Sand.dds";
        }
    }
}
