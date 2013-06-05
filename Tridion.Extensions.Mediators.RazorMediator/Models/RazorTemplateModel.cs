using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.Templating;

namespace Tridion.Extensions.Mediators.Razor.Models
{
    public class RazorTemplateModel : AbstractRepositoryLocalObject<Template>
    {
        public RazorTemplateModel(Engine engine, Template template)
            : base(engine, template)
        {

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
