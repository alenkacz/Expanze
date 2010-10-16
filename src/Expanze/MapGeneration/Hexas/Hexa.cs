using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanze
{
    /// <summary>
    /// Containing information about one single hexa
    /// </summary>
    class Hexa
    {
        int value;
        private Settings.Types type = Settings.Types.Water;

        public Hexa() { }

        public Hexa( int value , Settings.Types type)
        {
            this.type = type;
            this.value = value;
        }

        public string getModelPath()
        {
            return Settings.mapPaths[(int)getType()];
        }

        public Settings.Types getType()
        {
            return this.type;
        }
    }
}
