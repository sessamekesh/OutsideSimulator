using System;
using System.Linq;

using OutsideSimulator.Effects.TestEffect;

namespace OutsideSimulator.Renderable
{
    /// <summary>
    /// For testing purposes only. Remove it ASAP.
    /// </summary>
    public class TestRenderable : IRenderable
    {
        public uint[] GetIndexList(string EffectName)
        {
            switch (EffectName)
            {
                case Effects.EffectsGlobals.TestEffectName:
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

        public object[] GetVertexList(string EffectName)
        {
            if (EffectName == Effects.EffectsGlobals.TestEffectName)
            {
                // TODO KAM: I don't think the .Cast<object>().ToArray() is as efficient as it could be?
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
            }
            else
            {
                throw new CannotResolveVerticesException(EffectName, RenderableName);
            }
        }

        private readonly static string RenderableName = "TestRenderable";
    }
}
