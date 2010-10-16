using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanze
{
    static class Settings
    {
        public enum Types { Cornfield, Forest, Stone, Pasture, Mountains, Desert, Null };

        //indexed by enum Types
        public static String[] mapPaths = new String[] { "/Models/brownhex", "/Models/greenhex", "/Models/brownhex", "/Models/greennhex", "/Models/brownhex", "/Models/brownhex" };
    }
}
