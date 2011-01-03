using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public interface IFort
    {
        /// <summary>
        /// You show parade at fort and you get 3 points for it.
        /// </summary>
        /// <returns>If parade was succesful (if you had sources for paying it).</returns>
        bool ShowParade();

        /// <summary>
        /// Destroy target hexa with hexaID. 
        /// This hexa have to neighbourgh of fort hexa or fort hexa itself.
        /// </summary>
        /// <param name="hexaID">ID of hexa which should be destroyed.</param>
        /// <returns>True if hexa was destroyed, otherwise false.</returns>
        bool DestroyHexa(int hexaID);

        /// <summary>
        /// Capture target hexa with hexaID. 
        /// This hexa have to neighbourgh of fort hexa or fort hexa itself. 
        /// </summary>
        /// <param name="hexaID">ID of hexa which should be captured.</param>
        /// <returns>True if hexa was captured, otherwise false.</returns>
        bool CaptureHexa(int hexaID);

        /// <summary>
        /// Destroy sources of one player.
        /// </summary>
        /// <param name="playerID">Name of player you want destroy sources.</param>
        /// <returns>True if destroying sources was succesful, otherwise false.</returns>
        bool DestroySources(String playerName);
    }
}
