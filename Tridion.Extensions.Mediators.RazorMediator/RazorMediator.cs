using System.CodeDom.Compiler;
using System.Collections.Generic;
using Tridion.ContentManager;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.Templating;
using Tridion.ContentManager.Templating.Configuration;
using Tridion.Extensions.Mediators.Razor.Configuration;
using Tridion.Extensions.Mediators.Razor.Templating;

namespace Tridion.Extensions.Mediators.Razor
{
    /// <summary>
    /// The Razor Templating Mediator. Allows to use Razor templates as Template Building Blocks.
    /// </summary>
    public class RazorMediator : IMediator
    {
        /// <summary>
        /// The Tridion Templating Logger instance.
        /// </summary>
        private TemplatingLogger _logger;

        /// <summary>
        /// The namespaces to include in the razor templates.
        /// </summary>
        private List<string> _namespaces = new List<string>();

        /// <summary>
        /// The assembly references to add to the razor templates.
        /// </summary>
        private List<string> _references = new List<string>();

        /// <summary>
        /// The cache time in seconds to cache the razor templates.
        /// </summary>
        private int _cacheTime = 600;

        /// <summary>
        /// The razor.mediator configuration section.
        /// </summary>
        private RazorMediatorConfigurationSection _config;

        /// <summary>
        /// Constructor
        /// </summary>
        public RazorMediator()
        {
            _logger = TemplatingLogger.GetLogger(this.GetType());
        }

        #region IMediator Members

        /// <summary>
        /// Configure the mediator object, based on the configuration element of the mediator. 
        /// </summary>
        /// <param name="configuration">The MediatorElement configuration.</param>
        public void Configure(MediatorElement configuration)
        {
            _config = RazorMediatorConfigurationSection.GetConfiguration();

            _cacheTime = _config.CacheTime;

            foreach (NamespaceElement nameSpace in _config.Namespaces)
            {
                _namespaces.Add(nameSpace.Namespace);
            }

            foreach (AssemblyElement assembly in _config.Assemblies)
            {
                _references.Add(assembly.Assembly);
            }
        }

        /// <summary>
        /// Execute the specified template in the context of the given package. The mediator is expected to be able to handle the template, 
        /// as it is called based on the template types configuration.  
        /// </summary>
        /// <param name="engine">The Tridion engine invoking the templating.</param>
        /// <param name="template">The Tridion Template to execute.</param>
        /// <param name="package">The Tridion package with both the inputs and the outputs of the template.</param>
        public void Transform(Engine engine, Template template, Package package)
        {
            bool loaded = typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly != null;
            
            try
            {
                IRazorTemplateGenerator generator = new RazorTemplateGenerator();

                generator.ClearCache(_cacheTime);
                generator.RegisterTemplate<TridionRazorTemplate>(template.Id.ToString(), template.Content, _namespaces, template.RevisionDate);
                generator.CompileTemplates(_references);

                TridionRazorTemplate razor = generator.GenerateTemplate<TridionRazorTemplate>(template.Id.ToString());

                razor.Initialize(engine, package, _references);
                razor.Execute();

                string output = razor.ToString();

                if (_config.ExtractBinaries)
                    output = ExtractBinaries(output, engine, package);
                
                package.PushItem(Package.OutputName, package.CreateStringItem(ContentType.Html, output));

            }
            catch (TemplateCompileException ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
                foreach (CompilerError error in ex.Errors)
                {
                    _logger.Error(error.ErrorText);
                }
            }
        }

        /// <summary>
        /// Extracts binaries from the template and adds them to the package for processing.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="engine"></param>
        /// <param name="package"></param>
        /// <returns></returns>
        private string ExtractBinaries(string output, Engine engine, Package package)
        {
            IExtractBinariesContentWrapper contentWrapper = new ExtractBinariesFromText(output);
            foreach (LinkReferenceWrapper linkAttribute in contentWrapper.GetLinkAttributes())
            {
                string pathAttributeValue = contentWrapper.GetAttributeValue(linkAttribute);
                TcmUri targetItemUri = null;
                if (TcmUri.IsValid(pathAttributeValue))
                {
                    // Attribute value is TCM URI, now localize to currect context publication.
                    targetItemUri = engine.LocalizeUri(new TcmUri(pathAttributeValue));
                }

                if (targetItemUri != null)
                {
                    Component targetItem = engine.GetObject(targetItemUri) as Component;
                    if ((targetItem != null) && (targetItem.ComponentType == ComponentType.Multimedia))
                    {
                        Item binaryItem = package.CreateMultimediaItem(targetItemUri);
                        string itemName;
                        binaryItem.Properties.TryGetValue(Item.ItemPropertyFileName, out itemName);

                        Item existingItem = package.GetByName(itemName);
                        if (
                            existingItem == null ||
                            !existingItem.Properties[Item.ItemPropertyTcmUri].Equals(targetItemUri) ||
                            !existingItem.Equals(binaryItem) // Ensure that a transformed item is not considered the same
                        )
                        {
                            // _logger.Debug(string.Format("Image {0} ({1}) unique, adding to package", itemName, targetItemUri));
                            package.PushItem(itemName, binaryItem);
                        }
                        else
                        {
                            // _logger.Debug(string.Format("Image {0} ({1}) already present in package, not adding again", itemName, targetItemUri));
                        }

                        contentWrapper.ProcessLinkChange(linkAttribute, targetItemUri);
                    }
                }
            }

            return contentWrapper.Content.ToString();
        }

        #endregion
    }
}