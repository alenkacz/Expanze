﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public interface IComponentAI
    {
        String GetAIName();
        void InitAIComponent(IMapController mapController, int[][] koef);
        void ResolveAI();
        IComponentAI Clone();
    }
}
