using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutsideSimulator.D3DCore
{
    public static partial class MathF
    {
        public static readonly float Sqrt2 = (float)Math.Sqrt(2);

        public const float PI = (float)Math.PI;

        public static float ToRadians(float degrees)
        {
            return PI * degrees / 180.0f;
        }

        public static float Clamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(value, max));
        }
    }
}
