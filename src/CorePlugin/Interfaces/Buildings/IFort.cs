using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public enum DestroySourcesError { 
        /// <summary>
        /// There is no error.
        /// </summary>
        OK, 
        
        /// <summary>
        /// Without fort you cant destroy sources.
        /// </summary>
        NoFort, 
        
        /// <summary>
        /// Player with that name doesn't exist.
        /// </summary>
        NoPlayerWithName, 
        
        /// <summary>
        /// You have not enough sources for buying this action.
        /// </summary>
        NoSources }

    public enum DestroyHexaError { OK }
    public enum ParadeError
    {
        /// <summary>
        /// There is no error.
        /// </summary>
        OK,

        /// <summary>
        /// You have not enough sources.
        /// </summary>
        NoSources
    }

    public enum CaptureHexaError { OK, NoSources }

    public interface IFort
    {
        /// <summary>
        /// You show parade at fort and you get 3 points for it.
        /// </summary>
        /// <returns>If parade was succesful (if you had sources for paying it).</returns>
        bool ShowParade();

        /// <summary>
        /// Find out if you can show parade and get 3 points for it.
        /// </summary>
        /// <returns>Error why you cant show parade or ParadeError.OK</returns>
        ParadeError CanShowParade();

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

        CaptureHexaError CanCaptureHexa(int hexaID);

        /// <summary>
        /// Destroy sources of one player.
        /// </summary>
        /// <param name="playerID">Name of player you want destroy sources.</param>
        /// <returns>True if destroying sources was succesful, otherwise false.</returns>
        bool DestroySources(String playerName);

        /// <summary>
        /// Find out if you can destroy other player sources.
        /// </summary>
        /// <param name="playerName">Name of player you want destroy sources.</param>
        /// <returns>Error why you cant destroy sources or DestroySourcesError.OK</returns>
        DestroySourcesError CanDestroySources(String playerName);
    }
}
