﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutsideSimulator.Commands.Events
{
    public interface TimerTickSubscriber
    {
        void Update(float dt);
    }
}
