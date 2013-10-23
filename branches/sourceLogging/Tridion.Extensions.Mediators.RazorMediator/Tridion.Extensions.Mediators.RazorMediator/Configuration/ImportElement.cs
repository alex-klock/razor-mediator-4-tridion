using System;
using System.Configuration;

namespace Tridion.Extensions.Mediators.Razor.Configuration
{
    /// <summary>
    /// Represents an import element within a imports element collection.
    /// </summary>
    /// <example>
    ///   <add import="tcm:289-34443" />
    ///   <add import="C:\Program Files\Razor\razor-helpers.txt" />
    ///   <add import="/webdav/020 Design/Building Blocks/System/TBBs/Razor-Helpers" />
    /// </example>
    public class ImportElement : ConfigurationElement
    {
        /// <summary>
        /// Gets the path property for this element. Path can be a physical path on a filesystem, a Web Dav URL, or even a TCM URI.
        /// </summary>
        [ConfigurationProperty("import", IsKey = true, IsRequired = true)]
        public string Import
        {
            get
            {
                return (string)this["import"];
            }
            set
            {
                this["import"] = value;
            }
        }

        /// <summary>
        /// Gets the publications that this import will be applied to. (Comma separated Publication titles.)
        /// </summary>
        [ConfigurationProperty("publications", IsKey = false, IsRequired = false)]
        public string Publications
        {
            get
            {
                if (this["publications"] == null)
                    return String.Empty;

                return (string)this["publications"];
            }
            set
            {
                this["publications"] = value;
            }
        }
    }
}
