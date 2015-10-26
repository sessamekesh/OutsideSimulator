using System.Runtime.InteropServices;

using SlimDX;

namespace OutsideSimulator.Effects.BasicEffect
{
    [StructLayout(LayoutKind.Sequential)]
    public class BasicEffectVertex
    {
        public Vector3 Pos;
        public Vector2 TexCoord;

        public static readonly int Stride = Marshal.SizeOf(typeof(BasicEffectVertex));
    }
}