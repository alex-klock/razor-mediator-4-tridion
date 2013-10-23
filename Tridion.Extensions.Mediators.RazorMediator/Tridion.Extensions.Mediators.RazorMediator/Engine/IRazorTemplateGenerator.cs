using System;
using System.Collections.Generic;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.Extensions.Mediators.Razor.Configuration;

namespace Tridion.Extensions.Mediators.Razor.Templating
{
    /// <summary>
    /// Generator for Razor Templates.
    /// </summary>
    public interface IRazorTemplateGenerator
    {
        /// <summary>
        /// Clears all the template cache.
        /// </summary>
        void ClearAllCache();

        /// <summary>
        /// Clears the template cache of expired temlates.
        /// </summary>
        void ClearCache();

        /// <summary>
        /// Clears the where used items of this template.
        /// </summary>
        /// <param name="template"></param>
        /// <remarks>
        /// Not sure if I like the dependency to the TBB... I might want a TODO to think another way to remove dependencies from cache.
        /// </remarks>
        void ClearWhereUsed(TemplateBuildingBlock template);

        /// <summary>
        /// Whether or not a template has been updated.
        /// </summary>
        bool IsTemplateUpdated<T>(string templateID, DateTime templateLastUpdated) where T : RazorTemplateBase;

        /// <summary>
        /// Registers a template of type RazorTemplateBase to compile.
        /// </summary>
        void RegisterTemplate<T>(string templateID, string templateString, IEnumerable<string> namespaces, DateTime templateLastUpdated) where T : RazorTemplateBase;

        /// <summary>
        /// Compiles the registered templates.
        /// </summary>
        void CompileTemplates(IEnumerable<string> assemblyReferences, RazorMediatorConfigurationSection _config);

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
