using System.Runtime.InteropServices;

using SlimDX;
using SlimDX.Direct3D11;

namespace OutsideSimulator.Effects.TestEffect
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TestEffectVertex
    {
        public Vector3 Pos;
        public Vector4 Color;

        public static readonly int Stride = Marshal.SizeOf(typeof(TestEffectVertex));
    }
}
