using System;
using System.Collections.Generic;
using System.Reflection;

namespace Tridion.Extensions.Mediators.Razor.Templating
{
    /// <summary>
    /// Represents a Razor Template entry to be compiled.
    /// </summary>
    public class RazorTemplateEntry
    {
        /// <summary>
        /// The compiled assembly.
        /// </summary>
        public Assembly Assembly { get; internal set; }

        /// <summary>
        /// The time the template was compiled and added to teh template list.
        /// </summary>
        public DateTime CompiledTime { get; set; }

        /// <summary>
        /// The template type.
        /// </summary>
        public Type TemplateType { get; set; }

        /// <summary>
        /// The time the template was last updated.
        /// </summary>
        public DateTime TemplateUpdated { get; set; }

        /// <summary>
        /// The template string.
        /// </summary>
        public string TemplateString { get; set; }

        /// <summary>
        /// The template's name.
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// Gets or sets the template's unique identifier (used for cache).
        /// </summary>
        public string TemplateID { get; set; }

        /// <summary>
        /// The namespaces to be imported in the template.
        /// </summary>
        public IEnumerable<string> Namespaces { get; set; }
    }
}
