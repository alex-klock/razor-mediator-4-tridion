using System.CodeDom.Compiler;
using System.Collections.Generic;
using Tridion.ContentManager;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.Templating;
using Tridion.ContentManager.Templating.Configuration;
using Tridion.Extensions.Mediators.Razor.Configuration;
using Tridion.Extensions.Mediators.Razor.Templating;
using System.Text.RegularExpressions;
using System.IO;

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
            RazorHandler handler = new RazorHandler(template.Id.ToString(), template.WebDavUrl, template.Content, template);
            handler.Initialize();

            string output = handler.CompileAndExecute(template.RevisionDate, engine, package);

            if (_config.ExtractBinaries)
                output = ExtractBinaries(output, engine, package);
                
            package.PushItem(Package.OutputName, package.CreateStringItem(ContentType.Html, output));
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
                // _logger.Debug("Path: " + pathAttributeValue);
                TcmUri targetItemUri = null;
                if (TcmUri.IsValid(pathAttributeValue))
                {
                    // Attribute value is TCM URI, now localize to currect context publication.
                    targetItemUri = engine.LocalizeUri(new TcmUri(pathAttributeValue));
                }

                if (targetItemUri != null || pathAttributeValue.StartsWith("/webdav/"))
                {
                    Component targetItem = engine.GetObject(targetItemUri ?? pathAttributeValue) as Component;
                    if (targetItemUri == null && targetItem != null)
                    {
                        targetItemUri = engine.LocalizeUri(targetItem.Id);
                    }

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