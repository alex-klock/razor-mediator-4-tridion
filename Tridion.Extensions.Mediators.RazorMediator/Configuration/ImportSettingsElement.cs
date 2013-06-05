using System;
using System.Configuration;

namespace Tridion.Extensions.Mediators.Razor.Configuration
{
    /// <summary>
    /// Represents the importSettings element of the razor mediator config..
    /// </summary>
    /// <example>
    ///   <importSettings includeConfigWhereUsed="true" includeImportWhereUsed="true" replaceRelativePaths="false" />
    /// </example>
    public class ImportSettingsElement : ConfigurationElement
    {
        /// <summary>
        /// Gets whether or not to include imports from the razor config section to be included as Where Used.
        /// </summary>
        [ConfigurationProperty("includeConfigWhereUsed", IsKey = true, IsRequired = false)]
        public bool IncludeConfigWhereUsed
        {
            get
            {
                if (this["includeConfigWhereUsed"] == null)
                    return false;

                return this["includeConfigWhereUsed"].ToString().ToLower().Equals("true");
            }
            set
            {
                this["includeConfigWhereUsed"] = value;
            }
        }

        /// <summary>
        /// Gets whether or not to include imports from importRazor("path") statements to be included as Where Used.
        /// </summary>
        [ConfigurationProperty("includeImportWhereUsed", IsKey = true, IsRequired = false)]
        public bool IncludeImportWhereUsed
        {
            get
            {
                if (this["includeImportWhereUsed"] == null)
                    return false;

                return this["includeImportWhereUsed"].ToString().ToLower().Equals("true");
            }
            set
            {
                this["includeImportWhereUsed"] = value;
            }
        }

        /// <summary>
        /// Gets whether or not to replace relative paths in importRazor("path") statements with full webdav paths.
        /// </summary>
        [ConfigurationProperty("replaceRelativePaths", IsKey = true, IsRequired = false)]
        public bool ReplaceRelativePaths
        {
            get
            {
                if (this["replaceRelativePaths"] == null)
                    return false;

                return this["replaceRelativePaths"].ToString().ToLower().Equals("true");
            }
            set
            {
                this["replaceRelativePaths"] = value;
            }
        }
    }
}
