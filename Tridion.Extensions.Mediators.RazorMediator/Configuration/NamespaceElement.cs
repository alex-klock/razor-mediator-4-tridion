using System.Configuration;

namespace Tridion.Extensions.Mediators.Razor.Configuration
{
    /// <summary>
    /// Represents a namespace element within a namspace element collection.
    /// </summary>
    /// <example><add namespace="System.Linq" /></example>
    public class NamespaceElement : ConfigurationElement
    {
        /// <summary>
        /// Gets the namespace property for this element.
        /// </summary>
        [ConfigurationProperty("namespace", IsKey = true, IsRequired = true)]
        public string Namespace
        {
            get
            {
                return (string)this["namespace"];
            }
            set
            {
                this["namespace"] = value;
            }
        }
    }
}
