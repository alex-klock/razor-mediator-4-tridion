using System.Text.RegularExpressions;
using System.Xml;

namespace Tridion.Extensions.Mediators.Razor
{
    /// <summary>
    /// Structure representing either the XML or text related context of a found link attribute
    /// </summary>
    internal class LinkReferenceWrapper
    {
        /// <summary>
        /// The attribute's name.
        /// </summary>
        internal string _attributeName;

        /// <summary>
        /// Fields for regex match
        /// </summary>
        internal Match _regexMatch;

        /// <summary>
        /// The link position Regex group.
        /// </summary>
        internal Group _linkPosition;

        /// <summary>
        /// The output path attribute value.
        /// </summary>
        internal string _outputPathAttributeValue;

        /// <summary>
        /// Fields for element match
        /// </summary>
        internal XmlNode _element;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="match">The regex match.</param>
        internal LinkReferenceWrapper(Match match)
        {
            _regexMatch = match;
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attrName"></param>
        internal LinkReferenceWrapper(XmlNode element, string attrName)
        {
            _element = element;
            _attributeName = attrName;
        }
    }
}
