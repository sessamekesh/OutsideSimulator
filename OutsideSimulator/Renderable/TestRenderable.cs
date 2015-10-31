using System;
using System.Linq;

using OutsideSimulator.Effects.TestEffect;
using OutsideSimulator.Effects.BasicEffect;

namespace OutsideSimulator.Renderable
{
    /// <summary>
    /// For testing purposes only. Remove it ASAP.
    /// </summary>
    public class TestRenderable : IRenderable
    {
        public static readonly float size = 2.2f;

        public virtual uint[] GetIndexList(string EffectName)
        {
            switch (EffectName)
            {
                case Effects.EffectsGlobals.TestEffectName:
                case Effects.EffectsGlobals.BasicEffectName:
                    return new uint[]
                    {
                        // front
                        0,1,2,
                        0,2,3,
                        // back
                        4,6,5,
                        4,7,6,
                        // left
                        4,5,1,
                        4,1,0,
                        // right
                        3,2,6,
                        3,6,7,
                        //top
                        1,5,6,
                        1,6,2,
                        // bottom
                        4,0,3,
                        4,3,7
                    };
                default:
                    throw new CannotResolveIndicesException(EffectName, RenderableName);
            }
        }

        public virtual object[] GetVertexList(string EffectName)
        {
            switch (EffectName)
            {
                case Effects.EffectsGlobals.BasicEffectName:
                    return new[]
                    {
                        new BasicEffectVertex { Pos = new SlimDX.Vector3(-size, -size, -size), TexCoord = new SlimDX.Vector2(0.0f, 1.0f) },
                        new BasicEffectVertex { Pos = new SlimDX.Vector3(-size, size, -size), TexCoord = new SlimDX.Vector2(0.0f, 0.0f) },
                        new BasicEffectVertex { Pos = new SlimDX.Vector3(size, size, -size), TexCoord = new SlimDX.Vector2(1.0f, 0.0f) },
                        new BasicEffectVertex { Pos = new SlimDX.Vector3(size, -size, -size), TexCoord = new SlimDX.Vector2(1.0f, 1.0f) },
                        new BasicEffectVertex { Pos = new SlimDX.Vector3(-size, -size, size), TexCoord = new SlimDX.Vector2(1.0f, 1.0f) },
                        new BasicEffectVertex { Pos = new SlimDX.Vector3(-size, size, size), TexCoord = new SlimDX.Vector2(1.0f, 0.0f) },
                        new BasicEffectVertex { Pos = new SlimDX.Vector3(size, size, size), TexCoord = new SlimDX.Vector2(0.0f, 0.0f) },
                        new BasicEffectVertex { Pos = new SlimDX.Vector3(size, -size, size), TexCoord = new SlimDX.Vector2(0.0f, 1.0f) }

                    }.Cast<object>().ToArray();
                case Effects.EffectsGlobals.TestEffectName:
                    return new[]
                    {
                        new TestEffectVertex { Pos = new SlimDX.Vector3(-1.0f, -1.0f, -1.0f), Color = new SlimDX.Vector4(1.0f, 1.0f, 1.0f, 1.0f) },
                        new TestEffectVertex { Pos = new SlimDX.Vector3(-1.0f, 1.0f, -1.0f), Color = new SlimDX.Vector4(0.0f, 0.0f, 0.0f, 1.0f) },
                        new TestEffectVertex { Pos = new SlimDX.Vector3(1.0f, 1.0f, -1.0f), Color = new SlimDX.Vector4(1.0f, 0.0f, 0.0f, 1.0f) },
                        new TestEffectVertex { Pos = new SlimDX.Vector3(1.0f, -1.0f, -1.0f), Color = new SlimDX.Vector4(0.0f, 1.0f, 0.0f, 1.0f) },
                        new TestEffectVertex { Pos = new SlimDX.Vector3(-1.0f, -1.0f, 1.0f), Color = new SlimDX.Vector4(0.0f, 0.0f, 1.0f, 1.0f) },
                        new TestEffectVertex { Pos = new SlimDX.Vector3(-1.0f, 1.0f, 1.0f), Color = new SlimDX.Vector4(1.0f, 1.0f, 0.0f, 1.0f) },
                        new TestEffectVertex { Pos = new SlimDX.Vector3(1.0f, 1.0f, 1.0f), Color = new SlimDX.Vector4(1.0f, 0.0f, 1.0f, 1.0f) },
                        new TestEffectVertex { Pos = new SlimDX.Vector3(1.0f, -1.0f, 1.0f), Color = new SlimDX.Vector4(0.0f, 1.0f, 1.0f, 1.0f) }
                    }.Cast<object>().ToArray();
                default:
                    throw new CannotResolveVerticesException(EffectName, RenderableName);
            }
        }

        public virtual string GetTexturePath()
        {
            return "../../assets/Crates/Wood1.dds";
        }

        private readonly static string RenderableName = "TestRenderable";
    }
}
