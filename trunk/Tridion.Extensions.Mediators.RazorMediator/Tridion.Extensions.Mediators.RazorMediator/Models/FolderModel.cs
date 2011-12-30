using System.Collections.Generic;
using Tridion.ContentManager;
using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.Templating;

namespace Tridion.Extensions.Mediators.Razor.Models
{
    /// <summary>
    /// Represents a Tridion Folder
    /// </summary>
    public class FolderModel : AbstractRepositoryLocalObject<Folder>
    {
        /// <summary>
        /// The child components of this Folder (non-recursive).
        /// </summary>
        private List<ComponentModel> _components;

        /// <summary>
        /// The child folders of this Folder (non-recursive).
        /// </summary>
        private List<FolderModel> _folders;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">The Tridion Templating Engine.</param>
        /// <param name="folder">The Tridion folder object.</param>
        public FolderModel(Engine engine, Folder folder) : base(engine, folder)
        {

        }

        /// <summary>
        /// Gets all the Components that are contained within this Folder.
        /// </summary>
        public List<ComponentModel> Components
        {
            get
            {
                if (_components == null)
                {
                    _components = new List<ComponentModel>();

                    OrganizationalItemItemsFilter componentFilter = new OrganizationalItemItemsFilter(_tridionObject.Session)
                    {
                        ItemTypes = new List<ItemType> { ItemType.Component },
                        Recursive = false
                    };

                    foreach (Component component in _tridionObject.GetItems(componentFilter))
                    {
                        _components.Add(new ComponentModel(_engine, component));
                    }
                }
                return _components;
            }
        }

        /// <summary>
        /// Gets the child Folders of this Folder (non-recursive).
        /// </summary>
        public List<FolderModel> Folders
        {
            get
            {
                if (_folders == null)
                {
                    _folders = new List<FolderModel>();

                    OrganizationalItemItemsFilter filter = new OrganizationalItemItemsFilter(_tridionObject.Session)
                    {
                        ItemTypes = new List<ItemType> { ItemType.Folder },
                        Recursive = false
                    };

                    foreach (Folder folder in _tridionObject.GetItems(filter))
                    {
                        _folders.Add(new FolderModel(_engine, folder));
                    }
                }
                return _folders;
            }
        }

        /// <summary>
        /// Gets whether the Folder is the root or not.
        /// </summary>
        public bool IsRoot
        {
            get { return _tridionObject.IsRootOrganizationalItem; }
        }

        /// <summary>
        /// Gets the parent Folder.
        /// </summary>
        public FolderModel Parent
        {
            get { return OrganizationalItem; }
        }
    }
}
