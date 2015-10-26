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
        protected int XSubdivisions { get; private set; }
        protected int ZSubdivisions { get; private set; }

        protected Vector3[] Vertices;
        protected uint[] Indices;
        #endregion

        #region Ctor and IRenderable
        public TerrainRenderable(float width, float depth, uint xsubs, uint zsubs)
        {
            GeometryGenerator.CreateGrid(width, depth, xsubs, zsubs, out Vertices, out Indices);
        }

        public uint[] GetIndexList(string EffectName)
        {
            switch (EffectName)
            {
                case Effects.EffectsGlobals.TestEffectName:
                    return Indices;
                default:
                    throw new CannotResolveIndicesException(EffectName, RenderableName);
            }
        }

        public object[] GetVertexList(string EffectName)
        {
            switch (EffectName)
            {
                case Effects.EffectsGlobals.TestEffectName:
                    return Vertices.Select((vert) =>
                    {
                        return new Effects.TestEffect.TestEffectVertex()
                        {
                            Pos = vert,
                            Color = new Vector4(0.5f, 0.8f, 0.7f, 1.0f)
                        };
                    }).Cast<object>().ToArray();
                default:
                    throw new CannotResolveVerticesException(EffectName, RenderableName);
            }
        }
        #endregion

        public static readonly string RenderableName = "Terrain";
    }
}
