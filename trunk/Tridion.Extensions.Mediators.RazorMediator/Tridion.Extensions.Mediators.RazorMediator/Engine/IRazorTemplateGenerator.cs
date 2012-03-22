using System;
using System.Collections.Generic;

namespace Tridion.Extensions.Mediators.Razor.Templating
{
    /// <summary>
    /// Generator for Razor Templates.
    /// </summary>
    public interface IRazorTemplateGenerator
    {
        /// <summary>
        /// Clears the template cache of expired temlates.
        /// </summary>
        /// <param name="cacheTime">The time in seconds to expire templates by.</param>
        void ClearCache(int cacheTime);

        /// <summary>
        /// Registers a template of type RazorTemplateBase to compile.
        /// </summary>
        void RegisterTemplate<T>(string templateID, string templateString, IEnumerable<string> namespaces, DateTime templateLastUpdated) where T : RazorTemplateBase;

        /// <summary>
        /// Compiles the registered templates.
        /// </summary>
        void CompileTemplates(IEnumerable<string> assemblyReferences);

        /// <summary>
        /// Generates the RazorTemplate that is ready to be executed.
        /// </summary>
        T GenerateTemplate<T>(string templateID) where T: RazorTemplateBase;

        /// <summary>
        /// Removes a template from the entry list.
        /// </summary>
        /// <param name="templateID"></param>
        void RemoveTemplate(string templateID);
    }
}
