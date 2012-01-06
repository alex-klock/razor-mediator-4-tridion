using System.Collections.Generic;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.Publishing;
using Tridion.ContentManager.Templating;
using Tridion.Extensions.Mediators.Razor.Models;
using Tridion.Extensions.Mediators.Razor.Templating;
using Tridion.Extensions.Mediators.Razor.Utilities;
using System.Web;
using Tridion.ContentManager;
using Tridion.ContentManager.Templating.Expression;
using System.Reflection;
using System;
using System.Text.RegularExpressions;

namespace Tridion.Extensions.Mediators.Razor
{
    /// <summary>
    /// A Tridion RazorTemplate. Allows quick and easy access to Tridion package items and helper functions from the Razor Template.
    /// </summary>
    public class TridionRazorTemplate : RazorTemplateBase
    {
        private List<string> _references = new List<string>();
        private Engine _engine;
        private Template _template;
        private Package _package;
        private TemplatingLogger _logger;
        private TridionUtilities _tridionHelper;
        private BuiltInFunctions _builtInFunctions;
        private ModelUtilities _models;
        private ComponentModel _component;
        private dynamic _dynamicPackage;
        private PageModel _page;
        private PublicationModel _publication;

        /// <summary>
        /// Gets the Tridion Templating Logger instance.
        /// </summary>
        public TemplatingLogger Log
        {
            get 
            {
                if (_logger == null)
                    _logger = TemplatingLogger.GetLogger(this.GetType());
                return _logger;
            }
        }

        /// <summary>
        /// Gets the TridionUtilities helper class.
        /// </summary>
        public TridionUtilities TridionHelper
        {
            get
            {
                return _tridionHelper;
            }
        }

        /// <summary>
        /// Gets the template's ComponentModel.  The Component package item must exist (ComponentTemplate).
        /// </summary>
        public dynamic Component
        {
            get 
            {
                if (_component == null)
                {
                    Component c = _tridionHelper.GetComponent();
                    _component = new ComponentModel(_engine, c);
                }
                return _component; 
            }
        }

        /// <summary>
        /// Gets the Page's ComponentPresentationModels (requires access to Page). Shortcut for @Page.ComponentPresentations
        /// </summary>
        public List<ComponentPresentationModel> ComponentPresentations
        {
            get
            {
                return Page.ComponentPresentations;
            }
        }

        /// <summary>
        /// Gets the ComponentModel's Field items. Shortcut for @Component.Fields.
        /// </summary>
        public dynamic Fields
        {
            get
            {
                return Component.Fields;
            }
        }

        /// <summary>
        /// Gets the Component's Folder item. Shortcut for @Component.Folder.
        /// </summary>
        public dynamic Folder
        {
            get
            {
                return Component.Folder;
            }
        }

        /// <summary>
        /// Gets the ComponentModel's Metadata items. Shortcut for @Component.MetaData
        /// </summary>
        public dynamic MetaData
        {
            get
            {
                return Component.MetaData;
            }
        }

        /// <summary>
        /// Gets the ComponentModel's Metadata items. Shortcut for @Component.MetaData
        /// </summary>
        public dynamic Metadata
        {
            get
            {
                return Component.MetaData;
            }
        }

        /// <summary>
        /// Gets the ModelUtilities helper class.
        /// </summary>
        public ModelUtilities Models
        {
            get
            {
                if (_models == null)
                    _models = new ModelUtilities(_engine);

                return _models;
            }
        }

        /// <summary>
        /// Gets the DynamicPackage item. Allows dynamic access to the Tridion package's variables.
        /// </summary>
        public dynamic Package
        {
            get
            {
                if (_dynamicPackage == null)
                {
                    _dynamicPackage = new DynamicPackage(_engine, _package);

                }
                return _package;
            }
        }

        /// <summary>
        /// Gets the template's PageModel item. Will work with a PageTemplate, or a ComponentTemplate being executed from a Page.
        /// Will return NULL if used on a dynamic component presentation or previewing a CT.
        /// </summary>
        public dynamic Page
        {
            get
            {
                if (_page == null)
                {
                    Page page = _tridionHelper.GetPage();

                    if (page != null)
                    {
                        _page = new PageModel(_engine, page);
                    }
                }

                return _page;
            }
        }

        /// <summary>
        /// Gets the current PublicationModel that the template is being run from.
        /// </summary>
        public dynamic Publication
        {
            get
            {
                if (_publication == null)
                {
                    _publication = new PublicationModel(_engine, _tridionHelper.GetPublication());
                }

                return _publication;
            }
        }

        /// <summary>
        /// Gets the render mode.
        /// </summary>
        public string RenderMode
        {
            get
            {
                return _engine.RenderMode.ToString();
            }
        }

        /// <summary>
        /// Gets the page's StructureGroupModel. Must be able to access the Page property (everything except a dynamic component presentation).
        /// </summary>
        public dynamic StructureGroup
        {
            get
            {
                return Page.StructureGroup;
            }
        }

        /// <summary>
        /// Gets the Tridion Compound Template being executed.
        /// </summary>
        public Template Template
        {
            get
            {
                if (_template == null)
                {
                    _template = _engine.PublishingContext.ResolvedItem.Template;
                }
                return _template;
            }
        }

        /// <summary>
        /// Gets whether or not this is a ComponentTemplate.
        /// </summary>
        public bool IsComponentTemplate
        {
            get
            {
                return Template is ComponentTemplate;
            }
        }

        /// <summary>
        /// Gets whether or not this is a PageTemplate.
        /// </summary>
        public bool IsPageTemplate
        {
            get
            {
                return Template is PageTemplate;
            }
        }

        public TridionRazorTemplate()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(TemplateAssemblyResolveEventHandler);
        }

        /// <summary>
        /// Initializes the TridionRazorTemplate.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="package"></param>
        public void Initialize(Engine engine, Package package, List<string> assemblyReferences)
        {
            _engine = engine;
            _package = package;
            _tridionHelper = new TridionUtilities(engine, package);
            _builtInFunctions = new BuiltInFunctions(engine, package);
            _references = assemblyReferences;
        }

        /// <summary>
        /// For tests only...
        /// </summary>
        /// <param name="assemblyReferences"></param>
        public void Initialize(List<string> assemblyReferences)
        {
            _references = assemblyReferences;
        }

        /// <summary>
        /// Gets a list of ComponentPresentationModels from the Component Presentations on the page. Callable from a PageTemplate.
        /// </summary>
        /// <param name="templateName">The template's name to filter by.</param>
        /// <returns>A list of ComponentPresentationModels that match the template's name.</returns>
        public List<ComponentPresentationModel> GetComponentPresentationsByTemplate(string templateName)
        {
            List<ComponentPresentationModel> componentPresentations = new List<ComponentPresentationModel>();
            foreach (ComponentPresentationModel cp in ComponentPresentations)
            {
                if (cp.Template.Title.Equals(templateName))
                {
                    componentPresentations.Add(cp);
                }
            }

            

            return componentPresentations;
        }

        /// <summary>
        /// Converts an HTML encoded string into a decoded string.
        /// </summary>
        public string HtmlDecode(string textToDecode)
        {
            return HttpUtility.HtmlDecode(textToDecode);
        }

        /// <summary>
        /// Converts a string into a HTML encoded string.
        /// </summary>
        public string HtmlEncode(string textToEncode)
        {
            return HttpUtility.HtmlEncode(textToEncode);
        }

        /// <summary>
        /// Converts a URL encoded string into a decoded string.
        /// </summary>
        public string UrlDecode(string textToDecode)
        {
            return HttpUtility.UrlDecode(textToDecode);
        }

        /// <summary>
        /// Converts a string into a URL encoded string.
        /// </summary>
        public string UrlEncode(string textToEncode)
        {
            return HttpUtility.UrlEncode(textToEncode);
        }

        /// <summary>
        /// Strips a string of html elements.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public string StripHtml(string html)
        {
            return Regex.Replace(html, @"<(.|\n)*?>", String.Empty);
        }

        /// <summary>
        /// Renders a component field embedded in a tcdl:ComponenField tag.
        /// </summary>
        /// <param name="fieldExpression">Reference to a field relative to the context component. For example Fields.MyEmbeddedSchema.MyField</param>
        /// <param name="fieldIndex">Index of this value for multi-valued fields starting at 1. Single-value fields simply use 1.</param>
        /// <returns>The rendered component field.</returns>
        public string RenderComponentField(string fieldExpression, int fieldIndex)
        {
            return _builtInFunctions.RenderComponentField(fieldExpression, fieldIndex);
        }

        /// <summary>
        /// Renders a component field embedded in a tcdl:ComponenField tag.
        /// </summary>
        /// <param name="fieldExpression">Reference to a field relative to the context component. For example Fields.MyEmbeddedSchema.MyField</param>
        /// <param name="fieldIndex">Index of this value for multi-valued fields starting at 1. Single-value fields simply use 1.</param>
        /// <param name="value">Instead of looking up the field value, passthrough a value, eg IMG/A tags</param>
        /// <returns>The rendered component field.</returns>
        public string RenderComponentField(string fieldExpression, int fieldIndex, string value)
        {
            return _builtInFunctions.RenderComponentField(fieldExpression, fieldIndex, value);
        }

        /// <summary>
        /// Renders a component field embedded in a tcdl:ComponenField tag.
        /// </summary>
        /// <param name="fieldExpression">Reference to a field relative to the context component. For example Fields.MyEmbeddedSchema.MyField</param>
        /// <param name="fieldIndex">Index of this value for multi-valued fields starting at 1. Single-value fields simply use 1.</param>
        /// <param name="htmlEncodeResult">Html encodes the result if set to true.</param>
        /// <param name="resolveHtmlAsRTFContent">Resolves HTML as RTF content if set to true.</param>
        /// <returns>The rendered component field.</returns>
        public string RenderComponentField(string fieldExpression, int fieldIndex, bool htmlEncodeResult, bool resolveHtmlAsRTFContent)
        {
            return _builtInFunctions.RenderComponentField(fieldExpression, fieldIndex, htmlEncodeResult, resolveHtmlAsRTFContent);
        }

        /// <summary>
        /// Renders a Component Presentation.
        /// </summary>
        /// <param name="componentID">The component's tcm uri.</param>
        /// <param name="templateID">The template's tcm uri.</param>
        /// <returns>The rendered component presentation.</returns>
        public string RenderComponentPresentation(TcmUri componentID, TcmUri templateID)
        {
            return _engine.RenderComponentPresentation(componentID, templateID);
        }

        /// <summary>
        /// Renders a Component Presentation.
        /// </summary>
        /// <param name="componentID">The component's tcm uri.</param>
        /// <param name="templateID">The template's tcm uri.</param>
        /// <returns>The rendered component presentation.</returns>
        public string RenderComponentPresentation(string componentID, string templateID)
        {
            TcmUri componentUri = new TcmUri(componentID);
            TcmUri templateUri = new TcmUri(templateID);

            return RenderComponentPresentation(componentUri, templateUri);
        }

        /// <summary>
        /// Executes the template.
        /// </summary>
        public override void Execute()
        {

        }

        protected Assembly TemplateAssemblyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            var start = DateTime.Now;

            string assembly = "\\" + args.Name.Substring(0, args.Name.IndexOf(",")) + ".dll";
            foreach (string reference in _references)
            {
                if (reference.EndsWith(assembly))
                {
                    try
                    {
                        Console.WriteLine("Found Location: " + (start - DateTime.Now));
                        return Assembly.LoadFrom(reference);
                    }
                    finally
                    {
                        Console.WriteLine("Assembly Resolver: " + (start - DateTime.Now));
                    }
                }
            }
            
            return null;
        }
    }
}
