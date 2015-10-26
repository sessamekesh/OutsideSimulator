using System.Runtime.InteropServices;

using SlimDX;

namespace OutsideSimulator.Effects.MenuEffect
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MenuEffectVertex
    {
        public Vector2 ScreenSpacePos;
        public Vector2 Texcoord;

        public static readonly int Stride = Marshal.SizeOf(typeof(MenuEffectVertex));
    }
}
