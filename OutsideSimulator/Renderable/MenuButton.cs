using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SlimDX;

using OutsideSimulator.Commands;

namespace OutsideSimulator.Renderable
{
    public class MenuButton : IRenderable
    {
        #region Properties
        public string TextureLocation { get; private set; }
        public bool IsMouseOver { get; set; }
        public bool IsMouseDown { get; set; }

        public Vector2 UpperLeft { get; private set; }
        public Vector2 LowerRight { get; private set; }
        #endregion

        public MenuButton(string TextureLocation, Vector2 upperLeft, Vector2 lowerRight)
        {
            this.TextureLocation = TextureLocation;
            IsMouseDown = false;
            IsMouseDown = false;
            UpperLeft = upperLeft;
            LowerRight = lowerRight;
        }

        #region IRenderable
        public uint[] GetIndexList(string EffectName)
        {
            switch (EffectName)
            {
                case Effects.EffectsGlobals.MenuEffectName:
                    return new uint[]
                    {
                        0, 1, 2,
                        2, 3, 0
                    };
                default: throw new CannotResolveIndicesException(EffectName, RenderableName);
            }
        }

        public object[] GetVertexList(string EffectName)
        {
            switch (EffectName)
            {
                case Effects.EffectsGlobals.MenuEffectName:
                    return new object[]
                    {
                        new Effects.MenuEffect.MenuEffectVertex() { ScreenSpacePos = new Vector2(UpperLeft.X, UpperLeft.Y), Texcoord = new Vector2(0.0f, 0.0f) },
                        new Effects.MenuEffect.MenuEffectVertex() { ScreenSpacePos = new Vector2(LowerRight.X, UpperLeft.Y), Texcoord = new Vector2(1.0f, 0.0f) },
                        new Effects.MenuEffect.MenuEffectVertex() { ScreenSpacePos = new Vector2(LowerRight.X, LowerRight.Y), Texcoord = new Vector2(1.0f, 1.0f) },
                        new Effects.MenuEffect.MenuEffectVertex() { ScreenSpacePos = new Vector2(UpperLeft.X, LowerRight.Y), Texcoord = new Vector2(0.0f, 1.0f) }
                    };
                default: throw new CannotResolveVerticesException(EffectName, RenderableName);
            }
        }

        public string GetTexturePath()
        {
            return TextureLocation;
        }
        #endregion

        public static readonly string RenderableName = "MenuButton";
    }
}
