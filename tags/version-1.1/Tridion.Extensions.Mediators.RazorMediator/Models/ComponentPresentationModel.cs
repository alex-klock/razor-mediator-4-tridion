using System;
using Tridion.ContentManager;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.Templating;

namespace Tridion.Extensions.Mediators.Razor.Models
{
    /// <summary>
    /// Represents a Tridion Component Presentation.
    /// </summary>
    public class ComponentPresentationModel
    {
        private Engine _engine;
        private TcmUri _componentID;
        private TcmUri _templateID;
        private dynamic _component;
        private ComponentTemplate _template;

        public ComponentPresentationModel(Engine engine, TcmUri componentID, TcmUri templateID)
        {
            _engine = engine;
            _componentID = componentID;
            _templateID = templateID;
        }

        public ComponentPresentationModel(Engine engine, Component component, ComponentTemplate template)
        {
            _engine = engine;
            _component = new ComponentModel(engine, component);
            _template = template;

            _componentID = component.Id;
            _templateID = template.Id;
        }

        public string ComponentID
        {
            get
            {
                return _componentID;
            }
        }

        public string ComponentUri
        {
            get
            {
                return _componentID;
            }
        }

        public string TemplateID
        {
            get
            {
                return _templateID;
            }
        }

        public string TemplateUri
        {
            get
            {
                return _templateID;
            }
        }

        public dynamic Component
        {
            get
            {
                if (_component == null)
                {
                    _component = new ComponentModel(_engine, _componentID);
                }

                return _component;
            }
        }

        public ComponentTemplate Template
        {
            get
            {
                if (_template == null)
                {
                    _template = _engine.GetSession().GetObject(_templateID) as ComponentTemplate;
                    if (_template == null)
                    {
                        throw new Exception(String.Format("No ComponentTemplate With ID Of '{0}' Found", _templateID));
                    }
                }

                return _template;
            }
        }

        /// <summary>
        /// Renders the ComponentPresentation.
        /// </summary>
        /// <returns>The rendered component presentation.</returns>
        public string RenderComponentPresentation()
        {
            return _engine.RenderComponentPresentation(_componentID, _templateID);
        }
    }
}
