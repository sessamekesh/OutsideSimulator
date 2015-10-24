using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutsideSimulator.Effects
{
    public class EffectBuildException : Exception
    {
        public EffectBuildException(string message) : base(message)
        { }
    }
}
