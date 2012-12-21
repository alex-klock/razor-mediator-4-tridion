using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using Tridion.ContentManager;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.Templating;
using Tridion.ContentManager.Templating.Expression;
using Tridion.Extensions.Mediators.Razor.Models;
using Tridion.Extensions.Mediators.Razor.Templating;
using Tridion.Extensions.Mediators.Razor.Utilities;

namespace Tridion.Extensions.Mediators.Razor
{
    /// <summary>
    /// A Tridion RazorTemplate. Allows quick and easy access to Tridion package items and helper functions from the Razor Template.
    /// </summary>
    public class TridionRazorTemplate : RazorTemplateBase
    {
        /// <summary>
        /// A list of references that this assembly uses.
        /// </summary>
        private List<string> _references = new List<string>();

        /// <summary>
        /// The Tridion Engine instance.
        /// </summary>
        private Engine _engine;

        /// <summary>
        /// The Tridion Compound Template.
        /// </summary>
        private Template _template;

        /// <summary>
        /// The Tridion Package.
        /// </summary>
        private Package _package;

        /// <summary>
        /// The Component Template model.
        /// </summary>
        private ComponentTemplateModel _componentTemplate;

        /// <summary>
        /// The Tridion Logging instance.
        /// </summary>
        private TemplatingLogger _logger;

        /// <summary>
        /// A Tridion utilities and helpers instance.
        /// </summary>
        private TridionUtilities _tridionHelper;

        /// <summary>
        /// Built in functions for Tridion.
        /// </summary>
        private BuiltInFunctions _builtInFunctions;
        private ModelUtilities _models;
        private ComponentModel _component;
        private dynamic _dynamicPackage;
        private PageModel _page;
        private PublicationModel _publication;
        private bool _isSiteEditEnabled;
        private bool _cachedIsSiteEditEnabled = false;

        /// <summary>
        /// The Razor Template Building Block.
        /// </summary>
        private Template _razorTBB;

        /// <summary>
        /// The Razor TBB Model.
        /// </summary>
        private RazorTemplateModel _razorTemplate;

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
        /// Gets the ComponentTemplate if this is a Component Template, else returns null.
        /// </summary>
        public dynamic ComponentTemplate
        {
            get
            {
                if (IsComponentTemplate)
                {
                    if (_componentTemplate == null)
                    {
                        _componentTemplate = new ComponentTemplateModel(_engine, (ComponentTemplate)Template);
                    }

                    return _componentTemplate;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the Page's ComponentPresentationModels (requires access to Page). Shortcut for @Page.ComponentPresentations
        /// </summary>
        public List<ComponentPresentationModel> ComponentPresentations
        {
            get
            {
                return Page != null ? Page.ComponentPresentations : null;
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
        /// Gets whether or not Site Edit is enabled.
        /// </summary>
        public bool IsSiteEditEnabled
        {
            get
            {
                if (!_cachedIsSiteEditEnabled)
                {
                    if (_engine.PublishingContext.PublicationTarget != null && _engine.PublishingContext.PublicationTarget.Id != TcmUri.UriNull)
                    {
                        ApplicationData data = _engine.PublishingContext.PublicationTarget.LoadApplicationData("SiteEdit");
                        if (data == null)
                        {
                            _cachedIsSiteEditEnabled = true;
                            return false;
                        }

                        XmlElement appData = data.GetAs<XmlElement>();
                        XmlNodeList nodes = appData.GetElementsByTagName("EnableSiteEdit");
                        if (nodes.Count > 0 && nodes[0].InnerText.Equals("true"))
                        {
                            _isSiteEditEnabled = true;
                        }
                    }
                    _cachedIsSiteEditEnabled = true;
                }
                return _isSiteEditEnabled;
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
        /// Gets the PageModel's Metadata items if its a PageTemplate, else gets the ComponentModel's Metadata items. Shortcut for @Page.Metadata or @Component.MetaData.
        /// </summary>
        public dynamic Metadata
        {
            get
            {
                if (IsPageTemplate)
                {
                    return Page.MetaData;
                }
                else
                {
                    return Component.MetaData;
                }
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
                return _dynamicPackage;
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
        /// Gets the PageTemplate if a Page is available, else returns null.
        /// </summary>
        public dynamic PageTemplate
        {
            get
            {
                if (Page != null)
                {
                    return Page.PageTemplate;
                }
                else
                {
                    return null;
                }
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
        /// Gets the Razor Template model.
        /// </summary>
        public RazorTemplateModel RazorTemplate
        {
            get
            {
                if (_razorTemplate == null)
                {
                    _razorTemplate = new RazorTemplateModel(_engine, _razorTBB);
                }

                return _razorTemplate;
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

        /// <summary>
        /// Constructor.
        /// </summary>
        public TridionRazorTemplate()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(TemplateAssemblyResolveEventHandler);
        }

        /// <summary>
        /// Initializes the TridionRazorTemplate.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="package"></param>
        public void Initialize(Engine engine, Package package, Template template, List<string> assemblyReferences)
        {
            _engine = engine;
            _package = package;
            _tridionHelper = new TridionUtilities(engine, package);
            _razorTBB = template;
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
        /// Gets a list of ComponentPresentationModels from the Component Presentations on the page.
        /// </summary>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        public List<ComponentPresentationModel> GetComponentPresentationsBySchema(params string[] schemaNames)
        {
            List<ComponentPresentationModel> componentPresentations = new List<ComponentPresentationModel>();
            int i = 0;
            foreach (ComponentPresentationModel cp in ComponentPresentations)
            {
                if (schemaNames.Contains((string)cp.Component.Schema.Title))
                {
                    var cpm = new ComponentPresentationModel(_engine, (Component)cp.Component.TridionObject, cp.Template);
                    cpm.Index = i++;

                    componentPresentations.Add(cpm);
                }
            }

            if (componentPresentations.Count > 0)
            {
                componentPresentations.Last().IsLast = true;
            }

            return componentPresentations;
        }

        /// <summary>
        /// Gets a list of ComponentPresentationModels from the Component Presentations on the page. Callable from a PageTemplate.
        /// </summary>
        /// <param name="templateName">The template's name to filter by.</param>
        /// <returns>A list of ComponentPresentationModels that match the template's name.</returns>
        public List<ComponentPresentationModel> GetComponentPresentationsByTemplate(params string[] templateNames)
        {
            List<ComponentPresentationModel> componentPresentations = new List<ComponentPresentationModel>();
            int i = 0;
            foreach (ComponentPresentationModel cp in ComponentPresentations)
            {
                if (templateNames.Contains(cp.Template.Title))
                {
                    var cpm = new ComponentPresentationModel(_engine, (Component)cp.Component.TridionObject, cp.Template);
                    cpm.Index = i++;

                    componentPresentations.Add(cpm);
                }
            }

            if (componentPresentations.Count > 0)
            {
                componentPresentations.Last().IsLast = true;
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
            return RenderComponentField(fieldExpression, fieldIndex, true);
        }

        /// <summary>
        /// Renders a component field embedded in a tcdl:ComponenField tag.
        /// </summary>
        /// <param name="fieldExpression">Reference to a field relative to the context component. For example Fields.MyEmbeddedSchema.MyField</param>
        /// <param name="fieldIndex">Index of this value for multi-valued fields starting at 1. Single-value fields simply use 1.</param>
        /// <returns>The rendered component field.</returns>
        public string RenderComponentField(string fieldExpression, int fieldIndex, bool renderTcdlTagOnError)
        {
            try
            {
                return _builtInFunctions.RenderComponentField(fieldExpression, fieldIndex);
            }
            catch
            {
                return renderTcdlTagOnError ? "<tcdl:ComponentField name=\"" + fieldExpression + "\" index=\"" + fieldIndex + "\"></tcdl:ComponentField>" : String.Empty;
            }
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
            return RenderComponentField(fieldExpression, fieldIndex, value, true);
        }

        /// <summary>
        /// Renders a component field embedded in a tcdl:ComponenField tag.
        /// </summary>
        /// <param name="fieldExpression">Reference to a field relative to the context component. For example Fields.MyEmbeddedSchema.MyField</param>
        /// <param name="fieldIndex">Index of this value for multi-valued fields starting at 1. Single-value fields simply use 1.</param>
        /// <param name="value">Instead of looking up the field value, passthrough a value, eg IMG/A tags</param>
        /// <returns>The rendered component field.</returns>
        public string RenderComponentField(string fieldExpression, int fieldIndex, string value, bool renderTcdlTagOnError)
        {
            try
            {
                return _builtInFunctions.RenderComponentField(fieldExpression, fieldIndex, value);
            }
            catch
            {
                return renderTcdlTagOnError ? "<tcdl:ComponentField name=\"" + fieldExpression + "\" index=\"" + fieldIndex + "\"></tcdl:ComponentField>" : String.Empty;
            }
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
            return RenderComponentField(fieldExpression, fieldIndex, htmlEncodeResult, resolveHtmlAsRTFContent, true);
        }

        /// <summary>
        /// Renders a component field embedded in a tcdl:ComponenField tag.
        /// </summary>
        /// <param name="fieldExpression">Reference to a field relative to the context component. For example Fields.MyEmbeddedSchema.MyField</param>
        /// <param name="fieldIndex">Index of this value for multi-valued fields starting at 1. Single-value fields simply use 1.</param>
        /// <param name="htmlEncodeResult">Html encodes the result if set to true.</param>
        /// <param name="resolveHtmlAsRTFContent">Resolves HTML as RTF content if set to true.</param>
        /// <returns>The rendered component field.</returns>
        public string RenderComponentField(string fieldExpression, int fieldIndex, bool htmlEncodeResult, bool resolveHtmlAsRTFContent, bool renderTcdlTagOnError)
        {
            try
            {
                return _builtInFunctions.RenderComponentField(fieldExpression, fieldIndex, htmlEncodeResult, resolveHtmlAsRTFContent);
            }
            catch
            {
                return renderTcdlTagOnError ? "<tcdl:ComponentField name=\"" + fieldExpression + "\" index=\"" + fieldIndex + "\"></tcdl:ComponentField>" : String.Empty;
            }
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
        /// Renders all component presentations.
        /// </summary>
        /// <returns></returns>
        public string RenderComponentPresentations()
        {
            string rendering = String.Empty;
            foreach (ComponentPresentationModel cp in ComponentPresentations)
            {
                rendering += cp.RenderComponentPresentation();
            }

            return rendering;
        }

        /// <summary>
        /// Renders all component presentations.
        /// </summary>
        /// <returns></returns>
        public string RenderComponentPresentationsByTemplate(params string[] templateNames)
        {
            string rendering = String.Empty;
            foreach (ComponentPresentationModel cp in ComponentPresentations)
            {
                if (templateNames.Contains(cp.Template.Title))
                    rendering += cp.RenderComponentPresentation();
            }
            
            return rendering;
        }

        /// <summary>
        /// Executes the template.
        /// </summary>
        public override void Execute()
        {

        }

        #region Logging Methods

        public string Debug(string message)
        {
            Log.Debug(message);
            return String.Empty;
        }

        public string Error(string message)
        {
            Log.Error(message);
            return String.Empty;
        }

        public string Info(string message)
        {
            Log.Info(message);
            return String.Empty;
        }

        public string Warning(string message)
        {
            Log.Warning(message);
            return String.Empty;
        }

        #endregion

        /// <summary>
        /// Resolves unresolved assembly references.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
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
