using System;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.Templating;

namespace Tridion.Extensions.Mediators.Razor.Utilities
{
    public class TridionUtilities
    {
        private Engine _engine;

        private TemplatingLogger _logger;

        private Package _package;

        /// <summary>
        /// Gets the Tridion Templating Engine.
        /// </summary>
        public Engine Engine
        {
            get
            {
                return _engine;
            }
        }

        /// <summary>
        /// The Tridion templating logger.
        /// </summary>
        protected TemplatingLogger Logger
        {
            get
            {
                if (_logger == null)
                    _logger = TemplatingLogger.GetLogger(this.GetType());

                return _logger;
            }
        }

        /// <summary>
        /// Gets the Tridion Package.
        /// </summary>
        public Package Package
        {
            get
            {
                return _package;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="package"></param>
        public TridionUtilities(Engine engine, Package package)
        {
            _engine = engine;
            _package = package;
        }

        /// <summary>
        /// Gets the Tridion Component object that is defined in the package for this template.
        /// </summary>
        /// <returns></returns>
        public Component GetComponent()
        {
            Item component = _package.GetByType(ContentType.Component);

            if (component == null)
                throw new Exception("No Component ContentType Found In Package");

            return (Component)_engine.GetObject(component.GetAsSource().GetValue("ID"));
        }

        /// <summary>
        /// Gets the Tridion Page object that is defined in the package for this template. 
        /// </summary>
        /// <returns></returns>
        public Page GetPage()
        {
            Page page;
            Item pageItem = _package.GetByType(ContentType.Page);

            if (pageItem == null)
            {
                // No page content type item found (ie Component Template).  Try to get Page from context item.
                try
                {
                    page = (Page)_engine.PublishingContext.RenderContext.ContextItem; // Gets Page from Component Template
                }
                catch
                {
                    // No page at all found (ie Dynamic Component Template)
                    Logger.Warning("No Page Object Found In Package");
                    return null;
                }
            }
            else
            {
                page = (Page)_engine.GetObject(pageItem.GetAsSource().GetValue("ID"));
            }

            return page;
        }

        /// <summary>
        /// Gets the Component Presentations from the "Components" package item if available.
        /// </summary>
        /// <returns>A list of component presentations.</returns>
        public IComponentPresentationList GetComponentPresentations()
        {
            Item componentsItem = _package.GetByName(Package.ComponentsName);
            IComponentPresentationList componentPresentations = ComponentPresentationList.FromXml(componentsItem.GetAsString());

            return componentPresentations;
        }

        /// <summary>
        /// Gets the current context Publication.
        /// </summary>
        /// <returns>The current context Publication.</returns>
        public Publication GetPublication()
        {
            Repository repository;
            if (_package.GetByType(ContentType.Page) != null)
            {
                Page page = GetPage();
                repository = page.ContextRepository;
            }
            else
            {
                Component component = GetComponent();
                repository = component.ContextRepository;
            }
            if (repository is Publication)
            {
                return (Publication)repository;
            }
            return null;
        }
    }
}
