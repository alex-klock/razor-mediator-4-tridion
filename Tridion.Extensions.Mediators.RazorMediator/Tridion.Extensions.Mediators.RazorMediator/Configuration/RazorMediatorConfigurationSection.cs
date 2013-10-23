using System;
using System.Configuration;
using Tridion.ContentManager.Templating;

namespace Tridion.Extensions.Mediators.Razor.Configuration
{
    /// <summary>
    /// Represents the razor.mediator configuration section in the Tridion ContentManager.Settings.config file.
    /// </summary>
    public class RazorMediatorConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// Gets the Razor Mediator Configuration Section from the ContentManager.Settings.config configuration file.
        /// </summary>
        /// <returns></returns>
        public static RazorMediatorConfigurationSection GetConfiguration()
        {
            return (RazorMediatorConfigurationSection)TemplateUtilities.GetTemplatingSettings().CurrentConfiguration.GetSection("razor.mediator");
        }

        [ConfigurationProperty("adminUser", IsRequired = false)]
        public string AdminUser
        {
            get
            {
                if (this["adminUser"] == null)
                    return String.Empty;

                return this["adminUser"].ToString();
            }
            set
            {
                this["adminUser"] = value;
            }
        }

        /// <summary>
        /// Gets whether or not the Razor Mediator should extract binaries.
        /// </summary>
        [ConfigurationProperty("extractBinaries", IsRequired = true)]
        public bool ExtractBinaries
        {
            get
            {
                return this["extractBinaries"].ToString().ToLower().Equals("true");
            }
            set
            {
                this["extractBinaries"] = value;
            }
        }

        /// <summary>
        /// Gets whether or not the Razor Mediator should extract binaries.
        /// </summary>
        [ConfigurationProperty("dumpGeneratedSource", IsRequired = false, DefaultValue=false)]
        public bool DumpGeneratedSource
        {
            get
            {
                return this["dumpGeneratedSource"].ToString().ToLower().Equals("true");
            }
            set
            {
                this["dumpGeneratedSource"] = value;
            }
        }

        // TODO - deal with sensible defaults... 
        [ConfigurationProperty("generatedSourceFolder", IsRequired = false)]
        public string GeneratedSourceFolder
        {
            get 
            {
                return (string)this["generatedSourceFolder"];
            }
            set
            {
                this["generatedSourceFolder"] = value;
            }
        }

        /// <summary>
        /// Namespaces element of the config section.
        /// </summary>
        [ConfigurationProperty("namespaces")]
        public NamespaceElementCollection Namespaces
        {
            get
            {
                return (NamespaceElementCollection)this["namespaces"];
            }
            set
            {
                this["namespaces"] = value;
            }
        }

        /// <summary>
        /// Assemblies element of the config section.
        /// </summary>
        [ConfigurationProperty("assemblies")]
        public AssemblyElementCollection Assemblies
        {
            get
            {
                return (AssemblyElementCollection)this["assemblies"];
            }
            set
            {
                this["assemblies"] = value;
            }
        }

        /// <summary>
        /// Imports element of the config section.
        /// </summary>
        [ConfigurationProperty("imports")]
        public ImportElementCollection Imports
        {
            get
            {
                return (ImportElementCollection)this["imports"];
            }
            set
            {
                this["imports"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the ImportSettings element of the config section.
        /// </summary>
        [ConfigurationProperty("importSettings", IsRequired = false)]
        public ImportSettingsElement ImportSettings
        {
            get
            {
                return this["importSettings"] as ImportSettingsElement;
            }
            set
            {
                this["importSettings"] = value;
            }
        }
    }
}
