using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Expanze
{
    class PickVariables
    {
        public Boolean wasClickAnywhereNow = false;
        public Boolean wasClickAnywhereLast = false;
        public Boolean pickActive = false;         // if mouse is over pickable area
        public Boolean pickNewActive = false;      // if mouse is newly over pickable area
        public Boolean pickNewPress = false;       // if mouse press on pickable area newly
        public Boolean pickPress = false;          // if mouse press on pickable area
        public Boolean pickNewRelease = false;     // if mouse is newly release above pickable area

        public Color pickColor; // dont have to be set everywhere

        public PickVariables(Color color)
        {
            pickColor = color;
        }
    }
}
