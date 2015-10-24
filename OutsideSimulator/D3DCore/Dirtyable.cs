using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutsideSimulator.D3DCore
{
    /// <summary>
    /// GENIUS, right?
    /// Represents a cached value, essentially. If nothing has been done to dirty an object,
    ///  the cached value is used. If something which would change the value has happened,
    ///  notify the object via the "DoTheNasty" method. If that method has been called since
    ///  the last time the object's value was used, the value is re-computed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Dirtyable<T>
    {
        private T _value;

        public T Value
        {
            get
            {
                if (Dirty)
                {
                    _value = Compute();
                    Dirty = false;
                }

                return _value;
            }
        }

        /// <summary>
        /// Initialize the dirtyable
        /// </summary>
        /// <param name="RecomputeFunction">Function which will re-create the Dirtyable, if dirty</param>
        /// <param name="InitialValue">The initial value to give to the Dirtyable</param>
        public Dirtyable(Func<T> RecomputeFunction, T InitialValue)
        {
            Dirty = false;
            _value = InitialValue;
            Compute = RecomputeFunction;
        }

        /// <summary>
        /// Create a dirtyable without an initial value (will be computed when needed)
        /// </summary>
        /// <param name="RecomputeFunction">Function which will re-create the Dirtyable, if dirty</param>
        public Dirtyable(Func<T> RecomputeFunction)
        {
            Dirty = true;
            Compute = RecomputeFunction;
        }

        public bool Dirty { get; private set; }
        public Func<T> Compute;

        public void Clean()
        {
            Dirty = false;
        }

        /// <summary>
        /// I had to call it this. I had no choice! I've been working on this project all day!
        /// </summary>
        public void DoTheNasty()
        {
            Dirty = true;
        }

        public static implicit operator T(Dirtyable<T> dirtyable)
        {
            return dirtyable.Value;
        }
    }
}
