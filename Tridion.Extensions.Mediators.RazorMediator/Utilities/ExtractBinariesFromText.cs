using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Tridion.ContentManager;
using Tridion.ContentManager.Templating;

namespace Tridion.Extensions.Mediators.Razor
{
    /// <summary>
    /// Implementation for ExtractBinariesContentWrapper where underlying data is text and content is processed using regular expressions.
    /// </summary>
    internal class ExtractBinariesFromText : IExtractBinariesContentWrapper
    {
        #region Constants

        // Regex group names

        /// <summary>
        /// The Match path group name used in the regular expressions.
        /// </summary>
        private const string MATCH_PATH_GROUP_NAME = "url";

        /// <summary>
        /// The Match name group name used in the regular expressions.
        /// </summary>
        private const string MATCH_NAME_GROUP_NAME = "name";

        // Regular expressions for matching links in HTML.
        // All expressions have group name 'url' for the found paths, and group name 'name' for what type of link was matched
        // sub expression for matching path used in several expressions,  ["|']value["|']

        /// <summary>
        /// The quoted value sub expression prefix regular expression string.
        /// </summary>
        private const string QUOTED_VALUE_SUB_EXPRESSION_PREFIX = @"\s*((?<url>""[^""]+"")|(?<url>'[^']+')|(?<url>[^'""\s\";

        /// <summary>
        /// The quoted value sub expression postfix regular expression string.
        /// </summary>
        private const string QUOTED_VALUE_SUB_EXPRESSION_POSTFIX = @"]+))";

        // Binary path replacement patterns (quotes are always reinserted)
        // {0} = original path value (including quotes), {1} = component ID, {2} = attribute name

        /// <summary>
        /// The attribute value replacement format string.
        /// </summary>
        private const string ATTRIBUTE_VALUE_REPLACEMENT_FORMAT = @"""{1}""";

        /// <summary>
        /// The multimedia link attribute format string.
        /// </summary>
        private const string MULTIMEDIA_LINK_ATTRIBUTE_FORMAT = @"{0} tridion:href=""{1}"" tridion:type=""Multimedia"" tridion:targetattribute=""{2}""";

        /// <summary>
        /// The multimedia css import attribute format string.
        /// </summary>
        private const string MULTIMEDIA_CSS_IMPORT_ATTRIBUTE_FORMAT = ATTRIBUTE_VALUE_REPLACEMENT_FORMAT;

        #endregion

        #region Fields

        /// <summary>
        /// The quoted value expression regualr expression string.
        /// </summary>
        private static readonly string _quotedValueSubExpression = QUOTED_VALUE_SUB_EXPRESSION_PREFIX + '>' + QUOTED_VALUE_SUB_EXPRESSION_POSTFIX;

        /// <summary>
        /// The quoted value with close parenthesis regular expression string.
        /// </summary>
        private static readonly string _quotedValueSubExpressionCloseParenthesis = QUOTED_VALUE_SUB_EXPRESSION_PREFIX + ')' + QUOTED_VALUE_SUB_EXPRESSION_POSTFIX;
        
        /// <summary>
        /// The Template Logger.
        /// </summary>
        private static readonly TemplatingLogger _log = TemplatingLogger.GetLogger(typeof(ExtractBinariesFromText));

        /// <summary>
        /// The src attribute regex expression.
        /// </summary>
        /// <remarks>
        /// <??? ... src = ['|"]path['|"] ...>
        /// </remarks>
        private static readonly Regex _srcAttributeExpression = new Regex(@"<\w+[^>]*\s(?<name>src)\s*=" + _quotedValueSubExpression, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        
        /// <summary>
        /// The href attribute regex expression.
        /// </summary>
        /// <remarks>
        /// <??? ... href = ['|"]path['|"] ...>
        /// </remarks>
        private static readonly Regex _hrefAttributeExpression = new Regex(@"<\w+[^>]*\s(?<name>href)\s*=" + _quotedValueSubExpression, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        
        /// <summary>
        /// The css import regex expression. Limited to text/css type links here (no real need in that unmatched paths just remain)
        /// </summary>
        /// <remarks>
        /// @import url ( ['|"]path['|"] )
        /// </remarks>
        internal static readonly Regex _cssImportExpression = new Regex(@"@(?<name>import)\s+(url\s*\()?" + _quotedValueSubExpressionCloseParenthesis + @"\s*(\))?\s*;", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        
        /// <summary>
        /// The css url regex expression.
        /// </summary>
        /// <remarks>
        /// { ... url(["|']path["|']);...} (so no explicit check that it is really in a CSS section
        /// </remarks>
        internal static readonly Regex _cssUrlExpression = new Regex(@"{[^}]+(?<name>url)\s*\(" + _quotedValueSubExpressionCloseParenthesis + @"\s*\)\s*;.*}", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// The template content string.
        /// </summary>
        private string _contentString;

        #endregion

        #region Properties

        /// <summary>
        /// Retrieve the HTML content
        /// </summary>
        public object Content
        {
            get
            {
                return _contentString;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="text">The html text to process</param>
        internal ExtractBinariesFromText(string text)
        {
            _contentString = text;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get an Iterator of all the link attribute values in the document.
        /// </summary>
        /// <returns>An enumerator of link attribute matches.</returns>
        public IEnumerable<LinkReferenceWrapper> GetLinkAttributes()
        {
            Regex[] expressions = new Regex[] { _srcAttributeExpression, _hrefAttributeExpression, _cssImportExpression, _cssUrlExpression };
            foreach (LinkReferenceWrapper linkAttribute in GetLinkAttributes(expressions))
            {
                yield return linkAttribute;
            }
        }

        /// <summary>
        /// The the attribute value for a linkAttribute
        /// </summary>
        /// <param name="linkAttribute">The link attribute representation</param>
        /// <returns>The attribute value as a string</returns>
        public string GetAttributeValue(LinkReferenceWrapper linkAttribute)
        {
            // Remove leading and trailing quotes
            string pathAttributeValue = linkAttribute._outputPathAttributeValue.Trim(new char[] { '\'', '"' });
            return pathAttributeValue;
        }

        /// <summary>
        /// Add link attributes for a target item to an existing attribute,
        /// or change URL to a TCM URI in a CSS url reference.
        /// </summary>
        /// <param name="linkAttribute">The link attribute representation</param>
        /// <param name="targetItemUri">The Tcm Uri of the target</param>
        public void ProcessLinkChange(LinkReferenceWrapper linkAttribute, TcmUri targetItemUri)
        {
            // Make distinction between import and other statements
            string replacementFormatString;
            if ((linkAttribute._attributeName.ToLower() == "import") || (linkAttribute._attributeName.ToLower() == "url"))
            {
                // CSS reference
                replacementFormatString = MULTIMEDIA_CSS_IMPORT_ATTRIBUTE_FORMAT;
            }
            else
            {
                // other link
                replacementFormatString = MULTIMEDIA_LINK_ATTRIBUTE_FORMAT;
            }

            InjectAttribute(linkAttribute, replacementFormatString, targetItemUri);
        }

        /// <summary>
        /// Get an Iterator of all the link attribute values in the document.
        /// </summary>
        /// <param name="expressions">The regular expressions to execute on the text</param>
        /// <returns>An enumerator of link attribute matches.</returns>
        internal IEnumerable<LinkReferenceWrapper> GetLinkAttributes(Regex[] expressions)
        {
            string[] templateStringContainer = new string[] { _contentString }; ;
            foreach (Regex expression in expressions)
            {
                foreach (Match linkMatch in TemplateUtilities.GetRegexMatches(templateStringContainer, expression))
                {
                    LinkReferenceWrapper linkAttribute = new LinkReferenceWrapper(linkMatch);
                    linkAttribute._linkPosition = linkMatch.Groups[MATCH_PATH_GROUP_NAME];
                    linkAttribute._attributeName = linkMatch.Groups[MATCH_NAME_GROUP_NAME].ToString();
                    linkAttribute._outputPathAttributeValue = _contentString.Substring
                        (linkAttribute._linkPosition.Index, linkAttribute._linkPosition.Length);
                    yield return linkAttribute;
                    templateStringContainer[0] = _contentString;
                }
            }
        }

        /// <summary>
        /// Replace an existing link attribute value in a link reference.
        /// </summary>
        /// <param name="linkAttribute">The link to process</param>
        /// <param name="replacementValue">The new value to store</param>
        internal void ReplaceValue(LinkReferenceWrapper linkAttribute, string replacementValue)
        {
            InjectAttribute(linkAttribute, ATTRIBUTE_VALUE_REPLACEMENT_FORMAT, replacementValue);
        }

        /// <summary>
        /// Utility method to change a link attribute, by entering a new value.
        /// </summary>
        /// <param name="linkAttribute">The link reference representation to alter</param>
        /// <param name="replacementFormatString">The string-format to use in updating the value</param>
        /// <param name="replacementValue">The new value to enter into the document</param>
        private void InjectAttribute(LinkReferenceWrapper linkAttribute, string replacementFormatString, object replacementValue)
        {
            string attributeInjection = String.Format(replacementFormatString,
                linkAttribute._outputPathAttributeValue,		// 0
                replacementValue,							// 1
                linkAttribute._attributeName);				// 2
            Group linkPosition = linkAttribute._linkPosition;
            int linkEndPosition = linkPosition.Index + linkPosition.Length;

            _contentString = _contentString.Substring(0, linkPosition.Index) + attributeInjection +
                _contentString.Substring(linkEndPosition, _contentString.Length - linkEndPosition);
        }

        #endregion
    }
}
