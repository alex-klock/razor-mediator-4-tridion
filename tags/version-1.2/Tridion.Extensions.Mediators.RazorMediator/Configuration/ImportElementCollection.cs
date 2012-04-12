using System.Configuration;

namespace Tridion.Extensions.Mediators.Razor.Configuration
{
    /// <summary>
    /// Represents the import collection element.
    /// </summary>
    [ConfigurationCollection(typeof(AssemblyElement))]
    public class ImportElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Creates a new element.
        /// </summary>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ImportElement();
        }

        /// <summary>
        /// Gets the element key for the specified element.
        /// </summary>
        /// <param name="element">The configuration element to get the key for.</param>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ImportElement)element).Import;
        }
    }
}
