﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tridion.Extensions.Mediators.Razor.Templating
{
    /// <summary>
    /// Generator for Razor Templates.
    /// </summary>
    public class RazorTemplateGenerator : IRazorTemplateGenerator
    {
        /// <summary>
        /// The template items to compile.
        /// </summary>
        public static Dictionary<string, RazorTemplateEntry> TemplateItems = new Dictionary<string, RazorTemplateEntry>();

        /// <summary>
        /// Clears the template cache of expired temlates.
        /// </summary>
        /// <param name="cacheTime">The time in seconds to expire templates by.</param>
        public void ClearCache(int cacheTime)
        {
            var entriesToRemove = TemplateItems.Where(e => e.Value.CompiledTime.AddSeconds(cacheTime) < DateTime.Now).Select(e => e.Value).ToArray();
            foreach (var entry in entriesToRemove)
            {
                TemplateItems.Remove(TranslateKey(entry.TemplateType, entry.TemplateID));
            }
        }

        /// <summary>
        /// Registers a template of type RazorTemplateBase to compile.
        /// </summary>
        /// <typeparam name="T">Type of RazorTemplateBase.</typeparam>
        /// <param name="templateID">The template's unique identifier.</param>
        /// <param name="templateString">The template string to compile.</param>
        /// <param name="namespaces">The namespaces to add to the template.</param>
        public void RegisterTemplate<T>(string templateID, string templateString, IEnumerable<string> namespaces, DateTime templateLastUpdated) where T : RazorTemplateBase
        {
            if (templateString == null)
                throw new ArgumentNullException("templateString");

            string key = TranslateKey(typeof(T), templateID);

            RazorTemplateEntry entry = new RazorTemplateEntry()
            {
                TemplateType = typeof(T),
                TemplateID = templateID,
                TemplateUpdated = templateLastUpdated,
                TemplateString = templateString,
                TemplateName = "Rzr" + Guid.NewGuid().ToString("N") + "Template",
                Namespaces = namespaces
            };

            if (!TemplateItems.ContainsKey(key))
            {
                TemplateItems[key] = entry;
            }
            else
            {
                if (TemplateItems[key].TemplateUpdated < templateLastUpdated || TemplateItems[key].Assembly == null)
                {
                    TemplateItems[key] = entry;
                }
            }
        }

        /// <summary>
        /// Compiles the registered templates.
        /// </summary>
        public void CompileTemplates(IEnumerable<string> assemblyReferences)
        {
            Compiler.Compile(TemplateItems.Values, assemblyReferences);
        }

        /// <summary>
        /// Generates the RazorTemplate that is ready to be executed.
        /// </summary>
        /// <typeparam name="T">Type of RazorTemplateBase</typeparam>
        /// <returns>The RazorTemplate that is ready to execute.</returns>
        public T GenerateTemplate<T>(string templateID) where T : RazorTemplateBase
        {
            RazorTemplateEntry entry = null;

            if (TemplateItems.Count == 0)
                throw new InvalidOperationException("Templates have not been compiled.");

            try
            {
                entry = TemplateItems[TranslateKey(typeof(T), templateID)];
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentOutOfRangeException("No template has been registered under name.");
            }
            

            var template = (T)entry.Assembly.CreateInstance("Tridion.Extensions.Mediators.Razor.Templating." + entry.TemplateName);

            return template;
        }

        /// <summary>
        /// Removes a template from the entry list.
        /// </summary>
        /// <param name="templateID"></param>
        public void RemoveTemplate(string templateID)
        {
            RazorTemplateEntry entryToRemove = TemplateItems.Where(e => e.Value.TemplateID.Equals(templateID)).Select(e => e.Value).FirstOrDefault();
            if (entryToRemove != null)
                TemplateItems.Remove(TranslateKey(entryToRemove.TemplateType, entryToRemove.TemplateID));
        }

        /// <summary>
        /// Determines the key to use for retrieving template entries.
        /// </summary>
        /// <param name="type">The template's type.</param>
        /// <returns>A key to use for the dictionary of template items.</returns>
        private string TranslateKey(Type type, string templateID)
        {
            return string.Format("RzrTmpl::{0}-{1}", type.Name, templateID);
        }
    } 

}
