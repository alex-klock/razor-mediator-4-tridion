using System.Collections.Generic;
using Tridion.ContentManager;

namespace Tridion.Extensions.Mediators.Razor
{
    /// <summary>
    /// Interface that abstracts from the underlying data being a text-string or XML.
    /// </summary>
    internal interface IExtractBinariesContentWrapper
    {
        /// <summary>
        /// Get an enumerator of all found link attributes
        /// </summary>
        /// <returns>A collection of link reference wrappers.</returns>
        IEnumerable<LinkReferenceWrapper> GetLinkAttributes();

        /// <summary>
        /// Get the attribute value of the current link attribute match
        /// </summary>
        /// <param name="linkAttribute">The link attribute to get the value for.</param>
        /// <returns>The attribute's value.</returns>
        string GetAttributeValue(LinkReferenceWrapper linkAttribute);

        // 
        /// <summary>
        /// Changes the link attribute
        /// </summary>
        /// <param name="linkAttribute">The link attribute to change.</param>
        /// <param name="targetItemUri">The target tcm uri.</param>
        void ProcessLinkChange(LinkReferenceWrapper linkAttribute, TcmUri targetItemUri);

        /// <summary>
        /// Gets the underlying object (either a string or an XmlDocument)
        /// </summary>
        object Content
        {
            get;
        }
    }
}
