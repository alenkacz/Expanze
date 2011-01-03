using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public enum MarketError { 
        /// <summary>
        /// There is no problem with buying licence.
        /// </summary>
        OK, 
        
        /// <summary>
        /// In each market you can bought only 3 licences.
        /// This mean that you have bought all of them and you need
        /// new Market.
        /// </summary>
        MaxLicences, 
        
        /// <summary>
        /// Licences arent free, you have not enough sources for buying it.
        /// </summary>
        NoSources, 
        
        /// <summary>
        /// You are trying to buy second licence for the second time.
        /// You already have second licence.
        /// </summary>
        HaveSecondLicence };

    public interface IMarket
    {
        /// <summary>
        /// Buy licence for better conversion rate for source kind. 
        /// If you have first licence, you buy second licence. 
        /// Third licence doesn't exist.
        /// </summary>
        /// <param name="source">Kind of source.</param>
        /// <returns>True if licence was bought, otherwise false.</returns>
        bool BuyLicence(SourceKind source);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        MarketError CanBuyLicence(SourceKind source);
    }
}
