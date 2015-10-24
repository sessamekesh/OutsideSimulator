using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutsideSimulator.Renderable
{
    public class CannotResolveVerticesException : NotImplementedException
    {
        public string EffectName;
        public string RenderableName;

        public CannotResolveVerticesException(string EffectName, string RenderableName) : base()
        {
            this.EffectName = EffectName;
            this.RenderableName = RenderableName;
        }
    }
}
