using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SlimDX;
using FileFormatWavefront;

namespace OutsideSimulator.Renderable
{
    public abstract class FromOBJRenderable : IRenderable
    {
        protected Vector3[] verts;
        protected Vector2[] uvCoords;
        protected uint[] indices;

        public abstract string GetAssetPath();

        public FromOBJRenderable()
        {
            var obj = FileFormatObj.Load(GetAssetPath(), true);

            List<Vector3> vertList = new List<Vector3>();
            List<Vector2> uvList = new List<Vector2>();
            List<uint> indexList = new List<uint>();

            foreach (var face in obj.Model.UngroupedFaces)
            {
                foreach (var index in face.Indices)
                {
                    var vert = obj.Model.Vertices[index.vertex];
                    var uv = obj.Model.Uvs[index.uv.Value];

                    vertList.Add(
                        new Vector3(vert.x, vert.y, vert.z)
                    );

                    uvList.Add(
                        new Vector2(uv.u, uv.v)
                    );

                    indexList.Add((uint)indexList.Count);
                }
            }

            verts = vertList.ToArray();
            uvCoords = uvList.ToArray();
            indices = indexList.ToArray();
        }

        #region IRenderable
        public virtual uint[] GetIndexList(string EffectName)
        {
            return indices;
        }

        public abstract string GetTexturePath();

        public virtual object[] GetVertexList(string EffectName)
        {
            if (EffectName == Effects.EffectsGlobals.BasicEffectName)
            {
                List<object> bev = new List<object>();

                for (int i = 0; i < verts.Length; i++)
                {
                    bev.Add(new Effects.BasicEffect.BasicEffectVertex()
                    {
                        Pos = verts[i],
                        TexCoord = uvCoords[i]
                    });
                }

                return bev.ToArray();
            }
            else
            {
                throw new CannotResolveVerticesException(EffectName, RenderableName());
            }
        }

        public abstract string RenderableName();
        #endregion
    }
}
