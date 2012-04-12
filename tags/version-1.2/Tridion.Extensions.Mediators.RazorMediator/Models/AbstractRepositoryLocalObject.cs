using System.Dynamic;
using Tridion.ContentManager;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.ContentManagement.Fields;
using Tridion.ContentManager.Templating;

namespace Tridion.Extensions.Mediators.Razor.Models
{
    public abstract class AbstractRepositoryLocalObject<T> : DynamicObject where T: RepositoryLocalObject
    {
        /// <summary>
        /// The Tridion templating engine.
        /// </summary>
        protected Engine _engine;

        /// <summary>
        /// The Tridion object's metadata fields.
        /// </summary>
        private DynamicItemFields _metadata;

        /// <summary>
        /// The organizational item that contains this Tridion object.
        /// </summary>
        private dynamic _organizationItem;

        /// <summary>
        /// The wrapped Tridion object.
        /// </summary>
        protected T _tridionObject;

        /// <summary>
        /// The Item's Publication.
        /// </summary>
        private dynamic _publication;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">The Tridion templating engine.</param>
        /// <param name="tridionObject">The tridion object being wrapped.</param>
        public AbstractRepositoryLocalObject(Engine engine, T tridionObject)
        {
            _engine = engine;
            _tridionObject = tridionObject;
        }

        /// <summary>
        /// Gets the Metadata fields.
        /// </summary>
        public virtual dynamic Fields
        {
            get { return MetaData; }
        }

        /// <summary>
        /// Gets the Tcm Uri.
        /// </summary>
        public TcmUri Id
        {
            get { return _tridionObject.Id; }
        }

        /// <summary>
        /// Gets the Tcm Uri.
        /// </summary>
        public TcmUri ID
        {
            get { return _tridionObject.Id; }
        }

        /// <summary>
        /// Gets whether or not the item is localized in the current context Publication.
        /// </summary>
        public bool IsLocalized
        {
            get { return _tridionObject.IsLocalized; }
        }

        /// <summary>
        /// Gets whether or not the item is shared in the current context Publication.
        /// </summary>
        public bool IsShared
        {
            get { return _tridionObject.IsShared; }
        }

        /// <summary>
        /// Gets the Metadata fields.
        /// </summary>
        public dynamic Metadata
        {
            get { return MetaData; }
        }

        /// <summary>
        /// Gets the Metadata fields.
        /// </summary>
        public dynamic MetaData
        {
            get
            {
                if (_metadata == null)
                {
                    if (_tridionObject.MetadataSchema == null)
                    {
                        _metadata = new DynamicItemFields(_engine, null);
                    }
                    else
                    {
                        ItemFields fields = new ItemFields(_tridionObject.Metadata, _tridionObject.MetadataSchema);
                        _metadata = new DynamicItemFields(_engine, fields);
                    }
                }

                return _metadata;
            }
        }

        /// <summary>
        /// Gets the organizational item that contains this Tridion object.
        /// </summary>
        public dynamic OrganizationalItem
        {
            get
            {
                if (_organizationItem == null && _tridionObject.OrganizationalItem != null)
                {
                    if (_tridionObject.OrganizationalItem is Category)
                        _organizationItem = _tridionObject.OrganizationalItem as Category;
                    else if (_tridionObject.OrganizationalItem is Folder)
                        _organizationItem = new FolderModel(_engine, _tridionObject.OrganizationalItem as Folder);
                    else if (_tridionObject.OrganizationalItem is StructureGroup)
                        _organizationItem = new StructureGroupModel(_engine, _tridionObject.OrganizationalItem as StructureGroup);
                    else if (_tridionObject.OrganizationalItem is VirtualFolder)
                        _organizationItem = _tridionObject.OrganizationalItem as VirtualFolder;
                }
                return _organizationItem;
            }
        }

        /// <summary>
        /// Gets the full path to the item.
        /// </summary>
        public string Path
        {
            get { return _tridionObject.Path; }
        }

        /// <summary>
        /// Gets the Item's Context Repository (Publication).
        /// </summary>
        public dynamic Publication
        {
            get
            {
                if (_publication == null)
                    _publication = new PublicationModel(_engine, (Publication)_tridionObject.ContextRepository);

                return _publication;
            }
        }

        /// <summary>
        /// Gets the Title.
        /// </summary>
        public string Title
        {
            get { return _tridionObject.Title; }
        }

        /// <summary>
        /// Gets the underlying Tridion Obect.
        /// </summary>
        public T TridionObject
        {
            get { return _tridionObject; }
        }

        /// <summary>
        /// Gets the WebDAV URL of the item.
        /// </summary>
        public string WebDavUrl
        {
            get { return _tridionObject.WebDavUrl; }
        }

        /// <summary>
        /// Returns the Title.
        /// </summary>
        public override string ToString()
        {
            return _tridionObject.Title;
        }

        /// <summary>
        /// Attempts to get a member of the instance.
        /// </summary>
        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            return Fields.TryGetMember(binder, out result);
        }
    }
}
