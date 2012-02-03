using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.ContentManagement.Fields;
using Tridion.ContentManager.Templating;

namespace Tridion.Extensions.Mediators.Razor.Models
{
    /// <summary>
    /// Represents a Tridion Component.
    /// </summary>
    public class ComponentModel : AbstractRepositoryLocalObject<Component>
    {
        /// <summary>
        /// The Component's Fields.
        /// </summary>
        private dynamic _fields;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">The Tridion templating engine.</param>
        /// <param name="component">The component to wrap.</param>
        public ComponentModel(Engine engine, Component component) : base(engine, component)
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">The Tridion templating engine.</param>
        /// <param name="componentTcmUri">The component's tcm uri.</param>
        public ComponentModel(Engine engine, string componentTcmUri) : base(engine, engine.GetObject(componentTcmUri) as Component)
        {
            
        }

        /// <summary>
        /// Gets the Component's Schema.
        /// </summary>
        public Schema Schema 
        {
            get { return _tridionObject.Schema; } 
        }

        /// <summary>
        /// Gets the Component's Fields.
        /// </summary>
        public override dynamic Fields 
        {
            get
            {
                if (_fields == null)
                {
                    ItemFields itemFields = new ItemFields(_tridionObject.Content, _tridionObject.Schema);
                    _fields = new DynamicItemFields(_engine, itemFields);
                }

                return _fields;
            }
        }

        /// <summary>
        /// Gets the Component's Folder.
        /// </summary>
        public FolderModel Folder
        {
            get
            {
                return OrganizationalItem;
            }
        }

        /// <summary>
        /// Outputs the Tcm Uri of the component.
        /// </summary>
        public override string ToString()
        {
            return ID.ToString();
        }

        /// <summary>
        /// Attempts to access the Fields first, then Metadata.
        /// </summary>
        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            bool found = Fields.TryGetMember(binder, out result);
            if (!found)
                found = Metadata.TryGetMember(binder, out result);

            return found;
        }
    }
}
