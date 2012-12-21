using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.Templating;
using Tridion.ContentManager.ContentManagement;

namespace Tridion.Extensions.Mediators.Razor.Models
{
    public class ComponentTemplateModel : AbstractRepositoryLocalObject<ComponentTemplate>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="template"></param>
        public ComponentTemplateModel(Engine engine, ComponentTemplate template)
            : base(engine, template)
        {
            
        }

        /// <summary>
        /// Gets whether or not this template is allowed on page.
        /// </summary>
        public bool AllowOnPage
        {
            get
            {
                return _tridionObject.AllowOnPage;
            }
        }

        /// <summary>
        /// Get or set whether the Component Template renders "dynamic" Component Presentations. 
        /// </summary>
        public bool IsRepositoryPublishable
        {
            get
            {
                return _tridionObject.IsRepositoryPublishable;
            }
        }

        /// <summary>
        /// Get or set the format of the rendered Component Presentation. 
        /// </summary>
        public string OutputFormat
        {
            get
            {
                return _tridionObject.OutputFormat;
            }
        }

        /// <summary>
        /// Gets the parameter schema used by this page template.
        /// </summary>
        public Schema ParameterSchema
        {
            get
            {
                return _tridionObject.ParameterSchema;
            }
        }
    }
}
