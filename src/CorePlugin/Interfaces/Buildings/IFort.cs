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

    public enum DestroyHexaError
    {
        /// <summary>
        /// There is no error.
        /// </summary>
        OK,

        /// <summary>
        /// You have not enough sources.
        /// </summary>
        NoSources,

        /// <summary>
        /// There is not your fort in neighbourghood of target hexa.
        /// </summary>
        TooFarFromFort,

        /// <summary>
        /// Target hexa doesnt exist.
        /// </summary>
        InvalidHexaID
    }

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

    public enum CaptureHexaError {
        /// <summary>
        /// There is no error.
        /// </summary>
        OK, 

        /// <summary>
        /// You have not enough sources.
        /// </summary>
        NoSources, 
        
        /// <summary>
        /// There is not your fort in neighbourghood of target hexa.
        /// </summary>
        TooFarFromFort, 
        
        /// <summary>
        /// Target hexa doesnt exist.
        /// </summary>
        InvalidHexaID }

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
        /// Find out if you can destroy hexa with hexaID.
        /// </summary>
        /// <param name="hexaID">Target hexa ID</param>
        /// <returns>Error why you cant destroy hexa or DestroyHexaError.OK</returns>
        DestroyHexaError CanDestroyHexa(int hexaID);

        /// <summary>
        /// Capture target hexa with hexaID. 
        /// This hexa have to neighbourgh of fort hexa or fort hexa itself. 
        /// </summary>
        /// <param name="hexaID">ID of hexa which should be captured.</param>
        /// <returns>True if hexa was captured, otherwise false.</returns>
        bool CaptureHexa(int hexaID);

        /// <summary>
        /// Find out if you can capture hexa with hexaID.
        /// </summary>
        /// <param name="hexaID">Target hexa ID</param>
        /// <returns>Error why you cant capture hexa or CaptureHexaError.OK</returns>
        CaptureHexaError CanCaptureHexa(int hexaID);

        /// <summary>
        /// Steal sources of one player.
        /// </summary>
        /// <param name="playerID">Name of player you want to steal sources.</param>
        /// <returns>True if stealing sources was succesful, otherwise false.</returns>
        bool StealSources(String playerName);

        /// <summary>
        /// Find out if you can steal other player sources.
        /// </summary>
        /// <param name="playerName">Name of player you want to steal sources.</param>
        /// <returns>Error why you cant steal sources or DestroySourcesError.OK</returns>
        DestroySourcesError CanStealSources(String playerName);
    }
}
