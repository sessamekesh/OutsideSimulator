using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutsideSimulator.Effects
{
    public class EffectRunException : NotSupportedException
    {
        public EffectRunException(string message) : base(message)
        { }
    }
}
