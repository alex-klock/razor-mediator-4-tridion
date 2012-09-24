using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.Templating;

namespace Tridion.Extensions.Mediators.Razor.Models
{
    public class PageTemplateModel : AbstractRepositoryLocalObject<PageTemplate>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="template"></param>
        public PageTemplateModel(Engine engine, PageTemplate template)
            : base(engine, template)
        {
            
        }

        /// <summary>
        /// Gets the file extension used by pages of this page template type.
        /// </summary>
        public string FileExtension
        {
            get
            {
                return _tridionObject.FileExtension;
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
