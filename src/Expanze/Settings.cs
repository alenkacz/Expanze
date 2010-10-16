using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanze
{
    static class Settings
    {
        public enum Types { Cornfield, Forest, Stone, Pasture, Mountains, Desert, Water };

        //indexed by enum Types
        public static String[] mapPaths = new String[] { "Models/yellowhex", "Models/brownhex", "Models/greyhex", "Models/greenhex", "Models/redhex", "Models/orangehex", "Models/bluehex" };
    }
}
