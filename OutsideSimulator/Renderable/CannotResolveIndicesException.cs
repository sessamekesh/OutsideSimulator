using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutsideSimulator.Renderable
{
    public class CannotResolveIndicesException : NotImplementedException
    {
        public string EffectName;
        public string RenderableName;

        public CannotResolveIndicesException(string effectName, string renderableName) : base()
        {
            EffectName = effectName;
            RenderableName = renderableName;
        }
    }
}
