using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SlimDX;

using OutsideSimulator.D3DCore;

namespace OutsideSimulator.Renderable
{
    /// <summary>
    /// Generate information for the terrain of the outside place
    /// </summary>
    public class TerrainRenderable : IRenderable
    {
        #region Properties
        public float Width { get; private set; }
        public float Depth { get; private set; }
        public uint XSubdivisions { get; private set; }
        public uint ZSubdivisions { get; private set; }

        protected Effects.BasicEffect.BasicEffectVertex[] Vertices;
        protected uint[] Indices;
        #endregion

        #region Ctor and IRenderable
        public TerrainRenderable()
        {
            Width = 100.0f;
            Depth = 100.0f;
            XSubdivisions = 5;
            ZSubdivisions = 5;
            GeometryGenerator.CreateGrid(Width, Depth, XSubdivisions, ZSubdivisions, out Vertices, out Indices);
        }

        public TerrainRenderable(float width, float depth, uint xsubs, uint zsubs)
        {
            Width = width;
            Depth = depth;
            XSubdivisions = xsubs;
            ZSubdivisions = zsubs;
            GeometryGenerator.CreateGrid(width, depth, xsubs, zsubs, out Vertices, out Indices);
        }

        public virtual uint[] GetIndexList(string EffectName)
        {
            switch (EffectName)
            {
                case Effects.EffectsGlobals.TestEffectName:
                case Effects.EffectsGlobals.BasicEffectName:
                    return Indices;
                default:
                    throw new CannotResolveIndicesException(EffectName, RenderableName);
            }
        }

        public virtual object[] GetVertexList(string EffectName)
        {
            switch (EffectName)
            {
                case Effects.EffectsGlobals.TestEffectName:
                    return Vertices.Select((vert) =>
                    {
                        return new Effects.TestEffect.TestEffectVertex()
                        {
                            Pos = vert.Pos,
                            Color = new Vector4(0.5f, 0.8f, 0.7f, 1.0f)
                        };
                    }).Cast<object>().ToArray();
                case Effects.EffectsGlobals.BasicEffectName:
                    return Vertices.Select((vert) =>
                    {
                        return new Effects.BasicEffect.BasicEffectVertex()
                        {
                            Pos = vert.Pos,
                            TexCoord = vert.TexCoord
                        };
                    }).Cast<object>().ToArray();
                default:
                    throw new CannotResolveVerticesException(EffectName, RenderableName);
            }
        }

        public virtual string GetTexturePath()
        {
            return "../../assets/Terrains/soil.dds";
        }
        #endregion

        public static readonly string RenderableName = "Terrain";
    }
}
