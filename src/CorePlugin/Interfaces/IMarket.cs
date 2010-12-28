using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
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
    }
}
