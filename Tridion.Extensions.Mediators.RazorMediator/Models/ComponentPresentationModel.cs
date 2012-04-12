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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="componentID"></param>
        /// <param name="templateID"></param>
        public ComponentPresentationModel(Engine engine, TcmUri componentID, TcmUri templateID)
            : this(engine, componentID, templateID, 0)
        {

        }
        
        /// <summary>
        /// Constrctor
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="componentID"></param>
        /// <param name="templateID"></param>
        /// <param name="index"></param>
        public ComponentPresentationModel(Engine engine, TcmUri componentID, TcmUri templateID, int index)
        {
            _engine = engine;
            _componentID = componentID;
            _templateID = templateID;
            Index = index;
        }

        public ComponentPresentationModel(Engine engine, Component component, ComponentTemplate template)
            : this(engine, component, template, 0)
        {

        }

        public ComponentPresentationModel(Engine engine, Component component, ComponentTemplate template, int index)
        {
            _engine = engine;
            _component = new ComponentModel(engine, component);
            _template = template;

            _componentID = component.Id;
            _templateID = template.Id;
            Index = index;
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

        public int Index
        {
            get;
            set;
        }

        public bool IsFirst
        {
            get
            {
                return Index == 0;
            }
        }

        public bool IsLast
        {
            get;
            set;
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
