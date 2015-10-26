using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SlimDX;

namespace OutsideSimulator.D3DCore
{
    /// <summary>
    /// Geometry generation helper methods
    /// </summary>
    public static class GeometryGenerator
    {
        /// <summary>
        /// Fill in a grid - used in terrain generation (for now) and anything that requires planar data
        /// </summary>
        /// <param name="width">Width in world units of grid</param>
        /// <param name="depth">Depth in world units of grid</param>
        /// <param name="m">Subdivisions in the x direction</param>
        /// <param name="n">Subdivisions in the z direction</param>
        /// <param name="verts">Output array of vertex positions</param>
        /// <param name="indices">Output array of index positions</param>
        public static void CreateGrid(float width, float depth, uint m, uint n, out Effects.BasicEffect.BasicEffectVertex[] verts, out uint[] indices)
        {
            var vertList = new List<Effects.BasicEffect.BasicEffectVertex>();
            var indexList = new List<uint>();

            var halfWidth = width * 0.5f;
            var halfDepth = depth * 0.5f;

            var dx = width / (n - 1);
            var dz = depth / (m - 1);

            var du = 1.0f / (n - 1);
            var dv = 1.0f / (m - 1);

            for (var i = 0; i < m; ++i)
            {
                var z = halfDepth - i * dz;
                for (var j = 0; j < n; ++j)
                {
                    var x = -halfWidth + j * dx;
                    //vertList.Add(new Vector3(x, 0.0f, z));
                    vertList.Add(new Effects.BasicEffect.BasicEffectVertex()
                    {
                        Pos = new Vector3(x, 0.0f, z),
                        TexCoord = new Vector2(j * du, i * dv)
                    });
                }
            }

            for (var i = 0u; i < m - 1; ++i)
            {
                for (var j = 0u; j < n - 1; ++j)
                {
                    indexList.Add(i * n + j);
                    indexList.Add(i * n + j + 1);
                    indexList.Add((i + 1) * n + j);

                    indexList.Add((i + 1) * n + j);
                    indexList.Add(i * n + j + 1);
                    indexList.Add((i + 1) * n + j + 1);
                }
            }

            verts = vertList.ToArray();
            indices = indexList.ToArray();
        }
    }
}
