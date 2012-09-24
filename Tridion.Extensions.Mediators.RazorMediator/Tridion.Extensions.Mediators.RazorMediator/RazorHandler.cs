using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Tridion.ContentManager;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.Templating;
using Tridion.Extensions.Mediators.Razor.Configuration;
using Tridion.Extensions.Mediators.Razor.Templating;
using System.Web;

namespace Tridion.Extensions.Mediators.Razor
{
    /// <summary>
    /// Handles the retrieval, configuration, compiling, and execution of the Razor code.
    /// </summary>
    public class RazorHandler
    {
        /// <summary>
        /// The razor.mediator configuration section.
        /// </summary>
        private RazorMediatorConfigurationSection _config;

        /// <summary>
        /// The razor template generator.
        /// </summary>
        private IRazorTemplateGenerator _generator;

        /// <summary>
        /// The namespaces to include in the razor templates.
        /// </summary>
        private List<string> _namespaces = new List<string>();

        /// <summary>
        /// The assembly references to add to the razor templates.
        /// </summary>
        private List<string> _assemblies = new List<string>();

        /// <summary>
        /// The locking mechanism for thread safety.
        /// </summary>
        private static object _lock = new object();

        /// <summary>
        /// The template tcm uri.
        /// </summary>
        private string _templateID;

        /// <summary>
        /// The template content.
        /// </summary>
        private string _templateContent;

        /// <summary>
        /// The Tridion Session.
        /// </summary>
        private Session _session;

        /// <summary>
        /// The Tridion template.
        /// </summary>
        private Template _template;

        /// <summary>
        /// The template's web dav url.
        /// </summary>
        private string _webDavUrl;

        private TemplatingLogger _logger;

        /// <summary>
        /// Gets the razor.mediator config section.
        /// </summary>
        public RazorMediatorConfigurationSection Config
        {
            get
            {
                return _config;
            }
        }

        /// <summary>
        /// Gets the Tridion Session, impersonated with an admin user. Used to retrieve templates for imports.
        /// </summary>
        public Session Session
        {
            get
            {
                if (_session == null)
                {
                    if (String.IsNullOrEmpty(_config.AdminUser))
                    {
                        throw new Exception("Attempting to create Session - razor config attribute 'adminUser' is required to use this feature");
                    }
                    _session = new Session(_config.AdminUser);
                }
                return _session;
            }
        }

        /// <summary>
        /// Gets the Tridion template being executed.
        /// </summary>
        public Template Template
        {
            get
            {
                if (_template == null)
                {
                    _template = Session.GetObject(_templateID) as Template;
                }
                return _template;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="templateID">The tcm uri of the template.</param>
        /// <param name="templateContent">The content of the template</param>
        public RazorHandler(string templateID, string webDavUrl, string templateContent)
        {
            _templateID = templateID;
            _templateContent = templateContent;
            _webDavUrl = webDavUrl;
        }

        /// <summary>
        /// Configure the mediator object, based on the configuration element of the mediator. 
        /// </summary>
        /// <param name="configuration">The MediatorElement configuration.</param>
        public void Initialize()
        {
            _logger = TemplatingLogger.GetLogger(this.GetType());
            _config = RazorMediatorConfigurationSection.GetConfiguration();
            _generator = new RazorTemplateGenerator();

            foreach (NamespaceElement nameSpace in _config.Namespaces)
            {
                _namespaces.Add(nameSpace.Namespace);
            }

            foreach (AssemblyElement assembly in _config.Assemblies)
            {
                _assemblies.Add(assembly.Assembly);
            }
        }

        /// <summary>
        /// Only compiles the razor code.  Does not execute.
        /// </summary>
        /// <param name="revisionDate"></param>
        public void CompileOnly(DateTime revisionDate)
        {
            lock (_lock)
            {
                Compile(revisionDate);
            }
        }

        /// <summary>
        /// Compiles the razor code.
        /// </summary>
        /// <param name="revisionDate">The date the template has been modified.  If the revision date is newer than what's in the cache, will update. Otherwise, will not compile and use what's in cache.</param>
        private void Compile(DateTime revisionDate)
        {
            bool loaded = typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly != null;

            ImportIncludes();
            CleanupExtraImports();

            _generator.ClearCache(_config.CacheTime);
            _generator.RegisterTemplate<TridionRazorTemplate>(_templateID, _templateContent, _namespaces, revisionDate);

            try
            {
                _generator.CompileTemplates(_assemblies);
            }
            catch (TemplateCompileException ex)
            {
                string errorMessage = String.Empty;
                foreach (CompilerError error in ex.Errors)
                {
                    if (error.IsWarning == false)
                        errorMessage += String.Format(
                            "{0}{6}: {1} {2}Line {3} Column {4}: {5} {2}",
                            error.ErrorNumber,
                            error.ErrorText,
                            Environment.NewLine,
                            error.Line,
                            error.Column,
                            ex.GetSourceLine(error.Line),
                            error.IsWarning ? " (Warning)" : String.Empty);
                }
                errorMessage += "Stack Trace: " + Environment.NewLine + ex.StackTrace;
                _generator.RemoveTemplate(_templateID);

                throw new Exception("TemplateCompileException: " + errorMessage);
            }
        }

        /// <summary>
        /// Compiles and executes the razor template and returns the rendered string.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="package"></param>
        /// <returns></returns>
        public string CompileAndExecute(DateTime revisionDate, Engine engine, Package package)
        {
            TridionRazorTemplate razor;
            lock (_lock)
            {
                Compile(revisionDate);
                razor = _generator.GenerateTemplate<TridionRazorTemplate>(_templateID);
            }
            razor.Initialize(engine, package, Template, _assemblies);
            razor.Execute();

            return razor.ToString().Trim();
        }

        /// <summary>
        /// Remove extra import statements. (Normally from imports containing imports.)
        /// </summary>
        private void CleanupExtraImports()
        {
            Regex regex = new Regex(@"@importRazor\(""(?<path>[^""]*)""\)");
            foreach (Match match in regex.Matches(_templateContent))
            {
                _templateContent = _templateContent.Replace(match.Value, String.Empty);
            }
        }

        /// <summary>
        /// Imports includes found in the regex statements as well as the config files
        /// </summary>
        /// <param name="templateContent"></param>
        /// <param name="engine"></param>
        /// <returns></returns>
        private void ImportIncludes()
        {
            foreach (ImportElement import in _config.Imports)
            {
                bool importTemplate = true;
                if (!String.IsNullOrEmpty(import.Publications))
                {
                    string[] publications = import.Publications.Split(',');

                    string publicationTitle = Template != null ? Template.OwningRepository.Title : GetPublicationTitleFromWebDavUrl();
                    if (!publications.Contains(publicationTitle))
                    {
                        importTemplate = false;
                    }
                }

                if (importTemplate)
                    _templateContent = GetImportTemplateContent(import.Import) + Environment.NewLine + _templateContent;
            }
            
            Regex regex = new Regex(@"@importRazor\(""(?<path>[^""]*)""\)");
            foreach (Match match in regex.Matches(_templateContent))
            {
                string path = match.Groups["path"].Value;
                _templateContent = _templateContent.Replace(match.Value, GetImportTemplateContent(path));
            }
        }

        /// <summary>
        /// Gets a list of references from imports.
        /// </summary>
        /// <returns></returns>
        public List<string> GetImportReferences()
        {
            List<string> references = new List<string>();

            if (_config.ImportSettings.IncludeConfigWhereUsed)
            {
                foreach (ImportElement import in _config.Imports)
                {
                    bool importTemplate = true;
                    if (!String.IsNullOrEmpty(import.Publications))
                    {
                        string[] publications = import.Publications.Split(',');

                        string publicationTitle = Template != null ? Template.OwningRepository.Title : GetPublicationTitleFromWebDavUrl();
                        if (!publications.Contains(publicationTitle))
                        {
                            importTemplate = false;
                        }
                    }

                    if (importTemplate)
                    {
                        string path = import.Import;

                        if (path.Contains("\\"))
                        {
                            continue;
                        }

                        references.Add(path);
                    }
                }
            }

            if (_config.ImportSettings.IncludeImportWhereUsed)
            {
                Regex regex = new Regex(@"@importRazor\(""(?<path>[^""]*)""\)");
                foreach (Match match in regex.Matches(_templateContent))
                {
                    string path = match.Groups["path"].Value;

                    if (path.Contains("\\"))
                    {
                        continue;
                    }

                    references.Add(path);
                }
            }

            return references;
        }

        /// <summary>
        /// Gets an imported template's content. Works with tcm uri's, web dav urls, or physical file paths.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <param name="engine"></param>
        /// <returns></returns>
        private string GetImportTemplateContent(string path)
        {
            TcmUri templateID = new TcmUri(_templateID);

            if (path.ToLower().StartsWith("tcm:") || path.ToLower().StartsWith("/webdav/") || !path.Contains("\\"))
            {
                if (!path.ToLower().StartsWith("tcm:") && !path.ToLower().StartsWith("/webdav/"))
                {
                    path = GetRelativeImportPath(path);
                }

                TemplateBuildingBlock template;
                try
                {
                    template = Session.GetObject(path) as TemplateBuildingBlock;

                }
                catch
                {
                    _logger.Warning("Error import of '" + path + "'.");
                    return String.Empty;
                }

                if (template == null)
                {
                    _logger.Warning("Import of '" + path + "' not found.");
                    return String.Empty;
                }

                _logger.Debug("Comaring import template " + template.Id + " to razor tbb ID " + templateID);
                // Get local copy of the imported template if possible.
                if (template.Id.PublicationId != templateID.PublicationId)
                {
                    try
                    {
                        template = (TemplateBuildingBlock)Session.GetObject(TemplateUtilities.CreateTcmUriForPublication(templateID.PublicationId, template.Id));
                    }
                    catch
                    {
                        _logger.Warning("Error trying to get local copy of template '" + template.Id + "' for Publication ID '" + templateID.PublicationId + "'");
                    }
                }

                // Don't import itself
                if (template.Id.GetVersionlessUri().Equals(templateID.GetVersionlessUri()))
                {
                    return String.Empty;
                }

                return template.Content;
            }
            else
            {
                // If its a file path, get the contents of the file and return.
                return File.ReadAllText(path);
            }
        }

        /// <summary>
        /// Gets the Publication's title based on the web dav url.
        /// </summary>
        /// <returns></returns>
        private string GetPublicationTitleFromWebDavUrl()
        {
            string[] webDavParts = _webDavUrl.Split('/');
            return HttpUtility.UrlDecode(webDavParts[2]);
        }

        /// <summary>
        /// Gets the relative import path of an import.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetRelativeImportPath(string path)
        {
            List<string> templatePathParts = _webDavUrl.Split('/').ToList();
            templatePathParts.RemoveAt(templatePathParts.Count - 1);

            string[] pathParts = path.Split('/');

            if (pathParts.Length == 1)
            {
                // If there's no directory separator, path is in the same directory as this template.
                templatePathParts.Add(path);
            }
            else
            {
                foreach (string part in pathParts)
                {
                    if (part.Trim().Length == 0 || part.Equals("."))
                    {
                        // Ignore?
                    }
                    else if (part.Equals(".."))
                    {
                        templatePathParts.RemoveAt(templatePathParts.Count - 1);
                    }
                    else
                    {
                        templatePathParts.Add(part);
                    }
                }
            }

            return String.Join("/", templatePathParts);
        }
    }
}
