using System.Configuration;

namespace Tridion.Extensions.Mediators.Razor.Configuration
{
    /// <summary>
    /// Represents an assembly element within a namspace element collection.
    /// </summary>
    /// <example>
    ///   <add assembly="System.Security.dll" />
    ///   <add assembly="C:\Program Files\Razor\Test.Sample.dll />
    /// </example>
    public class AssemblyElement : ConfigurationElement
    {
        /// <summary>
        /// Gets the assembly property for this element.
        /// </summary>
        [ConfigurationProperty("assembly", IsKey = true, IsRequired = true)]
        public string Assembly
        {
            get
            {
                return (string)this["assembly"];
            }
            set
            {
                this["assembly"] = value;
            }
        }
    }
}
