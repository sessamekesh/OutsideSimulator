using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutsideSimulator.Renderable
{
    /// <summary>
    /// Renderable object, which behaves as a vertex factory
    /// </summary>
    public interface IRenderable
    {
        object[] GetVertexList(string EffectName);
        uint[] GetIndexList(string EffectName);
    }
}
