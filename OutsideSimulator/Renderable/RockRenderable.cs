﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SlimDX;

using FileFormatWavefront;
using FileFormatWavefront.Model;

namespace OutsideSimulator.Renderable
{
    public class RockRenderable : FromOBJRenderable
    {
        public override string GetAssetPath()
        {
            return "../../assets/Rocks/rock_4.obj";
        }

        public override string GetTexturePath()
        {
            return "../../assets/Rocks/rock_4_col.png";
        }

        public override string RenderableName()
        {
            return "RockRenderable";
        }
    }
}
